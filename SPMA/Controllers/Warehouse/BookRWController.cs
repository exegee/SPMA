using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPMA.Controllers.Optima;
using SPMA.Controllers.Production;
using SPMA.Data;
using SPMA.Dtos.Production;
using SPMA.Models.Orders;
using SPMA.Models.Production;
using SPMA.Models.Warehouse;
using System;
using System.Collections.Generic;
using System.Linq;
using static SPMA.Models.Shared.Structs;

namespace SPMA.Controllers.Warehouse
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookRWController : ControllerBase
    {
        #region Properties
        private ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly BookController _bookController;
        private readonly OptimaController _optimaController; 
        #endregion

        #region Constructor
        public BookRWController(ApplicationDbContext dbContext, IMapper mapper,
            BookController bookController,
            OptimaController optimaController)
        {
            _bookController = bookController;
            _optimaController = optimaController;
            _context = dbContext;
            _mapper = mapper;
        }
        #endregion

        #region Http Action Requests

        /// <summary>
        /// Adds bookcomponentRW records based on book id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("{id}")]
        //Post /api/bookrw/id
        public IActionResult AddBookRW(int id)
        {
            //// Get orderNumber from header
            //string orderNumber = HttpContext.Request.Headers["orderNumber"];

            //// Get book by id
            //Book book = _context.Books.Where(b => b.BookId == id).SingleOrDefault();

            //// Get order by number
            //Order order = _context.Orders.Where(o => o.Number == orderNumber).SingleOrDefault();

            //// If order book RW exists return
            //bool rwExist = _context.BookComponentRWs.Any(bc => bc.Order == order && bc.Book == book);
            //if (rwExist)
            //    return Ok("This book already have RW");

            //// Get book quantity
            //int bookQuantity = _context.OrderBooks.Where(b => b.BookId == id).Select(s => s.PlannedQty).SingleOrDefault();

            //// Get book wares
            //var bookWares= _context.BookComponents.Where(b => b.BookId == id)
            //    .Include(bk => bk.Component)
            //    .ThenInclude(c => c.ComponentWares)
            //    //.Where(n => n.Component.ComponentWares.Count > 0)
            //        .Include(cw => cw.Component).ThenInclude(g => g.ComponentWares).ThenInclude(j => j.Ware)
            //        //.SelectMany(y => y)
            //        //.GroupBy(t => new { t.Ware.Code, t.Length, t.Quantity})
            //        .ToList();

            //// Add each Ware to BookComponentRWs table
            //foreach (BookComponent bookComponent in bookWares)
            //{
            //    // Get component order 
            //    int componentOrder = bookComponent.Order;
            //    // Get component level
            //    int componentLevel = bookComponent.Level;
            //    // Get component quanity
            //    int componentQuantity = bookComponent.Quantity;
                
            //    foreach (ComponentWare componentWare in bookComponent.Component.ComponentWares)
            //    {
            //        // Get component ware quanity
            //        int componentWareQuantity = componentWare.Quantity;

            //        BookComponentRW bookComponentRW = new BookComponentRW
            //        {
            //            Order = order,
            //            Book = book,
            //            Issued = 0,
            //            ComponentWare = componentWare,
            //            ToIssue = bookQuantity * componentQuantity * componentWareQuantity,
            //            ComponentOrder = componentOrder,
            //            ComponentLevel = componentLevel
            //        };

            //        // Add book component to table
            //        _context.BookComponentRWs.Add(bookComponentRW);
            //    }
            //}


            //// Get purchase items 
            //var bookPurchaseItems = _context.BookPurchaseItems.Where(b => b.BookId == id)
            //    .Include(bpi => bpi.PurchaseItem)
            //    .ThenInclude(c => c.Ware)
            //    .Where(n => n.PurchaseItem.WareId != null)
            //        //.Include(cw => cw.Component).ThenInclude(g => g.ComponentWares).ThenInclude(j => j.Ware)
            //        //.SelectMany(y => y)
            //        //.GroupBy(t => new { t.Ware.Code, t.Length, t.Quantity})
            //        .ToList();

            //// Add each PurchaseItem to BookPurchaseItemRWs
            //foreach (BookPurchaseItem bookPurchaseItem in bookPurchaseItems)
            //{
            //    // Get purchase item
            //    PurchaseItem purchaseItem = bookPurchaseItem.PurchaseItem;
            //    // Get purchase item quantity
            //    int purchaseItemQuantity = bookPurchaseItem.Quantity;

            //    BookPurchaseItemRW bookPurchaseItemRW = new BookPurchaseItemRW
            //    {
            //        Order = order,
            //        Book = book,
            //        Issued = 0,
            //        PurchaseItem = purchaseItem,
            //        ToIssue = bookQuantity * purchaseItemQuantity
            //    };
            //    // Add purchase item to table
            //    _context.BookPurchaseItemRWs.Add(bookPurchaseItemRW);
            //}
            //// Save changes to database
            //_context.SaveChanges();

            //// Calculate duplicates
            //List<BookComponentRW> bookComponentRWs = _context.BookComponentRWs.Where(bc => bc.BookId == id).ToList();
            //bookComponentRWs.ForEach(bc => {
            //    bc.TotalToIssue = bookComponentRWs.Where(b => b.ComponentWare == bc.ComponentWare).Sum(i => i.ToIssue);
            //});

            //// Save changes to database
            //_context.SaveChanges();

            return Ok();
        }

        //[HttpGet("{id}")]
        ////GET /api/bookrw/id
        //public IActionResult GetBookRWStatus(int id)
        //{
            //// Get orderNumber from header
            //string orderNumber = HttpContext.Request.Headers["orderNumber"];

            //// Get order by number
            //Order order = _context.Orders.Where(o => o.Number == orderNumber).SingleOrDefault();

            //var percentage = new Dictionary<string, double>();

            //// Get components to issue and issued
            //var componentsToIssue = _context.BookComponentRWs
            //    .Where(bc => bc.BookId == id && bc.OrderId == order.OrderId)
            //    .Sum(s => s.ToIssue);

            //var componentsIssued = _context.BookComponentRWs
            //    .Where(bc => bc.BookId == id && bc.OrderId == order.OrderId)
            //    .Sum(s => s.Issued);

            //percentage.Add("Components", componentsIssued == 0 ? componentsIssued : componentsToIssue / componentsIssued);

            //// Get purchase items to issue and issued
            //var purchaseItemsToIssue = _context.BookPurchaseItemRWs
            //    .Where(bc => bc.BookId == id && bc.OrderId == order.OrderId)
            //    .Sum(s => s.ToIssue);

            //var purchaseItemsIssued = _context.BookPurchaseItemRWs
            //    .Where(bc => bc.BookId == id && bc.OrderId == order.OrderId)
            //    .Sum(s => s.Issued);

            //percentage.Add("Purchase Items", purchaseItemsIssued == 0 ? purchaseItemsIssued : purchaseItemsToIssue / purchaseItemsIssued);

            //return Ok(percentage);
        //}

        //[HttpGet("{id}")]
        public IActionResult GetBookComponentRW(int id, Order order)
        {

            //// Get components to issue and issued
            //var componentsToIssue = _context.BookComponentRWs
            //        .Where(bc => bc.BookId == id && bc.Order == order);

            //var componentsIssued = _context.BookComponentRWs
            //    .Where(bc => bc.BookId == id && bc.Order == order);

            return Ok();
        }


        /// <summary>
        /// Updates Book Component RW - this function issues only the available amount from the Optima Warehouse
        /// </summary>
        /// <param name="bookComponentRW"></param>
        /// <returns></returns>
        //[HttpPatch("{id}")]
        //public List<BookComponentRW> UpdateBookRW(BookComponentRW bookComponentRW)
        //{
            //// Get book components wares with specified order and book ID and that are left to issue
            //List<BookComponentRW> componentsToIssue = _context.BookComponentRWs
            //        .Where(bc => bc.Book == bookComponentRW.Book && bc.Order == bookComponentRW.Order && bc.Issued != bc.ToIssue)
            //        .Include(bcc => bcc.ComponentWare).ThenInclude(cw => cw.Ware).Include(c => c.ComponentWare.Component)
            //        .ToList();

            //// Locally stores issued ammount of wares
            //List<WareStruct> wareIssuedAmount = new List<WareStruct>();

            //foreach (BookComponentRW rwItem in componentsToIssue)
            //{
            //    string rwItemWareCode = rwItem.ComponentWare.Ware.Code;
            //    // Check how many items are left to issue
            //    int rwItemQtyLeftToIssue = rwItem.ToIssue - rwItem.Issued;

            //    // If 0 then skip this item
            //    if (rwItemQtyLeftToIssue == 0)
            //        continue;

            //    // Check if current ware have been already issued
            //    var issuedRwItem = wareIssuedAmount.Find(f => f.WareCode == rwItemWareCode);

            //    decimal wareAvailableLength = Convert.ToDecimal((_optimaController.GetWarehouseItemQty(rwItem.ComponentWare.Ware.Code) as OkObjectResult).Value);
            //    decimal rwItemLength = rwItem.ComponentWare.Length;
            //    int issuedRwItemQty = issuedRwItem.WareCode == null ? 0 : (int)(issuedRwItem.Length / rwItemLength);

            //    // Check integer count - round to floor substracted by issuedRwItemQty
            //    int integerCount = (int)Math.Floor(wareAvailableLength / rwItemLength) - issuedRwItemQty;

            //    // Clamp the integer count between 0 and rwItemQtyLeftToIssue
            //    int rwItemAvailableQtyToIssue = Math.Clamp(integerCount, 0, rwItemQtyLeftToIssue);

            //    // Remember ware issued amount from the Optima warehouse
            //    if (issuedRwItem.WareCode == null)
            //    {
            //        wareIssuedAmount.Add(new WareStruct()
            //        {
            //            WareCode = rwItemWareCode,
            //            Length = rwItemAvailableQtyToIssue * rwItemLength
            //        });
            //    }
            //    else
            //    {
            //        issuedRwItem.Length += rwItemAvailableQtyToIssue * rwItemLength;
            //    }

            //    // If integerCount bigger or equal 1 issue the amount available
            //    if (integerCount >= 1)
            //    {
            //        rwItem.Issued += rwItemAvailableQtyToIssue;
            //    }
            //}
            //// Save changes do database
            //_context.SaveChanges();

            //return componentsToIssue;
        //}
        #endregion
    }
}