using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPMA.Controllers.Optima;
using SPMA.Controllers.Production;
using SPMA.Core.Tools;
using SPMA.Data;
using SPMA.Models.Orders;
using SPMA.Models.Production;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using static SPMA.Core.Enums;

namespace SPMA.Controllers.Utility
{
    [Route("api/[controller]")]
    [ApiController]
    public class XmlController : ControllerBase
    {
        #region Properties
        private IHostingEnvironment _hostingEnvironment;
        private readonly BookController _bookController;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly OptimaController _optimaController;
        #endregion

        public XmlController(BookController bookController, IHostingEnvironment hostingEnvironment, ApplicationDbContext context,
            IMapper mapper, OptimaController optimaController)
        {
            _bookController = bookController;
            _hostingEnvironment = hostingEnvironment;
            _context = context;
            _mapper = mapper;
            _optimaController = optimaController;
        }
        
        [HttpGet("book/getwares/{id}")]
        public IActionResult GetWareList(int id, string orderNumber, string subOrderNumber, sbyte listType)
        {
            // Get order
            Order order = _context.Orders.Where(o => o.Number == orderNumber).SingleOrDefault();

            // Get book
            OrderBook subOrder = _context.OrderBooks
                .Include(ob => ob.Book)
                .Where(ob => ob.Number == subOrderNumber).SingleOrDefault();

            //List<InProductionRWDto> inProductionRWListDto;
            List<InProductionXML> inProductionWareList = new List<InProductionXML>();

            if (listType == 0)
            {
                inProductionWareList = _context.InProductionRWs
                    .Include(rw => rw.InProduction)
                    .ThenInclude(ip => ip.Component)
                    .Include(rw => rw.Ware)
                    .Where(rw => rw.InProduction.SourceType == (sbyte)LastSourceType.Standard
                      && rw.Ware != null && rw.InProduction.OrderBookId == subOrder.OrderBookId
                      //&& rw.InProduction.ProductionState.ProductionStateCode == (sbyte)ProductionStateEnum.Awaiting
                      && rw.IsAdditionallyPurchasable == 0)
                    .GroupBy(rw => rw.InProduction.Component.Number).Select(rwd => new InProductionXML
                    {
                        ComponentNumber = rwd.Select(i => i.InProduction.Component.Number).FirstOrDefault(),
                        WareCode = rwd.Select(i => i.Ware.Code).FirstOrDefault(),
                        WareName = rwd.Select(i => i.Ware.Name).FirstOrDefault(),
                        WareUnit = rwd.Select(i => i.WareUnit).FirstOrDefault(),
                        WareLength = rwd.Select(i => i.WareLength).FirstOrDefault(),
                        Issued = rwd.Select(i => i.Issued).FirstOrDefault(),
                        WareQuantity = rwd.Sum(i => i.ToIssue)
                    }).ToList();
            } 
            //Purchase list
            if (listType == 1)
            {
                inProductionWareList = _context.InProductionRWs
                    .Include(rw => rw.InProduction)
                    .ThenInclude(ip => ip.Component)
                    .Include(rw => rw.Ware)
                    .Where(rw => ((rw.InProduction.SourceType == (sbyte)LastSourceType.Purchase)
                      || ((rw.InProduction.SourceType == (sbyte)LastSourceType.Standard
                      || rw.InProduction.SourceType == (sbyte)LastSourceType.PlasmaIn
                      || rw.InProduction.SourceType == (sbyte)LastSourceType.PlasmaOutWithoutEntrustedMaterial
                      || rw.InProduction.SourceType == (sbyte)LastSourceType.PlasmaWithEntrustedMaterial) && rw.IsAdditionallyPurchasable == 1))
                      && rw.Ware != null && rw.InProduction.OrderBookId == subOrder.OrderBookId)
                    .GroupBy(rw => rw.InProduction.Ware.Code).Select(rwd => new InProductionXML
                    {
                        ComponentNumber = rwd.Select(i => i.InProduction.Component.Number).FirstOrDefault(),
                        WareCode = rwd.Select(i => i.Ware.Code).FirstOrDefault(),
                        WareName = rwd.Select(i => i.Ware.Name).FirstOrDefault(),
                        WareUnit = rwd.Select(i => i.WareUnit).FirstOrDefault(),
                        WareLength = rwd.Select(i => i.WareLength).FirstOrDefault(),
                        Issued = rwd.Select(i => i.Issued).FirstOrDefault(),
                        TotalToIssue = rwd.Sum(i => i.ToIssue)
                    }).ToList();
            }
            else if(listType == 2)
            {
                inProductionWareList = _context.InProductionRWs
                    .Include(rw => rw.InProduction)
                    .ThenInclude(ip => ip.Component)
                    .Include(rw => rw.Ware)
                    .Where(rw => rw.InProduction.SourceType == (sbyte)LastSourceType.PlasmaIn
                      && rw.Ware != null && rw.InProduction.OrderBookId == subOrder.OrderBookId
                      //&& rw.InProduction.ProductionState.ProductionStateCode == (sbyte)ProductionStateEnum.Awaiting
                      && rw.IsAdditionallyPurchasable == 0)
                    .Select(item => new InProductionXML
                    {
                        WareName = item.InProduction.Ware.Name,
                        WareCode = item.InProduction.Ware.Code,
                        WareUnit = item.WareUnit,
                        ToIssue = item.WareLength * item.ToIssue,
                        Issued = item.Issued
                    }).GroupBy(rw => rw.WareCode).Select(item => new InProductionXML
                    {
                        WareName = item.Select(i => i.WareName).FirstOrDefault(),
                        WareCode = item.Select(i => i.WareCode).FirstOrDefault(),
                        WareUnit = item.Select(i => i.WareUnit).FirstOrDefault(),
                        TotalToIssue = item.Sum(i => i.ToIssue),
                        Issued = item.Sum(i => i.Issued)
                    }).ToList();
            }

            else if (listType == 3)
            {
                inProductionWareList = _context.InProductionRWs
                    .Include(rw => rw.InProduction)
                    .ThenInclude(ip => ip.Component)
                    .Include(rw => rw.Ware)
                    .Where(rw => (rw.InProduction.SourceType == (sbyte)LastSourceType.PlasmaOutWithoutEntrustedMaterial
                      || rw.InProduction.SourceType == (sbyte)LastSourceType.PlasmaWithEntrustedMaterial)
                      && rw.InProduction.OrderBookId == subOrder.OrderBookId
                      //&& rw.InProduction.ProductionState.ProductionStateCode == (sbyte)ProductionStateEnum.Awaiting
                      && rw.IsAdditionallyPurchasable == 0)
                    .Select(item => new InProductionXML
                    {
                        WareName = item.InProduction.Component.Name,
                        WareCode = item.InProduction.Component.Number,
                        WareUnit = "szt",
                        TotalToIssue = item.ToIssue,
                        Issued = item.Issued
                    }).ToList();
            }


            //inProductionRWListDto = _mapper.Map<List<InProductionRW>, List<InProductionRWDto>>(inProductionWareList);

            return Ok(inProductionWareList);
        }

        [HttpGet("book/getwaressummary")]
        public IActionResult GetWareSummaryList(string orderNumber, sbyte listType, string itemsList)
        {
             if(itemsList==null)
            {
                return BadRequest();
            }
            string[] orderBooksToList = itemsList.Split(",");

            #region OLD
            //int j = 0;
            //for (int i = 1; i < orderBooksToList.Length; i++)
            //{

            //    if (orderBooksToList[i-1] == "true")
            //    {
            //        if (i < 10)
            //        {
            //            orderBooksNumbers[j] = orderNumber + "_0" + i.ToString();
            //        }
            //        else
            //        {
            //            orderBooksNumbers[j] = orderNumber + "_" + i.ToString();
            //        }
            //        j++;
            //    }


            //}
            #endregion

            //List<InProductionRWDto> inProductionRWListDto;
            List<InProductionXML> inProductionSummaryWareList = new List<InProductionXML>();

            int k = 0;
            foreach (var orderBookNumber in orderBooksToList)
            {
                List<InProductionXML> inProductionWareList = new List<InProductionXML>();
                if (listType == 0)
                {
                    inProductionWareList = _context.InProductionRWs
                        .Include(rw => rw.InProduction)
                        .ThenInclude(ip => ip.Component)
                        .Include(rw => rw.Ware)
                        .Where(rw => rw.InProduction.SourceType == (sbyte)LastSourceType.Standard
                          && rw.Ware != null && rw.InProduction.OrderBook.Number == orderBookNumber
                          && rw.InProduction.ProductionState.ProductionStateCode == (sbyte)ProductionStateEnum.Awaiting
                          && rw.IsAdditionallyPurchasable == 0)
                        .GroupBy(rw => rw.InProduction.Component.Number).Select(rwd => new InProductionXML
                        {
                            ComponentNumber = rwd.Select(i => i.InProduction.Component.Number).FirstOrDefault(),
                            WareCode = rwd.Select(i => i.Ware.Code).FirstOrDefault(),
                            WareName = rwd.Select(i => i.Ware.Name).FirstOrDefault(),
                            WareUnit = rwd.Select(i => i.WareUnit).FirstOrDefault(),
                            WareLength = rwd.Select(i => i.WareLength).FirstOrDefault(),
                            Issued = rwd.Select(i => i.Issued).FirstOrDefault(),
                            WareQuantity = rwd.Sum(i => i.ToIssue)
                        }).ToList();


                }
                //Purchase list
                if (listType == 1)
                {
                    inProductionWareList = _context.InProductionRWs
                        .Include(rw => rw.InProduction)
                        .ThenInclude(ip => ip.Component)
                        .Include(rw => rw.Ware)
                        .Where(rw => ((rw.InProduction.SourceType == (sbyte)LastSourceType.Purchase)
                          || ((rw.InProduction.SourceType == (sbyte)LastSourceType.Standard
                          || rw.InProduction.SourceType == (sbyte)LastSourceType.PlasmaIn
                          || rw.InProduction.SourceType == (sbyte)LastSourceType.PlasmaOutWithoutEntrustedMaterial
                          || rw.InProduction.SourceType == (sbyte)LastSourceType.PlasmaWithEntrustedMaterial) && rw.IsAdditionallyPurchasable == 1))
                          && rw.Ware != null && rw.InProduction.OrderBook.Number == orderBookNumber)
                        .GroupBy(rw => rw.InProduction.Ware.Code).Select(rwd => new InProductionXML
                        {
                            ComponentNumber = rwd.Select(i => i.InProduction.Component.Number).FirstOrDefault(),
                            WareCode = rwd.Select(i => i.Ware.Code).FirstOrDefault(),
                            WareName = rwd.Select(i => i.Ware.Name).FirstOrDefault(),
                            WareUnit = rwd.Select(i => i.WareUnit).FirstOrDefault(),
                            WareLength = rwd.Select(i => i.WareLength).FirstOrDefault(),
                            Issued = rwd.Select(i => i.Issued).FirstOrDefault(),
                            TotalToIssue = rwd.Sum(i => i.ToIssue)
                        }).ToList();
                }
                else if (listType == 2)
                {
                    inProductionWareList = _context.InProductionRWs
                        .Include(rw => rw.InProduction)
                        .ThenInclude(ip => ip.Component)
                        .Include(rw => rw.Ware)
                        .Where(rw => rw.InProduction.SourceType == (sbyte)LastSourceType.PlasmaIn
                          && rw.Ware != null && rw.InProduction.OrderBook.Number == orderBookNumber
                          && rw.InProduction.ProductionState.ProductionStateCode == (sbyte)ProductionStateEnum.Awaiting
                          && rw.IsAdditionallyPurchasable == 0)
                        .Select(item => new InProductionXML
                        {
                            WareName = item.InProduction.Ware.Name,
                            WareCode = item.InProduction.Ware.Code,
                            WareUnit = item.WareUnit,
                            ToIssue = item.WareLength * item.ToIssue,
                            Issued = item.Issued
                        }).GroupBy(rw => rw.WareCode).Select(item => new InProductionXML
                        {
                            WareName = item.Select(i => i.WareName).FirstOrDefault(),
                            WareCode = item.Select(i => i.WareCode).FirstOrDefault(),
                            WareUnit = item.Select(i => i.WareUnit).FirstOrDefault(),
                            TotalToIssue = item.Sum(i => i.ToIssue),
                            Issued = item.Sum(i => i.Issued)
                        }).ToList();
                }

                else if (listType == 3)
                {
                    inProductionWareList = _context.InProductionRWs
                        .Include(rw => rw.InProduction)
                        .ThenInclude(ip => ip.Component)
                        .Include(rw => rw.Ware)
                        .Where(rw => (rw.InProduction.SourceType == (sbyte)LastSourceType.PlasmaOutWithoutEntrustedMaterial
                          || rw.InProduction.SourceType == (sbyte)LastSourceType.PlasmaWithEntrustedMaterial)
                          && rw.InProduction.OrderBook.Number == orderBookNumber
                          && rw.InProduction.ProductionState.ProductionStateCode == (sbyte)ProductionStateEnum.Awaiting
                          && rw.IsAdditionallyPurchasable == 0)
                        .Select(item => new InProductionXML
                        {
                            WareName = item.InProduction.Component.Name,
                            WareCode = item.InProduction.Component.Number,
                            WareUnit = "szt",
                            TotalToIssue = item.ToIssue,
                            Issued = item.Issued
                        }).ToList();
                }
                this.notRepetableList(inProductionWareList, inProductionSummaryWareList,listType);
                k++;
            }





            //inProductionRWListDto = _mapper.Map<List<InProductionRW>, List<InProductionRWDto>>(inProductionWareList);

            return Ok(inProductionSummaryWareList);
        }

        [HttpPost("book/getwarelistxml")]
        public IActionResult ExportWareList([FromBody]
            InProductionXML[] wareList,
            string orderNumber,
            string subOrderNumber,
            string componentNumber,
            string updateQuantitiesStr,
            string rwDate,
            string rwType,
            string magType)
        {
            XmlGenerator xmlGenerator = new XmlGenerator();

            //// Get order
            //Order order = _context.Orders.Where(o => o.Number == orderNumber).SingleOrDefault();

            //// Get book
            //OrderBook subOrder = _context.OrderBooks
            //    .Include(ob => ob.Book)
            //    .Where(ob => ob.Number == subOrderNumber).SingleOrDefault();

            //bool updateQuantities = bool.Parse(updateQuantitiesStr);
            //if (updateQuantities)
            //{
            //    foreach (InProductionXML item in wareList)
            //    {
            //        if(item.ToIssue == 0)
            //        {
            //            continue;
            //        }

            //        var items = _context.InProductionRWs
            //                .Include(rw => rw.InProduction)
            //                .ThenInclude(ip => ip.Component)
            //                .Where(rw => rw.InProduction.Component.Number == item.ComponentNumber
            //                && rw.ToIssue != rw.Issued).ToList();

            //        if (item.ToIssue == item.TotalToIssue)
            //        {
            //            items.ForEach(i => i.Issued = i.ToIssue);
            //            //_context.SaveChanges();
            //        }
            //        if (item.ToIssue < item.TotalToIssue)
            //        {
            //            var leftToIssue = item.ToIssue / item.WareLength;
            //            items.ForEach(i =>
            //            {

            //            });
            //        }
            //    }
            //}

            var xml = xmlGenerator.GenerateRw(wareList, rwDate, orderNumber, subOrderNumber, componentNumber, rwType, magType);

            return Ok(xml);
        }

        [HttpPost("book/getmultiplewarelistxml")]
        public IActionResult ExportMultipleWareList([FromBody]
            InProductionXML[] wareList,
    string orderNumber,
    string subOrderNumbers,
    string componentNumbers,
    string updateQuantitiesStr,
    string rwDate,
    string rwType,
    string magType)
        {
            XmlGenerator xmlGenerator = new XmlGenerator();

            var xml = xmlGenerator.GenerateRw(wareList, rwDate, orderNumber, subOrderNumbers, componentNumbers, rwType, magType);

            return Ok(xml);
        }


            [HttpGet("book/bom/{id}")]
        public IActionResult GenerateRw(int id, string orderNumber, string subOrderNumber, sbyte listType)
        {
            XmlGenerator xmlGenerator = new XmlGenerator();
            XmlDocument xmlDocument = new XmlDocument();
            XDocument doc = new XDocument();
            string webRootPath = _hostingEnvironment.WebRootPath;
            string newPath = Path.Combine(webRootPath, "Upload/Export/Xml", "RW");
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }

            // Get order
            Order order = _context.Orders.Where(o => o.Number == orderNumber).SingleOrDefault();

            // Get book
            OrderBook subOrder = _context.OrderBooks
                .Include(ob => ob.Book)
                .Where(ob => ob.Number == subOrderNumber).SingleOrDefault();

            //List<InProductionRWDto> inProductionRWListDto;

            List<InProductionXML> inProductionWareList = _context.InProductionRWs
                    .Include(rw => rw.InProduction)
                    .ThenInclude(ip => ip.Component)
                    .Include(rw => rw.Ware)
                    .Where(rw => rw.InProduction.SourceType == (sbyte)LastSourceType.Standard
                      && rw.Ware != null && rw.InProduction.OrderBookId == subOrder.OrderBookId
                      && rw.InProduction.ProductionState.ProductionStateCode == (sbyte)ProductionStateEnum.Awaiting)
                    .GroupBy(rw => rw.InProduction.Component.Number).Select(rwd => new InProductionXML
                    {
                        ComponentNumber = rwd.Select(i => i.InProduction.Component.Number).FirstOrDefault(),
                        WareCode = rwd.Select(i => i.Ware.Code).FirstOrDefault(),
                        WareName = rwd.Select(i => i.Ware.Name).FirstOrDefault(),
                        WareUnit = rwd.Select(i => i.WareUnit).FirstOrDefault(),
                        WareLength = rwd.Select(i => i.WareLength).FirstOrDefault(),
                        ToIssue = rwd.Sum(i => i.ToIssue),
                        Issued = rwd.Sum(i => i.Issued)
                    }).ToList();

            //inProductionRWListDto = _mapper.Map<List<InProductionRW>, List<InProductionRWDto>>(inProductionWareList);

            //decimal optimaItemQty = 0;

            //foreach (InProductionRWDto item in inProductionRWListDto)
            //{
            //    optimaItemQty = 0;
            //    // Get ware quantity from Optima if ware not null
            //    if (item.Ware != null)
            //    {//TODO OPTIMA ILOSC ZAKTUALIZOWAC
            //        //optimaItemQty = (decimal)((_optimaController.GetWarehouseItemQty(item.InProduction.Ware) as OkObjectResult).Value);
            //    }

            //    if(item.ToIssue > optimaItemQty)
            //    {
            //        item.ToIssue = Decimal.ToInt32(optimaItemQty);
            //    }

            //}


            // Get book wares
            //List<Ware> wares = (_bookController.GetWares(id) as OkObjectResult).Value as List<Ware>;
            //List<ComponentDto> wares = (_bookController.GetWares(id) as OkObjectResult).Value as List<ComponentDto>;

            //var xml = xmlGenerator.GenerateRw(inProductionWareList);

            return Ok();
        }


        #region helper functions
        private void notRepetableList(List<InProductionXML> source, List<InProductionXML> destination, sbyte listType)
        {
            foreach(InProductionXML component in source)
            {
                int index = destination.FindIndex((item) => item.WareCode == component.WareCode);
                if(index == -1)
                {
                    if (listType == 0)
                    {
                        component.TotalToIssue = component.WareQuantity * component.WareLength;
                        component.ToIssue = component.TotalToIssue;
                        destination.Add(component);
                    }
                    else
                    {
                        destination.Add(component);
                    }
                    
                }
                else
                {

                    if (listType==0)
                    {
                        //destination[index].WareQuantity += component.WareQuantity;
                        destination[index].TotalToIssue += (component.WareQuantity * component.WareLength);
                        destination[index].ToIssue += destination[index].TotalToIssue;
                        destination[index].Issued += (component.Issued * component.WareLength);
                    }
                    else
                    {
                        destination[index].TotalToIssue += component.TotalToIssue;
                    }

                }
            }
        }

        #endregion

    }

}