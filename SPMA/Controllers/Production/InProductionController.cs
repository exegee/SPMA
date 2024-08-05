using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPMA.Controllers.Warehouse;
using SPMA.Data;
using SPMA.Dtos.Production;
using SPMA.Dtos.Warehouse;
using SPMA.Models.Optima;
using SPMA.Models.Orders;
using SPMA.Models.Production;
using SPMA.Models.Warehouse;
using static SPMA.Core.Enums;

namespace SPMA.Controllers.Production
{
    [Route("api/[controller]")]
    [ApiController]
    public class InProductionController : ControllerBase
    {
        #region Properties
        private ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly WarehouseController _warehouseController;
        private readonly WarehouseItemController _warehouseItemController;
        #endregion

        #region Constructor
        public InProductionController(ApplicationDbContext dbContext,
            IMapper mapper, WarehouseController warehouseController, 
            WarehouseItemController warehouseItemController)
        {
            _context = dbContext;
            _mapper = mapper;
            _warehouseController = warehouseController;
            _warehouseItemController = warehouseItemController;
        }
        #endregion

        #region Http Action Requests

        //GET /api/inproduction
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            InProductionDto inProductionDto = _mapper.Map<InProduction, InProductionDto>(_context.InProduction.Where(ip => ip.InProductionId == id).SingleOrDefault());

            return Ok(inProductionDto);
        }

        /// <summary>
        /// Adds book to production and fills RW table
        /// </summary>
        /// <param name="bookComponentDto"></param>
        /// <returns></returns>
        //POST /api/inproduction
        [HttpPost("book")]
        public IActionResult Add(BookComponentDto bookComponentDto)
        {
            var wareListCount = 0;
            var purchaseListCount = 0;
            var plasmaInListCount = 0;
            var plasmaOutListCount = 0;

            // Check if BookComponent is null
            if (bookComponentDto == null)
                return Content("bookComponentDto is null.");

            // Get main order
            Order orderInDb = _context.Orders.Where(o => o.OrderId == bookComponentDto.MainOrder.OrderId).SingleOrDefault();

            // Get SubOrder
            OrderBook orderBookInDb = _context.OrderBooks
                .Include(ob => ob.Order)
                .Where(ob => ob.Number == bookComponentDto.OrderBook.Number).Single();

            List<InProduction> addedInProductionPurchaseItems = new List<InProduction>();

            var componentQuantity = 0;
            // Add each item to InProduction table
            foreach (ComponentDto componentDto in bookComponentDto.Components)
            {
                componentQuantity = 0;
                // Get component from database
                Component componentInDb = _context.Components
                    .Include(c => c.Ware)
                    .Where(c => c.Number == componentDto.Number).SingleOrDefault();

                // Set production state for all components in list
                // Idle - not in production
                // Awaiting - in production
                ProductionState productionStateInDb = new ProductionState();
                productionStateInDb = _context.ProductionStates
                                        .Where(ps => ps.ProductionStateCode == ((componentDto.ToProduction == true)
                                        ? (sbyte)ProductionStateEnum.Awaiting : (sbyte)ProductionStateEnum.Idle)).SingleOrDefault();

                // Set quantities for InProduction table
                // Each book standard component (xxx.xx.xxx) is added separately to InProduction and InProductionRWs tables with additional information about it's total amount.
                // Hovewer, purchase component is added only once to those tables and its amount is summed
                if (componentDto.LastSourceType != 5)
                {
                    //Standard component
                    componentQuantity = orderBookInDb.Order.PlannedQty * orderBookInDb.PlannedQty * componentDto.Quantity;
                }
                else
                {
                    //Purchase item
                    componentQuantity = orderBookInDb.Order.PlannedQty * orderBookInDb.PlannedQty * componentDto.SumQuantity;

                    if (addedInProductionPurchaseItems.Any(a => a.Component.Number == componentDto.Number))
                    {
                        //addedInProductionPurchaseItems.Find(x => x.Component.Number == componentDto.Number).PlannedQty += componentDto.Quantity;
                        continue;
                    }
                }

                // Set date depending on production state
                DateTime? inProductionStartTime = new DateTime();
                if (productionStateInDb.ProductionStateCode == (sbyte)ProductionStateEnum.Awaiting)
                {
                    inProductionStartTime = DateTime.Now;
                }
                else
                {
                    inProductionStartTime = null;
                }


                //Set InProduction record
                InProduction inProduction = new InProduction
                {
                    OrderBook = orderBookInDb,
                    Component = componentInDb,
                    PlannedQty = componentQuantity,
                    BookQty = componentDto.SinglePieceQty,
                    FinishedQty = 0,
                    ProductionSocket = null,
                    ProductionState = productionStateInDb,
                    Ware = componentInDb.Ware,
                    WareLength = (componentDto.WareLength == null ? componentInDb.WareLength : componentDto.WareLength),
                    // Always equals to 1
                    WareQuantity = 1,
                    WareUnit = componentInDb.WareUnit,
                    StartDate = inProductionStartTime,
                    SourceType = (componentDto.LastSourceType == null ? componentInDb.LastSourceType : componentDto.LastSourceType),
                    Technology = (componentDto.LastTechnology == null ? componentInDb.LastTechnology : componentDto.LastTechnology),
                    BookLevelTree = componentDto.Level,
                    BookOrderTree = componentDto.Order,
                };

                // Calculate duplicates and total amount of components
                int totalAmount = 0;
                foreach (ComponentDto component in bookComponentDto.Components)
                {
                    if (component.Number == componentDto.Number)
                        totalAmount += component.Quantity;
                }

                //Set InProductionRW record
                InProductionRW inProductionRW = new InProductionRW
                {
                    InProduction = inProduction,
                    Issued = 0,
                    Ware = inProduction.Ware,
                    WareLength = inProduction.WareLength,
                    WareQuantity = inProduction.WareQuantity,
                    WareUnit = inProduction.WareUnit,
                    PlannedToCut = 0,
                    IsAdditionallyPurchasable = Convert.ToSByte(componentDto.IsAdditionallyPurchasable)
                };

                // Checking source type elements count - used for printing boms
                if (inProduction.Ware != null && componentDto.ToProduction)
                {
                    switch (componentDto.LastSourceType)
                    {
                        case (sbyte)LastSourceType.Standard:
                            if (!componentDto.IsAdditionallyPurchasable)
                            {
                                wareListCount++;
                            }
                            else
                            {
                                purchaseListCount++;
                            }
                            break;
                        case (sbyte)LastSourceType.Purchase:
                            purchaseListCount++;
                            break;
                        case (sbyte)LastSourceType.PlasmaIn:
                            plasmaInListCount++;
                            break;
                        case (sbyte)LastSourceType.PlasmaOutWithoutEntrustedMaterial:
                            plasmaOutListCount++;
                            break;
                        case (sbyte)LastSourceType.PlasmaWithEntrustedMaterial:
                            plasmaOutListCount++;
                            break;
                        default:
                            break;
                    }
                }
                else if (inProduction.Ware == null && componentDto.ToProduction)
                {
                    switch (componentDto.LastSourceType)
                    {
                        case (sbyte)LastSourceType.PlasmaOutWithoutEntrustedMaterial:
                            plasmaOutListCount++;
                            break;
                        case (sbyte)LastSourceType.PlasmaWithEntrustedMaterial:
                            plasmaOutListCount++;
                            break;
                    }
                }

                // Set toIssue property in InProductionRW table based on component type ( standard or purchase )
                if (componentDto.LastSourceType == (sbyte)LastSourceType.Purchase)
                {
                    inProductionRW.ToIssue = inProduction.PlannedQty;
                }
                else
                {
                    if (componentDto.WareQuantity == null)
                    {
                        inProductionRW.ToIssue = inProduction.PlannedQty;
                    }
                    else
                    {
                        inProductionRW.ToIssue = inProduction.PlannedQty * (int)componentDto.WareQuantity;
                    }

                }

                // Set total amount of the component to InProductionRW table
                inProductionRW.TotalToIssue = totalAmount * orderBookInDb.Order.PlannedQty * orderBookInDb.PlannedQty;

                _context.InProductionRWs.Add(inProductionRW);
                _context.InProduction.Add(inProduction);
                addedInProductionPurchaseItems.Add(inProduction);


                //if material has reservation
                WarehouseItem reservedItem;
                ReservedItem reservation;
                if (componentDto.reservedItemId != -1)
                {
                    reservedItem = _context.WarehouseItems.Where(w => w.WarehouseItemId == componentDto.reservedItemId).FirstOrDefault();
                    reservedItem.ReservedQty += componentDto.reservedQty;
                    reservation = new ReservedItem
                    {
                        InProductionRW = inProductionRW,
                        WarehouseItem = reservedItem,
                        Quantity = componentDto.reservedQty
                    };

                    _context.ReservedItems.Add(reservation);
                    inProduction.ProductionState = _context.ProductionStates.Where(ps => ps.ProductionStateCode == 120).FirstOrDefault();
                }

                //adding surplus items to warehouseItems table if there are any
                if (componentDto.Quantity > componentDto.SinglePieceQty)
                {
                    int quantytyWithNoExcess = (componentDto.SinglePieceQty * orderBookInDb.Order.PlannedQty * orderBookInDb.PlannedQty);

                    WarehouseItem newSurplusItem = new WarehouseItem
                    {
                        Component = componentInDb,
                        Ware = componentInDb.Ware,
                        ComponentQty = inProduction.PlannedQty - quantytyWithNoExcess,
                        WareQty = componentInDb.WareLength,
                        WareQtySum = (inProduction.PlannedQty - quantytyWithNoExcess) * componentInDb.WareLength,
                        AddedDate = DateTime.UtcNow,
                        Comment = "added automatically",
                        State = 0,
                        ParentRWItem = inProductionRW
                    };
                    if(newSurplusItem.Ware != null)
                    {
                        _warehouseItemController.AddItem(_mapper.Map<WarehouseItem, WarehouseItemDto>(newSurplusItem));
                    }
                   
                }

            }

            if (wareListCount > 0)
            {
                orderBookInDb.WareList = 1;
            }

            if (purchaseListCount > 0)
            {
                orderBookInDb.PurchaseList = 1;
            }

            if (plasmaInListCount > 0)
            {
                orderBookInDb.PlasmaInList = 1;
            }

            if (plasmaOutListCount > 0)
            {
                orderBookInDb.PlasmaOutList = 1;
            }

            //Save changes to database
            _context.SaveChanges();

            return Ok();
        }

        [HttpGet("rw/{id}")]
        public IActionResult GetInProductionRWByInProductionId(int id)
        {
            var inProductionRW = _context.InProductionRWs.Where(rw => rw.InProductionId == id).SingleOrDefault();

            return Ok(inProductionRW);
        }

        [HttpGet("saw/{filter}")]
        public IActionResult GetSawComponents(int filter)
        {

            int sawNumber = _context.InProduction
                .Where(sc => sc.SourceType == filter)
                .Count();
            return Ok(sawNumber);
        }

        //gives the list of drawing with the same material from the given book
        //currently not used (this operation is happenning in frontend logic)
        [HttpGet ("saw/samematerialinbook")]
        public IActionResult GetComponentsWithSameMaterialFromBook(int bookId,int wareId)
        {
            var components = _context.InProductionRWs
                .Include(o => o.InProduction)
                .ThenInclude(o=>o.Component)
                .ThenInclude(o=>o.Ware)
                .Where(o=>o.InProduction.OrderBookId == bookId & o.InProduction.Component.Ware.WareId == wareId)
                .ToList();

            List<string> drawingNumbers = new List<string>();
            foreach(var item in components)
            {
                drawingNumbers.Add(item.InProduction.Component.Number);
            }

            return Ok(drawingNumbers);
        }

        //gives the list of books and quantity of those books where given material name shows
        [HttpGet("saw/samematerialinorder")]
        public IActionResult GetComponentsWithSameMaterialFromOrder(int wareId, int sourceType)
        {

            var books = _context.InProductionRWs
                .Include(rw=>rw.InProduction)
                .Include(rw=>rw.InProduction.ProductionState)
                .Include(rw => rw.InProduction.OrderBook.Book)
                .Include(rw => rw.InProduction.OrderBook.Order)
                .Where(rw => rw.Ware.WareId == wareId &&
                             rw.InProduction.SourceType == sourceType &&
                            (rw.InProduction.ProductionState.ProductionStateCode >= 2 ||
                             rw.InProduction.ProductionState.ProductionStateCode <= 4) &&
                             rw.InProduction.Component.ComponentType == 0 &&
                             rw.Ware != null &&
                             rw.IsAdditionallyPurchasable == 0 &&
                             rw.InProduction.OrderBook.Order.State != 10)
                .Select(s => new
                {
                    officeNumber = s.InProduction.OrderBook.Book.OfficeNumber,
                    orderNumber = s.InProduction.OrderBook.Order.Number,
                    orderId = s.InProduction.OrderBook.OrderId,
                    subOrderNumber = s.InProduction.OrderBook.Number,
                    subOrderId = s.InProduction.OrderBookId
                })
                .OrderBy(s=>s.orderId)
                .ToList();

            //List<string> bookOfficeNames = new List<string>();
            //foreach (var item in books)
            //{
            //    bookOfficeNames.Add(item.InProduction.OrderBook.Book.OfficeNumber);
            //}

            return Ok(books);
        }

        //update production status
        [HttpPatch("statechange")]
        public IActionResult ChangeProductionStatus(sbyte stateCode, int inProductionId)
        {
            //get state Id with production code == stateCode
            ProductionState state = _context.ProductionStates
                .Where(s=>s.ProductionStateCode==stateCode)
                .SingleOrDefault();

            InProduction inProduction = _context.InProduction
                .Where(ip => ip.InProductionId == inProductionId)
                .SingleOrDefault();

            if (stateCode == 4)
            {
                //check if component is in WarehouseItems table
                int InProductionRWId = _context.InProductionRWs
                    .Where(rw => rw.InProductionId == inProduction.InProductionId)
                    .Select(rw => rw.InProductionRWId).SingleOrDefault();

                WarehouseItem warehouseItemDB = _context.WarehouseItems
                    .Where(w => w.ParentRWItemId == InProductionRWId)
                    .FirstOrDefault();

                if (warehouseItemDB != null)
                {
                    warehouseItemDB.State = 1;
                }
            }
            inProduction.ProductionStateId = state.ProductionStateId;
            _context.SaveChanges();

            return Ok();
        }

        //update wareLength 
        [HttpPatch("overflowlengthchange")]
        public IActionResult ChangeMaterialLength(int inProductionRWId,decimal newLength)
        {
            if (!(inProductionRWId > 0) && (newLength > 0))
            {
                return BadRequest("Wrong input data");
            }

            InProductionRW rwItem = _context.InProductionRWs
                .Where(ip => ip.InProductionRWId == inProductionRWId
                ).FirstOrDefault();

            rwItem.WareLength = newLength;

            _context.SaveChanges();

            return Ok();
        }

        //update toIssue prop
        [HttpPatch("quantitychange")]
        public IActionResult ChangeQuantity(int inProductionRWId, int newQty)
        {
            if (!(inProductionRWId > 0) && (newQty > 0))
            {
                return BadRequest("Wrong input data");
            }

            InProductionRW rwItem = _context.InProductionRWs
                .Where(ip => ip.InProductionRWId == inProductionRWId
                ).FirstOrDefault();

            int diffrence;
            diffrence = newQty - rwItem.TotalToIssue;
            rwItem.ToIssue = newQty;
            rwItem.TotalToIssue += diffrence;

            _context.SaveChanges();

            return Ok();
        }

        //update ware 
        [HttpPatch("changeware")]
        public IActionResult ChangeWare(int RWid, WareDto wareDto)
        {
            Ware ware = _context.Wares
                .Where(w => w.Code == wareDto.Code)
                .SingleOrDefault();

            if (ware == null)
            {
                _warehouseController.AddWare(wareDto);
            }

            Ware wareInDB = _context.Wares
                .Where(w => w.Code == wareDto.Code)
                .SingleOrDefault();

            InProductionRW inProdRW = _context.InProductionRWs
                .Where(iprw => iprw.InProductionRWId == RWid)
                .SingleOrDefault();

            inProdRW.Ware = wareInDB;

            _context.SaveChanges();

            return Ok(wareInDB);
        }



        #endregion

        #region
        #endregion


    }
}