using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPMA.Data;
using SPMA.Dtos.Orders;
using SPMA.Dtos.Warehouse;
using SPMA.Models.Orders;
using SPMA.Models.Production;
using SPMA.Models.Warehouse;
using System.Collections.Generic;
using System.Linq;
using static SPMA.Core.Enums;

namespace SPMA.Controllers.SubOrders
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubOrderController : ControllerBase
    {
        #region Properties
        private ApplicationDbContext _context;
        private readonly IMapper _mapper;
        #endregion

        #region Constructor
        public SubOrderController(ApplicationDbContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }
        #endregion

        #region Http Action Requests

        /// <summary>
        /// Get suborder items
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult GetSubOrderById(int id)
        {

            var productionStatesInDb = _context.ProductionStates.ToList();

            List<InProductionRW> inProductionRWs = _context.InProductionRWs
                            .Include(ip => ip.Ware)
                            .Include(ip => ip.InProduction)
                            .Include(ip => ip.InProduction.Component)
                            .Include(ip => ip.InProduction.ProductionState)
                            .Where(ip => ip.InProduction.OrderBookId == id )//&& ip.InProduction.SourceType==1)
                            .ToList();

            List<SubOrderDto> subOrder = _mapper.Map<List<InProductionRW>,List<SubOrderDto>>(inProductionRWs);

            return Ok(subOrder);
        }



        /// <summary>
        /// Get suborder items for band saw socket 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("bandsaw/{id}")]
        public IActionResult GetSubOrderbyIdForCut(int id, int sourceType)
        {

            var productionStatesInDb = _context.ProductionStates.ToList();

            List<InProductionRW> inProductionRWs = _context.InProductionRWs
                            .Include(ip => ip.Ware)
                            .Include(ip => ip.InProduction)
                            .Include(ip => ip.InProduction.Component)
                            .Include(ip => ip.InProduction.ProductionState)
                            .Where(ip => 
                                ip.InProduction.OrderBookId == id &&
                                ip.InProduction.SourceType == sourceType &&
                               ((ip.InProduction.ProductionState.ProductionStateCode >= 2 &&
                                ip.InProduction.ProductionState.ProductionStateCode <= 4) ||
                                ip.InProduction.ProductionState.ProductionStateCode == 120) &&
                                ip.InProduction.Component.ComponentType ==0 &&
                                ip.Ware !=  null && 
                                ip.IsAdditionallyPurchasable == 0)
                            .OrderBy(ip=>ip.InProduction.BookOrderTree)
                            .ToList();

            List<SubOrderDto> subOrder = _mapper.Map<List<InProductionRW>, List<SubOrderDto>>(inProductionRWs);

            return Ok(subOrder);
        }


        [HttpGet("getcomponentindex")]
        public IActionResult getComponentIndex(int id, int wareId, int sourceType)
        {

            List<InProductionRW> inProductionRWs = _context.InProductionRWs
                            .Include(ip => ip.InProduction)
                            .Include(ip=>ip.Ware)
                            .Include(ip => ip.InProduction.Component)
                            .Include(ip => ip.InProduction.ProductionState)
                            .Where(ip =>
                                ip.InProduction.OrderBookId == id &&
                                ip.InProduction.SourceType == sourceType &&
                               (ip.InProduction.ProductionState.ProductionStateCode >= 2 ||
                                ip.InProduction.ProductionState.ProductionStateCode <= 4) &&
                                ip.InProduction.Component.ComponentType == 0 &&
                                ip.Ware != null &&
                                ip.IsAdditionallyPurchasable == 0)
                            .OrderBy(ip => ip.InProduction.BookOrderTree)
                            .ToList();

            int index = inProductionRWs.FindIndex(ip => ip.Ware?.WareId == wareId);

            return Ok(index);
        }


        /// <summary>
        /// Get suborder additional info - OrderBooks table
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/info")]
        public IActionResult GetSubOrderByIdAdditionalInfo(int? id)
        {
            if(id == null)
            {
                return BadRequest();
            }
            else
            {
                OrderBook subOrderInfo = _context.OrderBooks
                                           .Where(ob => ob.OrderBookId == id)
                                           .Include(ob => ob.Book)
                                           .Include(ob=>ob.Order)
                                           .SingleOrDefault();

                OrderBookDto subOrderInfoDto = _mapper.Map<OrderBook, OrderBookDto>(subOrderInfo);
                return Ok(subOrderInfoDto);
            }
        }

        [HttpGet("ware")]
        public IActionResult GetSubOrderItemWare(int? id)
        {
            if(id == null)
            {
                return BadRequest();
            }
            else
            {
                Ware ware = _context.InProduction.Where(ip => ip.InProductionId == id)
                                                 .Select(ip => ip.Ware)
                                                 .SingleOrDefault();

                WareDto wareDto = _mapper.Map<Ware, WareDto>(ware);
                return Ok(wareDto);
            }
        }

        [HttpPatch]
        public IActionResult UpdateSubOrderItem([FromBody]SubOrderDto[] subOrderDto)
        {
            if(subOrderDto == null || subOrderDto.Length == 0)
            {
                return BadRequest(new object[] { new { Error = "SubOrderItems is null or 0." } });
            }
            else
            {
                var newBookMultiplier = 1;
                var oldBookMultiplier = 1;

                var subOrderInfo = _context.OrderBooks.Where(ob => ob.OrderBookId == subOrderDto[0].InProduction.OrderBookId).SingleOrDefault();


                if (subOrderDto[0].IsBookQtyChanged)
                {
                    
                    if(subOrderDto[0].BookQty > 0)
                    {
                        newBookMultiplier = subOrderDto[0].BookQty;
                        oldBookMultiplier = subOrderInfo.PlannedQty;
                        subOrderInfo.PlannedQty = newBookMultiplier;
                    }
                }
                


                //Get suborder from database
                foreach (SubOrderDto subOrderItemDto in subOrderDto)
                {
                    InProductionRW subOrderInDb = _context.InProductionRWs
                                           .Where(ip => ip.InProductionRWId == subOrderItemDto.InProductionRWId)
                                           .Include(ip => ip.Ware)
                                           .Include(ip => ip.InProduction)
                                           .ThenInclude(ip => ip.ProductionState)
                                           .SingleOrDefault();

                    //Update ware in InProductionRWs and InProduction tables
                    if (subOrderItemDto.Ware != null)
                    {
                        //If ware is not null grab it from database ( Wares table ) and assing it to InProductionRWs and InProduction tables
                        Ware wareInDb = _context.Wares.Where(w => w.Code == subOrderItemDto.Ware.Code).SingleOrDefault();

                        //if that ware exists only in optimaWare copy it to ware table
                        if(wareInDb == null)
                        {
                            wareInDb=_mapper.Map<WareDto, Ware>(subOrderItemDto.Ware);
                            _context.Wares.Add(wareInDb);
                        }

                        subOrderInDb.Ware = wareInDb;
                        subOrderInDb.WareLength = subOrderItemDto.WareLength;
                        subOrderInDb.WareQuantity = subOrderItemDto.WareQuantity;
                        subOrderInDb.WareUnit = subOrderItemDto.WareUnit;                        
                        subOrderInDb.InProduction.Ware = wareInDb;
                        subOrderInDb.InProduction.WareLength = subOrderItemDto.WareLength;
                        subOrderInDb.InProduction.WareQuantity = subOrderItemDto.WareQuantity;
                        subOrderInDb.InProduction.WareUnit = subOrderItemDto.WareUnit;
                    }
                    else
                    {
                        // Else if null clear ware from InProductionRWs and InProduction
                        subOrderInDb.Ware = null;
                        subOrderInDb.WareLength = null;
                        subOrderInDb.WareQuantity = 1;
                        subOrderInDb.WareUnit = null;
                        subOrderInDb.InProduction.Ware = null;
                        subOrderInDb.InProduction.WareLength = null;
                        subOrderInDb.InProduction.WareQuantity = 1;
                        subOrderInDb.InProduction.WareUnit = null;
                    }

                    //Update quantites in InProduction table
                    subOrderInDb.InProduction.PlannedQty = subOrderItemDto.InProduction.PlannedQty * newBookMultiplier / oldBookMultiplier;

                    //Update quantities in InProductionRW table
                    subOrderInDb.ToIssue = subOrderItemDto.InProduction.PlannedQty * newBookMultiplier / oldBookMultiplier;
                    subOrderInDb.TotalToIssue = subOrderItemDto.TotalToIssue * newBookMultiplier / oldBookMultiplier;

                    //Update production state
                    ProductionStateEnum newProductionState = subOrderItemDto.IsInProduction == true ? ProductionStateEnum.Awaiting : ProductionStateEnum.Idle;

                    ProductionState productionStateInDB = _context.ProductionStates.Where(ps => ps.ProductionStateCode == (sbyte)newProductionState).FirstOrDefault();

                    //if state in DB is the same or is not cut yet than update
                    if(productionStateInDB.ProductionStateId != subOrderInDb.InProduction.ProductionStateId && productionStateInDB.ProductionStateCode < 3)
                    {
                        subOrderInDb.InProduction.ProductionState = productionStateInDB;
                    }

                    //Update SourceType
                    subOrderInDb.InProduction.SourceType = subOrderItemDto.InProduction.SourceType;

                    //update isAdditionallyPurchasable
                    subOrderInDb.IsAdditionallyPurchasable = subOrderItemDto.IsAdditionallyPurchasable;

                }

                //update status of plasmaIn plasmaOut and purchase Lists in OrderBooks Table
                List<InProduction> InProduction_DB = _context.InProduction.Where(ip => ip.OrderBookId == subOrderDto[0].InProduction.OrderBookId).ToList();
                bool plasmaInOccurred = false;
                bool plasmaOutOccurred = false;
                bool purchaseElementOccured = false;

                foreach (InProduction item in InProduction_DB)
                    {

                    if(item.ProductionStateId != 1)
                    {
                        if (item.SourceType == 2)
                            plasmaInOccurred = true;
                        if (item.SourceType == 3 || item.SourceType == 4)
                            plasmaOutOccurred = true;
                        if (item.SourceType == 5)
                            purchaseElementOccured = true;
                    }
                }



                subOrderInfo.PlasmaInList =(byte) (plasmaInOccurred ?  1 : 0);
                subOrderInfo.PlasmaOutList = (byte)(plasmaOutOccurred ? 1 : 0);
                subOrderInfo.PurchaseList = (byte)(purchaseElementOccured ? 1 : 0);


                // Save changes to database
                _context.SaveChanges();
                return Ok(true);
            }
        }

        [HttpGet("getcutstatuses")]
        public IActionResult GetCutStatus(int orderId)
        {

            int sawStatusPercentage = 0;
            int plasmaStatusPercentage = 0;
            int[] orderBookIds = _context.OrderBooks
                .Where(ob => ob.OrderId == orderId)
                .OrderBy(ob=>ob.Number)
                .Select(ob => ob.OrderBookId)
                .ToArray();
            List<CutStatuses> doneStatusPercentage = new List<CutStatuses>();

            foreach (int orderBookId in orderBookIds)
            {
                var productionStatusesSaw = _context.InProduction
                    .Include(ip => ip.Component)
                    .Include(ip => ip.ProductionState)
                    .Where(ip => ip.OrderBookId == orderBookId &&
                    ip.SourceType == 0 &&
                    ip.Component.ComponentType == 0 &&
                    (ip.ProductionState.ProductionStateCode >= 2 &&
                    ip.ProductionState.ProductionStateCode <= 4) &&
                    ip.Ware != null)
                    .Select(ip => ip.ProductionState.ProductionStateCode)
                    .ToList();

                var productionStatusesPlasma = _context.InProduction
                    .Include(ip => ip.Component)
                    .Include(ip => ip.ProductionState)
                    .Where(ip => ip.OrderBookId == orderBookId &&
                    ip.SourceType == 2 &&
                    ip.Component.ComponentType == 0 &&
                    (ip.ProductionState.ProductionStateCode >= 2 &&
                    ip.ProductionState.ProductionStateCode <= 4) &&
                    ip.Ware != null)
                    .Select(ip => ip.ProductionState.ProductionStateCode)
                    .ToList();

                int totalAmount = productionStatusesSaw.Count;
                int numberOfAlreadyCut = 0;
                foreach (sbyte item in productionStatusesSaw)
                {
                    if (item == 4)
                    {
                        numberOfAlreadyCut++;
                    }
                }

                if (totalAmount != 0)
                {
                   sawStatusPercentage= 100 * numberOfAlreadyCut / totalAmount;
                }
                else
                {
                    sawStatusPercentage= -1;
                }


                totalAmount = productionStatusesPlasma.Count;
                numberOfAlreadyCut = 0;
                foreach (sbyte item in productionStatusesPlasma)
                {
                    if (item == 4)
                    {
                        numberOfAlreadyCut++;
                    }
                }

                if (totalAmount != 0)
                {
                    plasmaStatusPercentage = 100 * numberOfAlreadyCut / totalAmount;
                }
                else
                {
                    plasmaStatusPercentage = -1;
                }

                doneStatusPercentage.Add(new CutStatuses(sawStatusPercentage, plasmaStatusPercentage));

            }

            return Ok(doneStatusPercentage);
        }


        [HttpGet("getcutstatus")]
        public IActionResult GetCutStatuses(int orderBookId)
        {

            int sawStatusPercentage = 0;
            int plasmaStatusPercentage = 0;
            CutStatuses doneStatusPercentage;


            
            var productionStatusesSaw = _context.InProduction
                .Include(ip => ip.Component)
                .Include(ip => ip.ProductionState)
                .Where(ip => ip.OrderBookId == orderBookId &&
                ip.SourceType == 0 &&
                ip.Component.ComponentType == 0 &&
                (ip.ProductionState.ProductionStateCode >= 2 &&
                ip.ProductionState.ProductionStateCode <= 4) &&
                ip.Ware != null)
                .Select(ip => ip.ProductionState.ProductionStateCode)
                .ToList();

            var productionStatusesPlasma = _context.InProduction
                .Include(ip => ip.Component)
                .Include(ip => ip.ProductionState)
                .Where(ip => ip.OrderBookId == orderBookId &&
                ip.SourceType == 2 &&
                ip.Component.ComponentType == 0 &&
                (ip.ProductionState.ProductionStateCode >= 2 &&
                ip.ProductionState.ProductionStateCode <= 4) &&
                ip.Ware != null)
                .Select(ip => ip.ProductionState.ProductionStateCode)
                .ToList();

            int totalAmount = productionStatusesSaw.Count;
            int numberOfAlreadyCut = 0;
            foreach (sbyte item in productionStatusesSaw)
            {
                if (item == 4)
                {
                    numberOfAlreadyCut++;
                }
            }

            if (totalAmount != 0)
            {
                sawStatusPercentage = 100 * numberOfAlreadyCut / totalAmount;
            }
            else
            {
                sawStatusPercentage = -1;
            }


            totalAmount = productionStatusesPlasma.Count;
            numberOfAlreadyCut = 0;
            foreach (sbyte item in productionStatusesPlasma)
            {
                if (item == 4)
                {
                    numberOfAlreadyCut++;
                }
            }

            if (totalAmount != 0)
            {
                plasmaStatusPercentage = 100 * numberOfAlreadyCut / totalAmount;
            }
            else
            {
                plasmaStatusPercentage = -1;
            }

            doneStatusPercentage = new CutStatuses ( sawStatusPercentage, plasmaStatusPercentage);

            

            return Ok(doneStatusPercentage);
        }

        #endregion

        #region Helpers

        public struct CutStatuses
        {
          public int sawStatusPercentage { get; set; }
          public int plasmaStatusPercentage { get; set; }

            public CutStatuses(int x, int y)
            {
                sawStatusPercentage = x;
                plasmaStatusPercentage = y;
            }
        }

        #endregion

    }
}