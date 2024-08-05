using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SPMA.Core.ExtensionMethods;
using Microsoft.EntityFrameworkCore;
using SPMA.Controllers.Optima;
using SPMA.Data;
using SPMA.Dtos.Production;
using SPMA.Models.Production;
using System;
using System.Linq;
using SPMA.Core.ExtensionMethods.Paging;

namespace SPMA.Controllers.Production
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        #region Properties
        private ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ComponentsController _componentsController;
        #endregion

        #region Constructor
        public BookController(ApplicationDbContext dbContext, IMapper mapper,
            ComponentsController componentsController
            )
        {
            _context = dbContext;
            _mapper = mapper;
            _componentsController = componentsController;
        }
        #endregion

        #region Http Action Requests

        /// <summary>
        /// Gets books count
        /// </summary>
        /// <returns></returns>
        [HttpGet("bookscount")]
        public IActionResult GetAllBooksCount()
        {
            var booksCount = _context.Books.Count();

            return Ok(booksCount);
        }

        /// <summary>
        /// Gets book single component
        /// </summary>
        /// <param name="bookId"></param>
        /// <param name="componentId"></param>
        /// <returns></returns>
        [HttpGet("getSingleComponent")]
        public IActionResult GetSingleComponentByBookIdAndComponentId(int bookId, int componentId)
        {
            var component = _context.BookComponents.Where(bc => bc.BookId == bookId & bc.ComponentId == componentId).SingleOrDefault();

            return Ok(component);
        }

        /// <summary>
        /// Gets paged books result
        /// </summary>
        /// <param name="orderBy"></param>
        /// <param name="sortOrder"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpGet("getpagedbooks")]
        public IActionResult GetBooks(string orderBy, string sortOrder, [FromQuery]int page, [FromQuery]int pageSize, string filter)
        {
            if (orderBy == null) orderBy = "BookId";
            bool asc = true;
            if (sortOrder == "desc")
            {
                asc = false;
            }

            var books = _context.Books
                .Include(book => book.BookComponents).ThenInclude(bc => bc.Component)
                .Select(bc => new BookDto
                {
                    BookId = bc.BookId,
                    OfficeNumber = bc.OfficeNumber,
                    Name = bc.Name,
                    ComponentNumber = bc.BookComponents.Where(item => item.Order == 0).Select(item => item.Component.Number).FirstOrDefault(),
                    ModifiedDate = bc.ModifiedDate
                })
                .Where(item => EF.Functions.Like(item.OfficeNumber, $"%{filter}%") ||
                               EF.Functions.Like(item.Name, $"%{filter}%") ||
                               EF.Functions.Like(item.ComponentNumber, $"%{filter}%"))
                //.Where(item => item.OfficeNumber.ToLower().Contains(filter) ||
                //               item.Name.ToLower().Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                //               item.ComponentNumber.ToLower().Contains(filter, StringComparison.OrdinalIgnoreCase))
                .OrderBy(orderBy, asc)
                .GetPaged(page, pageSize);

            int pos = (pageSize * (page - 1)) + pageSize + 1;
            foreach (BookDto book in books.Results)
            {
                book.Position = pos;
                pos++;
            }
            return Ok(books);
        }

        /// <summary>
        /// GET /api/book - Gets specified book components list
        /// </summary>
        /// <param name="componentNumber"></param>
        /// <param name="officeNumber"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetBook(string componentNumber, string officeNumber)
        {
            if (componentNumber == null)
                return BadRequest();
            var bookComponent = _context.BookComponents
                .Include(b => b.Book)
                .Include(c => c.Component)
                .Where(bc => bc.Component.Number == componentNumber && bc.Book.OfficeNumber == officeNumber && bc.Level == 0)
                .SingleOrDefault();

            return Ok(bookComponent);
        }

        /// <summary>
        /// Gets specified book office number
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/officenumber")]
        public IActionResult GetBookOfficeNumber(int? id)
        {
            if (id == null)
                return BadRequest("id cannot be null");

            var bookOfficeNumber = _context.Books.Where(b => b.BookId == id).Select(b => b.OfficeNumber).SingleOrDefault();

            return Ok(bookOfficeNumber);
        }

        /// <summary>
        /// Gets specified book component number
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/componentnumber")]
        public IActionResult GetBookComponentNumber(int? id)
        {
            if (id == null)
                return BadRequest("id cannot be null");

            var bookComponentNumber = _context.BookComponents
                .Where(b => b.BookId == id && b.Level == 0).Select(b => b.Component.Number).SingleOrDefault();
            return Ok(bookComponentNumber);
        }

        /// <summary>
        /// Gets book name
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/name")]
        public IActionResult GetBookName(int? id)
        {
            if (id == null)
                return BadRequest("id cannot be null");

            //var bookName = _context.BookComponents
            //    .Where(b => b.BookId == id && b.Level == 0).Select(b => b.Component.Name).SingleOrDefault();
            var bookName = _context.Books.Where(b => b.BookId == id).Select(b => b.Name).SingleOrDefault();
            return Ok(bookName);
        }

        /// <summary>
        /// Creates a book given bookDto
        /// </summary>
        /// <param name="bookDto"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult CreateBook(BookDto bookDto)
        {
            if (bookDto.Name == null || bookDto.OfficeNumber == null)
                return BadRequest();

            var bookInDb = _context.Books.SingleOrDefault(b => b.OfficeNumber == bookDto.OfficeNumber);

            if (bookInDb == null)
            {
                bookDto.ModifiedDate = DateTime.Now;
                _context.Books.Add(_mapper.Map<BookDto, Book>(bookDto));
                _context.SaveChanges();
                bookInDb = _context.Books.SingleOrDefault(b => b.OfficeNumber == bookDto.OfficeNumber);
            }
            return Ok(bookInDb);
        }

        /// <summary>
        /// Checks if book exists in database
        /// </summary>
        /// <param name="bookComponentDto"></param>
        /// <returns></returns>
        [HttpPatch("addcomponents/checkifbookexist")]
        public IActionResult CheckIfBookExist(BookComponentDto bookComponentDto)
        {
            // Check if book already exists
            if (_context.Books.Any(b => b.OfficeNumber == bookComponentDto.Book.OfficeNumber)
                || _context.BookComponents.Include(bc => (bc.Book)).Any(b => b.Book.OfficeNumber == bookComponentDto.Book.OfficeNumber)
                //|| _context.BookComponents.Include(bc => bc.Component).Any(b => b.Component.Number == bookComponentDto.Components[0].Number)
                )
            {
                return new JsonResult(true);
            }
            else
            {
                return new JsonResult(false);
            }
        }

        /// <summary>
        /// Adds components to specified book - insertion to bookComponent table
        /// </summary>
        /// <param name="bookComponentDto"></param>
        /// <returns></returns>
        [HttpPost("components")]
        public IActionResult AddBookComponents(BookComponentDto bookComponentDto)
        {
            // Check if BookComponent is null
            if (bookComponentDto == null)
                return Content("bookComponentDto is null.");

            // Check if book already exists
            if (_context.Books.Any(b => b.OfficeNumber == bookComponentDto.Book.OfficeNumber)
                || _context.BookComponents.Include(bc => (bc.Book)).Any(b => b.Book.OfficeNumber == bookComponentDto.Book.OfficeNumber)
                //|| _context.BookComponents.Include(bc => bc.Component).Any(b => b.Component.Number == bookComponentDto.Components[0].Number)
                )
            {
                return Content("Book already exists in database.");
            }

            // Create book if do not exist
            Book bookInDb = (CreateBook(bookComponentDto.Book) as OkObjectResult).Value as Book;

            // Add each item to BookComponent table
            foreach (ComponentDto componentDto in bookComponentDto.Components)
            {
                Component componentInDb = (_componentsController.AddComponent(componentDto) as OkObjectResult).Value as Component;

                BookComponent bookComponent = new BookComponent
                {
                    Book = bookInDb,
                    Component = componentInDb,
                    Level = componentDto.Level,
                    Order = componentDto.Order,
                    Quantity = componentDto.SinglePieceQty,
                    Ware = componentInDb.Ware,
                    WareLength = (componentDto.WareLength == null ? componentInDb.WareLength : componentDto.WareLength),
                    WareQuantity = componentInDb.WareQuantity,
                    WareUnit = componentInDb.WareUnit,
                    LastSourceType = (componentDto.LastSourceType == componentInDb.LastSourceType ? componentInDb.LastSourceType : componentDto.LastSourceType),
                    LastTechnology = (componentDto.LastTechnology == componentInDb.LastTechnology ? componentInDb.LastTechnology : componentDto.LastTechnology),
                };

                _context.BookComponents.Add(bookComponent);
            }

            _context.SaveChanges();

            return Ok(bookInDb);
        }

        /// <summary>
        /// Generates book name
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("generatenumber/{orderId}")]
        public IActionResult GenerateBookName(int orderId)
        {
            int lastOrderBookNumber;
            string typeYear;
            string orderNumber;
            string subOrderNumber;
            var year = DateTime.Now.Year;
            var lastOrderBookInDb = _context.OrderBooks.Where(ob => ob.OrderId == orderId).OrderBy(o => o.Number).LastOrDefault();

            if (lastOrderBookInDb != null)
            {
                string[] subOrderWords = lastOrderBookInDb.Number.Split("_");
                typeYear = subOrderWords[0];
                orderNumber = subOrderWords[1];
                subOrderNumber = subOrderWords[2];

                int.TryParse(subOrderNumber, out lastOrderBookNumber);

            }
            else
            {
                var orderInDb = _context.Orders.Where(o => o.OrderId == orderId).SingleOrDefault();
                string[] orderWords = orderInDb.Number.Split("_");
                typeYear = orderWords[0];
                orderNumber = orderWords[1];
                lastOrderBookNumber = 0;
            }


            string value = typeYear + "_" + orderNumber + "_" + (++lastOrderBookNumber).ToString("D2");

            return Ok(new { value });
        }
        #endregion

        /// <summary>
        /// Deletes specified book
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public IActionResult DeleteBook(int id)
        {
            var bookInDb = _context.Books.SingleOrDefault(b => b.BookId == id);

            if (bookInDb == null)
                return NotFound();

            _context.Books.Remove(bookInDb);
            _context.SaveChanges();
            return Ok();
        }

        /// <summary>
        /// Gets book info - book table
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/info")]
        public IActionResult GetBookInfo(int? id)
        {
            if (id == null)
                return BadRequest("Id cannot be null");

            Book book = _context.Books.SingleOrDefault(b => b.BookId == id);

            if(book != null)
            {
                return Ok(book);
            }
            else
            {
                return BadRequest("This book do not exist");
            }
        }

        //############### NOT USED #################

        [HttpPost("addplazmaitems")]
        //POST /api/book/addplazmaitems
        public IActionResult AddBookPlazmaItems(BookComponentDto bookComponentDto)
        {
            if (bookComponentDto == null)
                return BadRequest("bookComponentDto is null.");

            var bookInDb = _context.Books.SingleOrDefault(b => b.OfficeNumber == bookComponentDto.Book.OfficeNumber);

            if (bookInDb == null)
                return BadRequest("No book in database");


            foreach (ComponentDto componentDto in bookComponentDto.Components)
            {

                Component componentInDb = (_componentsController.AddComponent(componentDto) as OkObjectResult).Value as Component;

                //BookPlazmaItem bookPlazmaItem = new BookPlazmaItem
                //{
                //    Book = bookInDb,
                //    BookId = bookInDb.BookId,
                //    Component = componentInDb,
                //    ComponentId = componentInDb.ComponentId,
                //    Quantity = componentDto.Quantity
                //};

                //var bookPlazmaItemInDb = _context.BookPlazmaItems.Find(bookInDb.BookId, componentInDb.ComponentId);

                //if (bookPlazmaItemInDb == null)
                //{
                //    _context.BookPlazmaItems.Add(bookPlazmaItem);
                //}
                //else
                //{
                //    bookPlazmaItemInDb.Quantity += componentDto.Quantity;
                //}

            }
            _context.SaveChanges();


            return Ok();
        }


        [HttpPost("addpurchaseitems")]
        //POST /api/book/addpurchaseitems
        public IActionResult AddBookPurchaseItems(BookComponentDto bookComponentDto)
        {
            if (bookComponentDto == null)
                return BadRequest("bookComponentDto is null.");

            var bookInDb = _context.Books.SingleOrDefault(b => b.OfficeNumber == bookComponentDto.Book.OfficeNumber);

            if (bookInDb == null)
                return BadRequest("No book in database");


            foreach (ComponentDto componentDto in bookComponentDto.Components)
            {

                //PurchaseItem purchaseItemInDb = (_purchaseItemController.CreatePurchaseItem(purchaseItemDto) as OkObjectResult).Value as PurchaseItem;

                //BookPurchaseItem bookPurchaseItem = new BookPurchaseItem
                //{
                //    Book = bookInDb,
                //    BookId = bookInDb.BookId,
                //    PurchaseItem = purchaseItemInDb,
                //    PurchaseItemId = purchaseItemInDb.PurchaseItemId,
                //    Quantity = componentDto.Quantity
                //};

                //var bookPurchaseItemInDb = _context.BookPurchaseItems.Find(bookPurchaseItem.BookId, bookPurchaseItem.PurchaseItemId);

                //if (bookPurchaseItemInDb == null)
                //{
                //    _context.BookPurchaseItems.Add(bookPurchaseItem);
                //}
                //else
                //{
                //    bookPurchaseItemInDb.Quantity += componentDto.Quantity;
                //}

            }
            _context.SaveChanges();


            return Ok();
        }

        [HttpGet("getwares/{id}")]
        public IActionResult GetWares(int id)
        {
            //var componentDtosSum = _context.BookComponents.Where(b => b.BookId == id)
            //    .Include(bk => bk.Component)
            //    .ThenInclude(c => c.ComponentWares).Where(n => n.Component.ComponentWares.Count > 0)
            //        .Include(cw => cw.Component).ThenInclude(g => g.ComponentWares).ThenInclude(j => j.Ware)
            //        .SelectMany(y => y.Component.ComponentWares.Select(t => new ComponentWareDto
            //        {
            //            ComponentId = t.ComponentId,
            //            ComponentNumber = t.Component.Number,
            //            WareId = t.WareId,
            //            WareCode = t.Ware.Code,
            //            WareName = t.Ware.Name,
            //            Length = t.Length,
            //            Quantity = t.Quantity,
            //            Unit = t.Unit
            //        })).GroupBy(r => new { r.Length, r.WareCode, r.Unit, r.WareName })
            //    .Select(h => new WareStruct
            //    {
            //        WareCode = h.Key.WareCode,
            //        WareName = h.Key.WareName,
            //        Length = h.Key.Length,
            //        Unit = h.Key.Unit,
            //        Sum = h.Sum(i => i.Quantity)
            //    }).ToList();

            //var componentDtos = _context.BookComponents.Where(b => b.BookId == id)
            //    .Include(bk => bk.Component)
            //    .ThenInclude(c => c.ComponentWares).Where(n => n.Component.ComponentWares.Count > 0)
            //        .Include(cw => cw.Component).ThenInclude(g => g.ComponentWares).ThenInclude(j => j.Ware)
            //        .Select(y => new ComponentDto
            //        {
            //            ComponentId = y.ComponentId,
            //            Number = y.Component.Number,
            //            ComponentWareDtos = y.Component.ComponentWares.Select(x => new ComponentWareDto
            //            {
            //                ComponentOrder = y.Order,
            //                ComponentLevel = y.Level,
            //                ComponentWareId = x.ComponentWareId,
            //                WareId = x.WareId,
            //                WareCode = x.Ware.Code,
            //                Length = x.Length,
            //                Quantity = x.Quantity,
            //                QtyWhDiff = (Convert.ToDecimal((_optimaController.GetWarehouseItemQty(x.Ware.Code) as OkObjectResult).Value)) - (x.Length * x.Quantity),
            //                Unit = x.Unit
            //            }).ToList()
            //        }).ToList();

            //componentDtos.ForEach(item =>
            //{
            //    item.ComponentWareDtos.ForEach(x => {
            //        x.TotalQuantity = componentDtosSum.Where(v => v.WareCode == x.WareCode).Select(t => t.Sum).First();
            //    });
            //});


            return Ok(/*componentDtos*/);
        }

    }


}