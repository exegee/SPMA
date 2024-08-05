using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SPMA.Core.ExtensionMethods;
using Microsoft.EntityFrameworkCore;
using SPMA.Data;
using SPMA.Dtos.Orders;
using SPMA.Models.Orders;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using SPMA.Dtos.Production;
using System.Threading.Tasks;
using SPMA.Models.Production;
using SPMA.Core.ExtensionMethods.Paging;
using System.Collections.Generic;
using System.Diagnostics;
using SPMA.Controllers.Production;
using SPMA.Controllers.Optima;
using SPMA.Models.Optima;
using static SPMA.Core.Enums;

namespace SPMA.Controllers.Orders
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        #region Properties
        private ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly BookController _bookController;
        private readonly OptimaController _optimaController;
        #endregion

        #region Constructor
        public OrdersController(ApplicationDbContext dbContext, IMapper mapper, BookController bookController, OptimaController optimaController)
        {
            _context = dbContext;
            _mapper = mapper;
            _bookController = bookController;
            _optimaController = optimaController;
        }
        #endregion

        #region Http Action Requests
        //GET /api/orders
        [HttpGet]
        public IActionResult GetOrders(string orderBy, string sortOrder, [FromQuery] int orderState, [FromQuery] int page, [FromQuery] int pageSize, string filterByName, string filterByNumber, string filterByClientName)
        {
            if (orderBy == null) orderBy = "OrderId";
            bool asc = true;
            if (sortOrder == "desc")
            {
                asc = false;
            }

            var orders = _context.Orders
                .Select(o => new OrderDto
                {
                    Name = o.Name,
                    Number = o.Number,
                    ClientName = o.ClientName,
                    Comment = o.Comment,
                    FinishedQty = o.FinishedQty,
                    PlannedQty = o.PlannedQty,
                    AddedById = o.AddedById,
                    OrderDate = o.OrderDate,
                    OrderId = o.OrderId,
                    RequiredDate = o.RequiredDate,
                    ShippedDate = o.ShippedDate,
                    ShippingAddress = o.ShippingAddress,
                    ShippingCity = o.ShippingCity,
                    ShippingCountry = o.ShippingCountry,
                    ShippingName = o.ShippingName,
                    ShippingPostalCode = o.ShippingPostalCode,
                    ShippingRegion = o.ShippingRegion,
                    State = o.State,
                    Type = o.Type,
                    RwCompletion = o.RwCompletion
                }).Where(item => (EF.Functions.Like(item.Name, $"%{filterByName}%") &
                               EF.Functions.Like(item.Number, $"%{filterByNumber}%") &
                               EF.Functions.Like(item.ClientName, $"%{filterByClientName}%")) &
                               item.State == orderState)
                .OrderBy(orderBy, asc)
                .GetPaged(page, pageSize);


            PagedResult < OrderDto> pagedOrders = orders;

            //switch (filterBy)
            //{
            //    case "Name":
            //        pagedOrders=orders.Where(item => (
            //                  EF.Functions.Like(item.Name, $"%{filter}%") &&
            //                  item.State != 10)).OrderBy(orderBy, asc).GetPaged(page, pageSize);
            //        break;
            //    case "Number":
            //        pagedOrders = orders.Where(item => (EF.Functions.Like(item.Number, $"%{filter}%") &&
            //                  item.State != 10)).OrderBy(orderBy, asc).GetPaged(page, pageSize);
            //        break;
            //    case "ClientName":
            //        pagedOrders = orders.Where(item => (EF.Functions.Like(item.ClientName, $"%{filter}%") &&
            //                  item.State != 10)).OrderBy(orderBy, asc).GetPaged(page, pageSize);
            //        break;
            //    default:
            //        pagedOrders = orders.Where(item => (EF.Functions.Like(item.Name, $"%{filter}%") ||
            //                  EF.Functions.Like(item.Number, $"%{filter}%") ||
            //                  EF.Functions.Like(item.ClientName, $"%{filter}%")) &&
            //                  item.State != 10).OrderBy(orderBy, asc).GetPaged(page, pageSize);
            //        break;
            //}



            int pos = (pageSize * (page - 1)) + pageSize + 1;
            foreach (OrderDto order in pagedOrders.Results)
            {                
                order.Position = pos;
                pos++;
            }
            return Ok(pagedOrders);
        }


        //gives list of filtered orders including archive orders
        [HttpGet ("filteredorders")]
        public IActionResult GetFilteredOrders(string filter)
        {
            var orderBy = "OrderId";
            bool asc = true;

            var orders = _context.Orders
                .Select(o => new OrderDto
                {
                    Name = o.Name,
                    Number = o.Number,
                    ClientName = o.ClientName,
                    Comment = o.Comment,
                    FinishedQty = o.FinishedQty,
                    PlannedQty = o.PlannedQty,
                    AddedById = o.AddedById,
                    OrderDate = o.OrderDate,
                    OrderId = o.OrderId,
                    RequiredDate = o.RequiredDate,
                    ShippedDate = o.ShippedDate,
                    ShippingAddress = o.ShippingAddress,
                    ShippingCity = o.ShippingCity,
                    ShippingCountry = o.ShippingCountry,
                    ShippingName = o.ShippingName,
                    ShippingPostalCode = o.ShippingPostalCode,
                    ShippingRegion = o.ShippingRegion,
                    State = o.State,
                    Type = o.Type,
                    RwCompletion = o.RwCompletion
                }).Where(item => (EF.Functions.Like(item.Name, $"%{filter}%") |
                               EF.Functions.Like(item.Number, $"%{filter}%") |
                               EF.Functions.Like(item.ClientName, $"%{filter}%"))
                              )
                .OrderBy(orderBy, asc);

            return Ok(orders);
        }

        [HttpGet("simplelist")]
        public IActionResult GetOrdersSimpleList()
        {
            List<OrderDto> orders = _context.Orders
                .Select(o => new OrderDto
                {
                    Name = o.Name,
                    Number = o.Number,
                    ClientName = o.ClientName,
                    Comment = o.Comment,
                    FinishedQty = o.FinishedQty,
                    PlannedQty = o.PlannedQty,
                    AddedById = o.AddedById,
                    OrderDate = o.OrderDate,
                    OrderId = o.OrderId,
                    RequiredDate = o.RequiredDate,
                    ShippedDate = o.ShippedDate,
                    ShippingAddress = o.ShippingAddress,
                    ShippingCity = o.ShippingCity,
                    ShippingCountry = o.ShippingCountry,
                    ShippingName = o.ShippingName,
                    ShippingPostalCode = o.ShippingPostalCode,
                    ShippingRegion = o.ShippingRegion,
                    State = o.State,
                    Type = o.Type,
                    RwCompletion = o.RwCompletion
                }).Where(item => item.State != 10).ToList();

            return Ok(orders);
        }

        /// <summary>
        /// Gets rw status in % for order
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/statusrw")]
        public IActionResult GetRWStatus(int id)
        {
            decimal statusRW = 0;
            // Get suborder for current order
            List<OrderBookDto> suborders = _context.OrderBooks
                .Where(ob => ob.OrderId == id)
                .Select(ob => new OrderBookDto
                {
                    BookId = ob.BookId,
                    Number = ob.Number,
                    PlasmaInList = ob.PlasmaInList,
                    PlasmaOutList = ob.PlasmaOutList,
                    PurchaseList = ob.PurchaseList,
                    WareList = ob.WareList
                }).AsNoTracking().ToList();

            int orderListCount = 0;
            int orderListBufferRW = 0;
            // Checks each suborders book in optima database and returns its RW's
            foreach (OrderBookDto suborder in suborders)
            {
                orderListCount += suborder.PlasmaInList + suborder.PlasmaOutList + suborder.PurchaseList + suborder.WareList;
                suborder.ComponentNumber = (_bookController.GetBookComponentNumber(suborder.BookId) as OkObjectResult).Value as string;
                if ((suborder.PlasmaInList == 1) | (suborder.PlasmaOutList == 1) | (suborder.PurchaseList == 1) | (suborder.WareList == 1))
                {
                    List<OptimaRW> rws =
                        (_optimaController.GetRWNumberByDescFilter(suborder.Number.Substring(0, 9), suborder.Number, suborder.ComponentNumber) as OkObjectResult).Value as List<OptimaRW>;
                    foreach(OptimaRW rw in rws)
                    {
                        if (rw.TrN_Bufor >= 0)
                        {
                            ++orderListBufferRW;
                        }
                        
                    }

                }
            }
            // Calculates progress for each suborder in %
            if(orderListBufferRW > 0)
            {
                statusRW = ((decimal)orderListBufferRW / orderListCount)*100;
            }
            return Ok(statusRW);
        }

        //GET /api/orders/id
        [HttpGet("{id}")]
        public IActionResult GetOrder(int id)
        {
            var order = _context.Orders.SingleOrDefault(o => o.OrderId == id);

            if (order == null)
                return BadRequest("No such order!");

            return Ok(_mapper.Map<Order, OrderDto>(order));
        }

        //GET /api/orders/id
        [HttpGet("async/{id}")]
        public IActionResult GetOrderAsync(int id)
        {
            var order = _context.Orders.SingleOrDefault(o => o.OrderId == id);

            if (order == null)
                return BadRequest("No such order!");

            return Ok(_mapper.Map<Order, OrderDto>(order));
        }

        //GET /api/orders/id/books
        [HttpGet("{id}/books")]
        public IActionResult GetOrderBooks(int id)
        {
            var index = 0;
            OrderBookDto[] orderBooks = _context.OrderBooks.Where(o => o.OrderId == id)
                .Include(o => o.Order)
                .Include(o => o.Book).ThenInclude(o => o.BookComponents).ThenInclude(bk => bk.Component)
                .Select(b => new OrderBookDto
                {
                    OrderBookId = b.OrderBookId,
                    Order = _mapper.Map<Order, OrderDto>(b.Order),
                    Book = _mapper.Map<Book, BookDto>(b.Book),
                    PlannedQty = b.PlannedQty,
                    FinishedQty = b.FinishedQty,
                    AddedDate = b.AddedDate,
                    Comment = b.Comment,
                    Number = b.Number,
                    ComponentNumber = b.Book.BookComponents.Where(x=>x.Level==0).Select(y=>y.Component.Number).FirstOrDefault(),
                    WareList = b.WareList,
                    PurchaseList = b.PurchaseList,
                    PlasmaInList = b.PlasmaInList,
                    PlasmaOutList = b.PlasmaOutList
                }).OrderBy(ob=>ob.Number)
                .ToArray();

            foreach (OrderBookDto orderBookDto in orderBooks)
            {
                orderBookDto.Position = index;
                Debug.WriteLine(orderBookDto.ComponentNumber);
                index++;
            }
            if (orderBooks == null)
                return NotFound();

            return Ok(orderBooks);
        }


        [HttpPost]
        //POST /api/orders
        public IActionResult CreateOrder(OrderDto orderDto)
        {
            if (orderDto == null)
                return BadRequest();

            var order = _mapper.Map<OrderDto, Order>(orderDto);

            _context.Orders.Add(order);
            _context.SaveChanges();
            //TODO dodac aktualizowanie ilosci w inProduction i inProductionRW
            return Ok();
        }

        [HttpPut("{id}")]
        //PUT /api/orders/id
        public IActionResult UpdateOrder(int id, OrderDto orderDto)
        {
            if (orderDto == null)
                return NotFound();


            var orderInDb = _context.Orders.SingleOrDefault(o => o.OrderId == id);

            if (orderInDb == null)
                return NotFound();

            _mapper.Map(orderDto, orderInDb);

            List<OrderBook> orderBooks = _context.OrderBooks
            .Where(ob => ob.OrderId == orderInDb.OrderId)
            .ToList();

            //pick inProduction items from one orderBook
            for (int i = 0; i < orderBooks.Count; i++)
            {
                List<InProduction> inProduction = _context.InProduction
                     .Include(ip => ip.Component)
                     .Where(ip => ip.OrderBookId == orderBooks[i].OrderBookId)
                     .ToList();

                InProductionRW inProductionRW;

                //actualize quantities in every single inProduction
                for(int j = 0; j < inProduction.Count; j++)
                {
                    inProduction[j].PlannedQty = orderInDb.PlannedQty * orderBooks[i].PlannedQty * inProduction[j].BookQty;
                    inProductionRW = _context.InProductionRWs
                        .Where(rw => rw.InProductionId == inProduction[j].InProductionId)
                        .FirstOrDefault();
                    inProductionRW.ToIssue = inProduction[j].PlannedQty;

                    //Calculate duplicates and total amount of components
                    int totalAmount = 0;
                    foreach (InProduction item in inProduction)
                    {
                        if(item.ComponentId == inProduction[j].ComponentId)
                        {
                            totalAmount += item.BookQty;
                        }
                    }

                    inProductionRW.TotalToIssue = totalAmount * orderInDb.PlannedQty * orderBooks[i].PlannedQty;
                }
            }

            _context.SaveChanges();
            return Ok();
        }

        [HttpDelete("{id}")]
        //DELETE /api/orders/id
        public IActionResult DeleteOrder(int id)
        {
            var orderInDb = _context.Orders.SingleOrDefault(o => o.OrderId == id);

            if (orderInDb == null)
                return NotFound();

            _context.Orders.Remove(orderInDb);
            _context.SaveChanges();
            return Ok();
        }

        [HttpGet]
        //GET /api/orders
        [Route("generatenumber/{orderType}")]
        public IActionResult GenerateOrderName(int orderType, string orderYear)
        {
            int lastOrderSubNumber;
            int year = Int16.Parse(orderYear);
            var lastOrderInDb = _context.Orders.Where(o => (o.Type == orderType && o.OrderDate.Value.Year == year)).OrderBy(o => o.Number).LastOrDefault();
            if (lastOrderInDb != null)
            {
                string lastOrdernumber = lastOrderInDb.Number;

                Regex regex = new Regex(@"_\d+");

                int.TryParse(regex.Match(lastOrdernumber).ToString().TrimStart('_'), out lastOrderSubNumber);

            }
            else
            {
                lastOrderSubNumber = 0;
            }


            string orderTypeString;
            switch (orderType)
            {
                case 0:
                    orderTypeString = "K";
                    break;
                case 1:
                    orderTypeString = "R";
                    break;
                case 2:
                    orderTypeString = "W";
                    break;
                default:
                    orderTypeString = "K";
                    break;
            }

            string value = orderTypeString + year + "_" + (++lastOrderSubNumber).ToString("D3");

            return Ok(new { value });
        }

        
        /// <summary>
        /// Adds book to OrdersBook table ( add book to order )
        /// </summary>
        /// <param name="orderBookDto"></param>
        /// <returns></returns>
        [HttpPost("book")]
        //POST /api/orders/book
        public IActionResult AddBook(OrderBookDto orderBookDto)
        {

            if (orderBookDto == null)
                return Content("orderBookDto is null.");

            var orderInDb = _context.Orders.SingleOrDefault(o => o.Number == orderBookDto.Order.Number);

            var bookInDb = _context.Books.SingleOrDefault(b => b.OfficeNumber == orderBookDto.Book.OfficeNumber);

            if ((orderInDb == null) || (bookInDb == null))
                return Content("No such order or book.");

            OrderBook orderBook = new OrderBook
            {
                Order = orderInDb,
                Book = bookInDb,
                PlannedQty = orderBookDto.PlannedQty,
                AddedDate = orderBookDto.AddedDate,
                AddedById = 0,
                Comment = "",
                FinishedQty = 0,
                Number = orderBookDto.Number
            };

            _context.OrderBooks.Add(orderBook);
            _context.SaveChanges();

            
            return Ok();
        }

        /// <summary>
        /// Deletes book from OrderBook table ( delete book from order )
        /// </summary>
        /// <returns></returns>
        [HttpDelete("book")]
        //DELETE /api/orders/addbook
        public async Task<IActionResult> DeleteBook()
        {
                // Get http request headers
                int orderId = Convert.ToInt32(HttpContext.Request.Headers["orderId"]);
                int bookId = Convert.ToInt32(HttpContext.Request.Headers["bookId"]);
                string subOrderNumber = HttpContext.Request.Headers["subOrderNumber"];

            var orderBookInDb = _context.OrderBooks
                .Where(ob => ob.OrderId == orderId && ob.BookId == bookId && ob.Number == subOrderNumber).AsNoTracking()
                .Single();

            await _context.OrderBooks
            .Where(ob => ob.OrderId == orderId && ob.BookId == bookId && ob.Number == subOrderNumber).AsNoTracking()
            .DeleteFromQueryAsync();
            await _context.InProduction
              .Where(p => p.OrderBookId == orderBookInDb.OrderBookId)
              .Select(o => new InProduction
              {
                  InProductionId = o.InProductionId
              }).DeleteFromQueryAsync();

            //var inProductionComponentsInDb = _context.InProduction
            //    .Include(ip => ip.OrderBook)
            //    .ThenInclude(ob => ob.Book)
            //    .Where(p => p.OrderBook.Number == subOrderNumber &&
            //           p.OrderBook.BookId == bookId)
            //    .AsNoTracking().ToList();

            //var inProductionComponentsInDb = _context.InProduction
            //  .Where(p => p.OrderBookId == orderBookInDb.OrderBookId)
            //  .Select(o => new InProduction
            //  {
            //      InProductionId = o.InProductionId
            //  }).DeleteFromQueryAsync();

            //if (orderBookInDb != null)
            //    {
            //        _context.OrderBooks.Remove(orderBookInDb);
            //    }
            //    if (inProductionComponentsInDb != null)
            //    { 
            //        _context.InProduction.RemoveRange(inProductionComponentsInDb);
            //    }
            //_context.Database.SetCommandTimeout(180);
            //try
            //{
            //    await _context.SaveChangesAsync();

            //}
            //catch(Exception e)
            //{
            //    Console.WriteLine(e.Message);
            //}
            return Ok();
        }

        [HttpGet("searchorder")]
        public IActionResult SearchOrder(string filter)
        {
            List<Order> ordersInDb = _context.Orders
                .Where(item => EF.Functions.Like(item.Name, $"%{filter}%") ||
                               EF.Functions.Like(item.Number, $"%{filter}%") ||
                               EF.Functions.Like(item.ClientName, $"%{filter}%"))
                .ToList();

            List<OrderDto> ordersDtos = _mapper.Map<List<Order>, List<OrderDto>>(ordersInDb);

            return Ok(ordersDtos);
        }

        // PATCH /api/orders/archive
        [HttpPatch("archive")]
        public IActionResult ArchiveOrder()
        {
            // Get http request headers
            int orderId = Convert.ToInt32(HttpContext.Request.Headers["orderId"]);
            int orderState = Convert.ToInt32(HttpContext.Request.Headers["orderState"]);

            var order = _context.Orders.First(x => x.OrderId == orderId);
            order.State = orderState;
            _context.SaveChanges();
            return Ok();
        }

        // GET /api/orders/archive
        [HttpGet("archive")]
        public IActionResult GetArchiveOrders()
        {
            List<Order> archiveOrders = _context.Orders.Where(order => order.State == 10).ToList();

            if (archiveOrders != null)
            {
                List<OrderDto> archiveOrdersDto = _mapper.Map<List<Order>, List<OrderDto>>(archiveOrders);
                return Ok(archiveOrders);
            }
            else
            {
                return Ok("No orders found");
            }

        }


        [HttpPost("copyorder/{existingOrderId}")]
        public IActionResult CopyOrder(int existingOrderId, [FromBody] OrderDto newOrderDto)
        {

            #region creating new order in DB
            //create new Order in DB
            if (newOrderDto == null)
                BadRequest("newOrder can't be null");

            int oldOrderQty = _context.Orders.Where(o => o.OrderId == existingOrderId).Select(o => o.PlannedQty).FirstOrDefault();
            int newOrderQty = (int)newOrderDto.PlannedQty;

            var newOrder = _mapper.Map<OrderDto, Order>(newOrderDto);
            _context.Orders.Add(newOrder);

            #endregion


            #region creating new orderBooks in DB
            //get existing orderbooks
            List<OrderBook> existingOrderBooks = _context.OrderBooks.Include(o => o.Book).Where(ob => ob.OrderId == existingOrderId).ToList();
            List<OrderBook> newOrderBooks = new List<OrderBook>();

            //checking if it's not empty
            if (existingOrderBooks.Count == 0)
                BadRequest("No OrderBooks for this \"existingOrderId\"");

            //copy to new OrderBooks
            int index = 1;


            foreach(OrderBook obitem in existingOrderBooks)
            {
                OrderBook temp = new OrderBook();

                temp.Order = newOrder;
                temp.AddedDate = newOrder.OrderDate;
                temp.Number = orderBookNumberGen(newOrder.Number, index);
                temp.Book = obitem.Book;
                temp.PlannedQty = obitem.PlannedQty;
                temp.Comment = obitem.Comment;
                temp.ProductionState = obitem.ProductionState;
                temp.PlasmaInList = obitem.PlasmaInList;
                temp.PlasmaOutList = obitem.PlasmaOutList;
                temp.PurchaseList = obitem.PurchaseList;
                temp.WareList = obitem.WareList;
                newOrderBooks.Add(temp);
                _context.OrderBooks.Add(temp);
                index++;
            }

            #endregion

            #region create new inProduction in DB

            foreach (OrderBook obitem in existingOrderBooks)
            {

                //corresponding new orderBook
                OrderBook newOrderBookItem = newOrderBooks.Find(n => n.BookId == obitem.BookId);

                List<InProduction> existingInProduction = _context.InProduction
                    .Include(ip=>ip.Component)
                    .Include(ip=>ip.ProductionState)
                    .Include(ip=>ip.Ware)
                    .Include(ip=>ip.ProductionSocket)
                    .Where(ip => ip.OrderBookId == obitem.OrderBookId)
                    .OrderBy(ip => ip.InProductionId).ToList();

                
                //create new inProduction and inProductionRW
                foreach (InProduction ipitem in existingInProduction)
                {
                    InProduction inProdItem = new InProduction();

                    inProdItem.OrderBook = newOrderBookItem;
                    inProdItem.Component = ipitem.Component;
                    inProdItem.PlannedQty = ipitem.PlannedQty*newOrderQty/oldOrderQty;
                    inProdItem.BookQty = ipitem.BookQty;
                    inProdItem.ProductionSocket = ipitem.ProductionSocket;
                    inProdItem.ProductionState = ipitem.ProductionState;
                    if (inProdItem.ProductionState.ProductionStateId > 3)
                        inProdItem.ProductionState = _context.ProductionStates.Where(ps => ps.ProductionStateId == 2).FirstOrDefault();
                    inProdItem.SourceType = ipitem.SourceType;
                    inProdItem.Ware = ipitem.Ware;
                    inProdItem.WareQuantity = ipitem.WareQuantity;
                    inProdItem.WareLength = ipitem.WareLength;
                    inProdItem.WareUnit = ipitem.WareUnit;
                    inProdItem.StartDate = newOrder.OrderDate;
                    inProdItem.BookLevelTree = ipitem.BookLevelTree;
                    inProdItem.BookOrderTree = ipitem.BookOrderTree;


                    InProductionRW existingInProdRW = _context.InProductionRWs.Where(rw => rw.InProductionId == ipitem.InProductionId).FirstOrDefault();
                    InProductionRW inProdRWItem = new InProductionRW();

                    inProdRWItem.InProduction = inProdItem;
                    inProdRWItem.Ware = ipitem.Ware;
                    //ware data in prodRW can be changed in bandsaw component so copy them from existing inProduction(not inProductionRW) 
                    inProdRWItem.WareQuantity = ipitem.WareQuantity;
                    inProdRWItem.WareLength = ipitem.WareLength;
                    inProdRWItem.WareUnit = ipitem.WareUnit;
                    inProdRWItem.ToIssue = existingInProdRW.ToIssue*newOrderQty/oldOrderQty;
                    inProdRWItem.PlannedToCut = 0;
                    inProdRWItem.Issued = 0;
                    inProdRWItem.TotalToIssue = existingInProdRW.TotalToIssue;
                    inProdRWItem.IsAdditionallyPurchasable = existingInProdRW.IsAdditionallyPurchasable;

                    _context.InProduction.Add(inProdItem);
                    _context.InProductionRWs.Add(inProdRWItem);


                }

            }

            #endregion

            _context.SaveChanges();

            return Ok();

        }


        [HttpPost("copyorderpart/{existingOrderId}")]
        public IActionResult CopyOrderPart(int existingOrderId, [FromBody] List<OrderBookDto> newOrderBooksDto)
        {

            Order order = _context.Orders.Where(o => o.OrderId == existingOrderId).FirstOrDefault();
            int oldOrderQty = _context.OrderBooks.Include(ob => ob.Order).Where(o => o.OrderBookId == newOrderBooksDto[0].OrderBookId).Select(o=>o.Order.PlannedQty).FirstOrDefault();
            int newOrderQty = order.PlannedQty;
            #region creating new orderBooks in DB
            //get existing orderbooks
            List<OrderBook> newOrderBooks = _mapper.Map<List<OrderBookDto>, List<OrderBook>>(newOrderBooksDto);
            List<OrderBook> newOrderBooksinDB = new List<OrderBook>();

            //checking if it's not empty
            if (newOrderBooks.Count == 0)
                BadRequest("No OrderBooks for this \"existingOrderId\"");

            //copy new OrderBooks to order
            newOrderBooks.ForEach(obitem => {
                OrderBook temp = new OrderBook();
                OrderBook existingSuborder = _context.OrderBooks.Include(ob=>ob.Book).Where(ob => ob.OrderBookId == obitem.OrderBookId).SingleOrDefault();

                temp.Order = order;
                temp.AddedDate = obitem.AddedDate;
                temp.Number = obitem.Number;
                temp.Book = existingSuborder.Book;
                temp.PlannedQty = obitem.PlannedQty;
                temp.Comment = obitem.Comment;
                temp.ProductionState = obitem.ProductionState;
                temp.PlasmaInList = obitem.PlasmaInList;
                temp.PlasmaOutList = obitem.PlasmaOutList;
                temp.PurchaseList = obitem.PurchaseList;
                temp.WareList = obitem.WareList;
                newOrderBooksinDB.Add(temp);
                _context.OrderBooks.Add(temp);

            });

            #endregion

            #region create new inProduction in DB

            foreach (OrderBook obitem in newOrderBooks)
            {

                //corresponding new orderBook
                OrderBook OrderBookItem = newOrderBooksinDB.Find(n => n.Book.BookId == obitem.Book.BookId);

                List<InProduction> existingInProduction = _context.InProduction
                    .Include(ip => ip.Component)
                    .Include(ip => ip.ProductionState)
                    .Include(ip => ip.Ware)
                    .Include(ip => ip.ProductionSocket)
                    .Where(ip => ip.OrderBookId == obitem.OrderBookId)
                    .OrderBy(ip => ip.InProductionId).ToList();


                //create new inProduction and inProductionRW
                foreach (InProduction ipitem in existingInProduction)
                {
                    InProduction inProdItem = new InProduction();

                    inProdItem.OrderBook = OrderBookItem;
                    inProdItem.Component = ipitem.Component;
                    inProdItem.PlannedQty = ipitem.PlannedQty*newOrderQty/oldOrderQty;
                    inProdItem.BookQty = ipitem.BookQty;
                    inProdItem.ProductionSocket = ipitem.ProductionSocket;
                    inProdItem.ProductionState = ipitem.ProductionState;
                    if (inProdItem.ProductionState.ProductionStateId > 3)
                        inProdItem.ProductionState = _context.ProductionStates.Where(ps => ps.ProductionStateId == 2).FirstOrDefault();
                    inProdItem.SourceType = ipitem.SourceType;
                    inProdItem.Ware = ipitem.Ware;
                    inProdItem.WareQuantity = ipitem.WareQuantity;
                    inProdItem.WareLength = ipitem.WareLength;
                    inProdItem.WareUnit = ipitem.WareUnit;
                    inProdItem.StartDate = order.OrderDate;
                    inProdItem.BookLevelTree = ipitem.BookLevelTree;
                    inProdItem.BookOrderTree = ipitem.BookOrderTree;


                    InProductionRW existingInProdRW = _context.InProductionRWs.Where(rw => rw.InProductionId == ipitem.InProductionId).FirstOrDefault();
                    InProductionRW inProdRWItem = new InProductionRW();

                    inProdRWItem.InProduction = inProdItem;
                    inProdRWItem.Ware = ipitem.Ware;
                    //ware data in prodRW can be changed in bandsaw component so copy them from existing inProduction(not inProductionRW) 
                    inProdRWItem.WareQuantity = ipitem.WareQuantity;
                    inProdRWItem.WareLength = ipitem.WareLength;
                    inProdRWItem.WareUnit = ipitem.WareUnit;
                    inProdRWItem.ToIssue = existingInProdRW.ToIssue*newOrderQty/oldOrderQty;
                    inProdRWItem.PlannedToCut = 0;
                    inProdRWItem.Issued = 0;
                    inProdRWItem.TotalToIssue = existingInProdRW.TotalToIssue;
                    inProdRWItem.IsAdditionallyPurchasable = existingInProdRW.IsAdditionallyPurchasable;

                    _context.InProduction.Add(inProdItem);
                    _context.InProductionRWs.Add(inProdRWItem);


                }

            }

            #endregion

            _context.SaveChanges();

            return Ok();
        }

        #endregion

        #region help functions

        string orderBookNumberGen(string orderName, int index)
        {
            string orderBookNumber ="";

            if (index < 10)
                orderBookNumber = orderName + "_0" + index.ToString();
            else
                orderBookNumber = orderName + "_" + index.ToString();

            return orderBookNumber;
        }

        #endregion

    }
}