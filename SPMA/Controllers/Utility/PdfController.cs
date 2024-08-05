using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MigraDoc.Rendering;
using SPMA.Controllers.Optima;
using SPMA.Controllers.Production;
using SPMA.Controllers.Warehouse;
using SPMA.Core.Tools;
using SPMA.Data;
using SPMA.Dtos.Orders;
using SPMA.Dtos.Production;
using SPMA.Models.Orders;
using SPMA.Models.Production;
using SPMA.Models.Warehouse;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using static SPMA.Core.Enums;

namespace SPMA.Controllers.Utility
{


    [Route("api/[controller]")]
    [ApiController]
    public class PdfController : ControllerBase
    {
        #region Properties
        private IHostingEnvironment _hostingEnvironment;
        private readonly BookController _bookController;
        private readonly BookRWController _bookRWController;
        private readonly ApplicationDbContext _context;
        private readonly OptimaController _optimaController;
        private readonly IMapper _mapper;
        #endregion

        #region Constructor
        public PdfController(BookController bookController,
            BookRWController bookRWController,
            ApplicationDbContext context,
            IHostingEnvironment hostingEnvironment,
            IMapper mapper,
            OptimaController optimaController)
        {
            _bookRWController = bookRWController;
            _bookController = bookController;
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _mapper = mapper;
            _optimaController = optimaController;
        }
        #endregion

        #region Htttp Action Requests

        /// <summary>
        /// Generates Orders list to print
        /// </summary>
        /// <param name="id"></param>
        /// <param name="orderNumber"></param>
        /// <param name="subOrderNumber"></param>
        /// <param name="listType"></param>
        /// <returns></returns>
        [HttpPost("order/orderslist")]
        public IActionResult GetOrdersList([FromBody] List<OrderDto> orders)
        {
            // Set path for saving pdf file
            string webRootPath = _hostingEnvironment.WebRootPath;
            string newPath = Path.Combine(webRootPath, "Download/Pdf", "RW");
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }

            // Set encoding
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var enc1252 = Encoding.GetEncoding(1252);

            // Create a renderer for PDF that uses Unicode font encoding
            PdfGenerator pdfGenerator = new PdfGenerator();
            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true);

            // Set the MigraDoc document
            pdfRenderer.Document = pdfGenerator.GenerateOrdersList(orders);

            // Create the PDF document
            pdfRenderer.RenderDocument();

            //// Save the PDF document...
            //string filename = "rw.pdf";
            //pdfRenderer.Save(filename);

            // Save to stream
            MemoryStream stream = new MemoryStream();
            pdfRenderer.Save(stream, false);

            return File(stream, "application/pdf");
        }


        [HttpPost("order/suborderslist")]
        public IActionResult GetSubOrdersList([FromBody] List<OrderBookDto> orderBooks)
        {
            // Set path for saving pdf file
            string webRootPath = _hostingEnvironment.WebRootPath;
            string newPath = Path.Combine(webRootPath, "Download/Pdf", "RW");
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }

            // Set encoding
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var enc1252 = Encoding.GetEncoding(1252);

            // Create a renderer for PDF that uses Unicode font encoding
            PdfGenerator pdfGenerator = new PdfGenerator();
            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true);

            // Set the MigraDoc document
            pdfRenderer.Document = pdfGenerator.GenerateSubOrdersList(orderBooks);

            // Create the PDF document
            pdfRenderer.RenderDocument();

            //// Save the PDF document...
            //string filename = "rw.pdf";
            //pdfRenderer.Save(filename);

            // Save to stream
            MemoryStream stream = new MemoryStream();
            pdfRenderer.Save(stream, false);

            return File(stream, "application/pdf");
        }

        [HttpGet("order/{id}/details")]
        public IActionResult GetOrderDetail(int id)
        {
            // Set path for saving pdf file
            string webRootPath = _hostingEnvironment.WebRootPath;
            string newPath = Path.Combine(webRootPath, "Download/Pdf", "RW");
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }

            // Set encoding
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var enc1252 = Encoding.GetEncoding(1252);

            // Create a renderer for PDF that uses Unicode font encoding
            PdfGenerator pdfGenerator = new PdfGenerator();
            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true);

            // Get data
            ProductionState awaitingProductionState = _context.ProductionStates.Where(ps => ps.ProductionStateCode == (sbyte)ProductionStateEnum.Awaiting).SingleOrDefault();
            Order order = _context.Orders.Where(o => o.OrderId == id).SingleOrDefault();
            List<InProduction> inProductionItems = _context.InProduction
                            .Include(ip => ip.Component)
                            .Include(ip => ip.OrderBook)
                            .ThenInclude(ob => ob.Book)
                            .Where(ip => ip.OrderBook.OrderId == id && ip.ProductionState == awaitingProductionState)
                            .OrderBy(x => x.OrderBookId).ThenBy(x => x.BookOrderTree)
                            .ToList();

            List<InProductionDto> inProductionItemsDto = _mapper.Map<List<InProduction>, List<InProductionDto>>(inProductionItems);



            //foreach(InProductionDto item in inProductionItemsDto)
            //{
            //    List<KeyValuePair<string,int>> itemInfo = _context.BookComponents.Where(bk => bk.BookId == item.OrderBook.BookId && bk.ComponentId == item.ComponentId)
            //                           .Select(x => new List<KeyValuePair<string,int>>() {
            //                               new KeyValuePair<string, int>("LevelTree", x.Level),
            //                               new KeyValuePair<string, int>("OrderTree", x.Order)
            //                           }).Single();
            //    item.BookLevelTree = itemInfo[0].Value;
            //    item.BookOrderTree = itemInfo[1].Value;
            //}


            // Set the MigraDoc document
            pdfRenderer.Document = pdfGenerator.GenerateOrderDetails(order, inProductionItems);

            // Create the PDF document
            pdfRenderer.RenderDocument();

            //// Save the PDF document...
            //string filename = "rw.pdf";
            //pdfRenderer.Save(filename);

            // Save to stream
            MemoryStream stream = new MemoryStream();
            pdfRenderer.Save(stream, false);

            return File(stream, "application/pdf");
        }

        // Generates BOM list
        [HttpGet("book/parts/{id}")]
        public IActionResult GeneratePartList(int id, string orderNumber, string subOrderNumber, sbyte listType)
        {
            // Set path for saving pdf file
            string webRootPath = _hostingEnvironment.WebRootPath;
            string newPath = Path.Combine(webRootPath, "Download/Pdf", "RW");
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }

            // Get http request headers
            //string orderNumber = HttpContext.Request.Headers["orderNumber"];
            //string subOrderNumber = HttpContext.Request.Headers["subOrderNumber"];
            bool genZD = Convert.ToBoolean(HttpContext.Request.Headers["genZD"]);

            // ListType
            // 0 - Saw
            // 1 - Plazma
            // 2 - BOM
            //sbyte listType = Convert.ToSByte(HttpContext.Request.Headers["listType"]);

            // Get order
            Order order = _context.Orders.Where(o => o.Number == orderNumber).SingleOrDefault();

            // Get book
            Book book = _context.Books.Where(b => b.BookId == id).SingleOrDefault();

            //*********************************** PRZEROBIC **************************
            List<BookComponent> bomComponents = _context.BookComponents
                .Include(bc => bc.Component)
                .Include(bc => bc.Book)
                .Where(bc => bc.BookId == book.BookId && bc.LastSourceType != (sbyte)LastSourceType.Purchase)
                .OrderBy(bc => bc.Order)
                .ToList();

            List<BookComponentDto> bomComponentsDto = _mapper.Map<List<BookComponent>, List<BookComponentDto>>(bomComponents);
            //componentsToIssue = _context.BookRWs  
            //.Include(rw => rw.InProduction)
            //.ThenInclude(ip => ip.Book)
            //.Include(rw2 => rw2.InProduction)
            //.ThenInclude(ip2 => ip2.Component)
            //.ThenInclude(c => c.Ware)
            //.Include(rw3 => rw3.InProduction)
            //.ThenInclude(ip3 => ip3.Order)
            //.Include(v => v.InProduction.ProductionState)
            //.Where(w => w.InProduction.Order.Number == subOrderNumber &&
            //            w.InProduction.Book.BookId == book.BookId &&
            //            w.InProduction.ProductionState.ProductionStateCode == (sbyte)ProductionStateEnum.Awaiting
            //            )
            //.OrderBy(r => r.InProduction.Order)
            //.ToList();

            // Set encoding
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var enc1252 = Encoding.GetEncoding(1252);

            var bookOfficeNumber = (_bookController.GetBookOfficeNumber(id) as OkObjectResult).Value as string;
            var bookComponentNumber = (_bookController.GetBookComponentNumber(id) as OkObjectResult).Value as string;
            var bookName = (_bookController.GetBookName(id) as OkObjectResult).Value as string;

            // Create a renderer for PDF that uses Unicode font encoding
            PdfGenerator pdfGenerator = new PdfGenerator();
            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true);

            // Set the MigraDoc document
            pdfRenderer.Document = pdfGenerator.GenerateBomList(bomComponentsDto, orderNumber, bookOfficeNumber, bookName, bookComponentNumber, subOrderNumber, genZD, listType);

            // Create the PDF document
            pdfRenderer.RenderDocument();

            //// Save the PDF document...
            //string filename = "rw.pdf";
            //pdfRenderer.Save(filename);

            // Save to stream
            MemoryStream stream = new MemoryStream();
            pdfRenderer.Save(stream, false);

            return File(stream, "application/pdf");
        }

        //Generates Wares list to cut
        [HttpGet("book/bom/{id}")]
        public IActionResult GenerateBomList(int id, string orderNumber, string subOrderNumber, sbyte listType)
        {
            // Set path for saving pdf file
            string webRootPath = _hostingEnvironment.WebRootPath;
            string newPath = Path.Combine(webRootPath, "Download/Pdf", "RW");
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }

            // Get http request headers
            //string orderNumber = HttpContext.Request.Headers["orderNumber"];
            //string subOrderNumber = HttpContext.Request.Headers["subOrderNumber"];
            bool genZD = Convert.ToBoolean(HttpContext.Request.Headers["genZD"]);

            // ListType
            // 0 - Saw
            // 1 - Plazma
            // 2 - BOM
            //sbyte listType = Convert.ToSByte(HttpContext.Request.Headers["listType"]);

            // Get order
            Order order = _context.Orders.Where(o => o.Number == orderNumber).SingleOrDefault();

            // Get book
            OrderBook subOrder = _context.OrderBooks
                .Include(ob => ob.Book)
                .Where(ob => ob.Number == subOrderNumber).SingleOrDefault();

            List<InProductionRWDto> inProductionRWListDto;

            // If listType is set to 0 get wares list
            if (listType == 0)
            {
                //List<InProductionRW> inProductionWareList = _context.InProductionRWs
                //    .Include(rw => rw.InProduction)
                //    .ThenInclude(ip => ip.Component)
                //    .Include(rw => rw.Ware)
                //    .Where(rw => rw.InProduction.SourceType == (sbyte)LastSourceType.Standard
                //      && rw.Ware != null && rw.InProduction.OrderBookId == subOrder.OrderBookId
                //      && rw.InProduction.ProductionState.ProductionStateCode == (sbyte)ProductionStateEnum.Awaiting)
                //    .OrderBy(rw => rw.InProduction.Component.Number)
                //    .ToList();

            
                  List<InProductionRW> inProductionWareList = _context.InProductionRWs
                  .Include(rw => rw.InProduction)
                  .ThenInclude(ip => ip.Component)
                  .Include(rw => rw.Ware)
                  .Where(rw => rw.InProduction.SourceType == (sbyte)LastSourceType.Standard
                    && rw.Ware != null && rw.InProduction.OrderBookId == subOrder.OrderBookId
                    //&& rw.InProduction.ProductionState.ProductionStateCode == (sbyte)ProductionStateEnum.Awaiting
                    )
                  .GroupBy(rw => rw.InProduction.Component.Number).Select(rwd => new InProductionRW
                  {
                      InProduction = rwd.Select(i => i.InProduction).FirstOrDefault(),
                      Ware = rwd.Select(i => i.Ware).FirstOrDefault(),
                      ToIssue = rwd.Sum(i => i.ToIssue),
                      WareLength = rwd.Select(i => i.WareLength).FirstOrDefault(),
                      WareUnit = rwd.Select(i => i.WareUnit).FirstOrDefault()
                  }).AsNoTracking().ToList();


                inProductionRWListDto = _mapper.Map<List<InProductionRW>, List<InProductionRWDto>>(inProductionWareList);
            }
            // If listType is set to 1 get purchase wares list
            else if (listType == 1)
            {
                //// TODO Remove this after some time!!!
                //var updateDate = new DateTime(2020, 2, 7);
                //sbyte productionState;
                //if (subOrder.AddedDate >= updateDate)
                //{
                //    productionState = (sbyte)ProductionStateEnum.Awaiting;
                //}
                //else
                //{
                //    productionState = (sbyte)ProductionStateEnum.PurchaseItem;
                //}

                List<InProductionRW> inProductionPurchaseList = _context.InProductionRWs
                    .Include(rw => rw.InProduction)
                    .ThenInclude(ip => ip.Component)
                    .Include(rw => rw.Ware)
                    .Where(rw => 
                      ((rw.InProduction.SourceType == (sbyte)LastSourceType.Purchase) 
                      || ((rw.InProduction.SourceType == (sbyte)LastSourceType.Standard 
                      || rw.InProduction.SourceType == (sbyte)LastSourceType.PlasmaIn
                      || rw.InProduction.SourceType == (sbyte)LastSourceType.PlasmaOutWithoutEntrustedMaterial
                      || rw.InProduction.SourceType == (sbyte)LastSourceType.PlasmaWithEntrustedMaterial) && rw.IsAdditionallyPurchasable == 1))
                      && rw.Ware != null && rw.InProduction.OrderBookId == subOrder.OrderBookId
                      //&& rw.InProduction.ProductionState.ProductionStateCode == productionState
                      ) 
                    .OrderBy(rw => rw.InProduction.Component.Number)
                    .ToList();

                inProductionRWListDto = _mapper.Map<List<InProductionRW>, List<InProductionRWDto>>(inProductionPurchaseList);
            }
            // If listType is set to 2 get plazma internal cut wares list
            else if (listType == 2)
            {
                List<InProductionRW> inProductionPurchaseList = _context.InProductionRWs
                    .Include(rw => rw.InProduction)
                    .ThenInclude(ip => ip.Component)
                    .Include(rw => rw.Ware)
                    .Where(rw => 
                      rw.InProduction.SourceType == (sbyte)LastSourceType.PlasmaIn
                      && rw.Ware != null && rw.InProduction.OrderBookId == subOrder.OrderBookId
                      //&& rw.InProduction.ProductionState.ProductionStateCode == (sbyte)ProductionStateEnum.Awaiting
                      )
                    .GroupBy(rw => rw.InProduction.Component.Number).Select(rwd => new InProductionRW
                    {
                        InProduction = rwd.Select(i => i.InProduction).FirstOrDefault(),
                        Ware = rwd.Select(i => i.Ware).FirstOrDefault(),
                        ToIssue = rwd.Sum(i => i.ToIssue),
                        WareLength = rwd.Select(i => i.WareLength).FirstOrDefault(),
                        WareUnit = rwd.Select(i => i.WareUnit).FirstOrDefault()
                    }).ToList();
                //.OrderBy(rw => rw.InProduction.Component.Number)
                //.ToList();

                inProductionRWListDto = _mapper.Map<List<InProductionRW>, List<InProductionRWDto>>(inProductionPurchaseList);
            }
            else
            {
                List<InProductionRW> inProductionPurchaseList = _context.InProductionRWs
                    .Include(rw => rw.InProduction)
                    .ThenInclude(ip => ip.Component)
                    .Include(rw => rw.Ware)
                    .Where(rw => (rw.InProduction.SourceType == (sbyte)LastSourceType.PlasmaWithEntrustedMaterial || rw.InProduction.SourceType == (sbyte)LastSourceType.PlasmaOutWithoutEntrustedMaterial)
                      && rw.InProduction.OrderBookId == subOrder.OrderBookId
                      //&& rw.InProduction.ProductionState.ProductionStateCode == (sbyte)ProductionStateEnum.Awaiting
                      )
                    .GroupBy(rw => rw.InProduction.Component.Number).Select(rwd => new InProductionRW
                    {
                        InProduction = rwd.Select(i => i.InProduction).FirstOrDefault(),
                        Ware = rwd.Select(i => i.Ware).FirstOrDefault(),
                        ToIssue = rwd.Sum(i => i.ToIssue),
                        WareLength = rwd.Select(i => i.WareLength).FirstOrDefault(),
                        WareUnit = rwd.Select(i => i.WareUnit).FirstOrDefault()
                    }).ToList();
                //.OrderBy(rw => rw.InProduction.Component.Number)
                //.ToList();

                inProductionRWListDto = _mapper.Map<List<InProductionRW>, List<InProductionRWDto>>(inProductionPurchaseList);
            }


            foreach (InProductionRWDto item in inProductionRWListDto)
            {
                //if (item.ToIssue >= 1)
                //{
                //    item.ToIssue = item.InProduction.PlannedQty * subOrder.PlannedQty * order.PlannedQty;
                //    //item.ToIssuePerBook = item.ToIssue / subOrder.PlannedQty / order.PlannedQty;
                //    //item.ToIssuePerSubOrder = item.ToIssue / order.PlannedQty;
                //}

                // Check how many items are left to issue
                int itemQtyLeftToIssue = item.ToIssue - item.Issued;

                // If 0 then skip this item
                if (itemQtyLeftToIssue == 0)
                    continue;

                decimal optimaItemQty = 0;

                // Get ware quantity from Optima if ware not null
                if (item.InProduction.Ware != null)
                {
                    string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
                    optimaItemQty = (decimal)((_optimaController.GetWarehouseItemQty(item.InProduction.Ware.Code, currentDate) as OkObjectResult).Value);
                }

                item.QtyWhDiff = optimaItemQty - (item.ToIssue * item.WareLength);

            }

            // Set encoding
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var enc1252 = Encoding.GetEncoding(1252);

            var bookOfficeNumber = (_bookController.GetBookOfficeNumber(id) as OkObjectResult).Value as string;
            var bookComponentNumber = (_bookController.GetBookComponentNumber(id) as OkObjectResult).Value as string;
            var bookName = (_bookController.GetBookName(id) as OkObjectResult).Value as string;
            //var orderName = order.Name;

            // Create a renderer for PDF that uses Unicode font encoding
            PdfGenerator pdfGenerator = new PdfGenerator();
            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true);

            // Set the MigraDoc document
            pdfRenderer.Document = pdfGenerator.GenerateWareList(
                inProductionRWListDto, 
                order,
                subOrder,
                bookOfficeNumber, 
                bookName, 
                bookComponentNumber, 
                subOrderNumber, 
                genZD, 
                listType);

            // Create the PDF document
            pdfRenderer.RenderDocument();

            // Save to stream
            MemoryStream stream = new MemoryStream();
            pdfRenderer.Save(stream, false);

            return File(stream, "application/pdf");
        }

        [HttpGet("book/rw/{id}")]
        public IActionResult GenerateRW(int id)
        {
            // Set path for saving pdf file
            string webRootPath = _hostingEnvironment.WebRootPath;
            string newPath = Path.Combine(webRootPath, "Download/Pdf", "RW");
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }

            // Get http request headers
            string orderNumber = HttpContext.Request.Headers["orderNumber"];
            string subOrderNumber = HttpContext.Request.Headers["subOrderNumber"];
            bool genZD = Convert.ToBoolean(HttpContext.Request.Headers["genZD"]);
            // ListType
            // 0 - Saw
            // 1 - Plazma
            sbyte listType = Convert.ToSByte(HttpContext.Request.Headers["listType"]);

            // Get order
            Order order = _context.Orders.Where(o => o.Number == orderNumber).SingleOrDefault();

            // Get book
            Book book = _context.Books.Where(b => b.BookId == id).SingleOrDefault();

            List<InProductionRW> componentsToIssue;
            if (listType == 0)
            {//*********************************** PRZEROBIC **************************
                componentsToIssue = null;
                //componentsToIssue = _context.BookRWs
                //.Include(rw => rw.InProduction)
                //.ThenInclude(ip => ip.BookComponent)
                //.ThenInclude(bc => bc.Component)
                //.ThenInclude(c => c.Ware)
                //.Include(rw2 => rw2.InProduction)
                //.ThenInclude(ip2 => ip2.OrderBook)
                //.Include(vv => vv.InProduction.BookComponent.Book)
                //.Include(v => v.InProduction.ProductionState)
                //.Where(w => w.InProduction.OrderBook.Number == subOrderNumber &&
                //            w.InProduction.BookComponent.BookId == book.BookId &&
                //            w.InProduction.ProductionState.ProductionStateCode == (sbyte)ProductionStateEnum.Awaiting &&
                //            w.InProduction.BookComponent.LastSourceType == (sbyte)LastSourceType.Standard &&
                //            w.Issued != w.ToIssue 
                //            )
                //.OrderBy(r => r.InProduction.BookComponent.Order)
                //.ToList();

                //List<BookRWDto> componentsToIssueDtos = _mapper.Map<List<BookRW>, List<BookRWDto>>(componentsToIssue);

                // UPDATES BOOKRW - DO THIS ON EXPORTING TO XML ONLY !!!
                // Locally stores issued ammount of wares
                //List<WareStruct> wareIssuedAmount = new List<WareStruct>();

                //foreach (BookRW rwItem in componentsToIssue)
                //{
                //    if (rwItem.InProduction.Ware == null)
                //        continue;

                //    string rwItemWareCode = rwItem.InProduction.BookComponent.Component.Ware.Code;
                //    // Check how many items are left to issue
                //    int rwItemQtyLeftToIssue = rwItem.ToIssue - rwItem.Issued;

                //    // If 0 then skip this item
                //    if (rwItemQtyLeftToIssue == 0)
                //        continue;

                //    // Check if current ware have been already issued
                //    var issuedRwItem = wareIssuedAmount.Find(f => f.WareCode == rwItemWareCode);

                //    decimal wareAvailableLength = Convert.ToDecimal((_optimaController.GetWarehouseItemQty(rwItemWareCode) as OkObjectResult).Value);
                //    decimal rwItemLength = (decimal)rwItem.InProduction.BookComponent.WareLength;
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


                //if(bookRWDto.InProduction.Ware != null)
                //{
                //    var componentsGrouped = componentsToIssueDtos
                //        .Where(d => d.InProduction.Ware != null)
                //        .Where(c => c.InProduction.BookComponent.Component.Ware.Code == bookRWDto.InProduction.BookComponent.Component.Ware.Code)
                //        .GroupBy(r => new
                //        {
                //            r.InProduction.BookComponent.Component.Ware.Code,
                //            r.InProduction.BookComponent.WareLength
                //        }).Select(s => s.Sum(i => i.ToIssue));

                //decimal wareAvailableQty = Convert.ToDecimal((_optimaController.GetWarehouseItemQty(bookRWDto.InProduction.BookComponent.Component.Ware.Code) as OkObjectResult).Value);


            }
            //}
            //}
            else
            {
                //if (listType == 1)
                // TODO PRZEROBIC NA PLAZME!!!
                //*********************************** PRZEROBIC **************************
                componentsToIssue = null;
                //componentsToIssue = _context.BookRWs
                //.Include(rw => rw.InProduction)
                //.ThenInclude(ip => ip.BookComponent)
                //.ThenInclude(bc => bc.Component)
                //.ThenInclude(c => c.Ware)
                //.Include(rw2 => rw2.InProduction)
                //.ThenInclude(ip2 => ip2.OrderBook)
                //.Include(vv => vv.InProduction.BookComponent.Book)
                //.Include(v => v.InProduction.ProductionState)
                //.Where(w => w.InProduction.OrderBook.Number == subOrderNumber &&
                //            w.InProduction.BookComponent.BookId == book.BookId &&
                //            w.InProduction.ProductionState.ProductionStateCode == (sbyte)ProductionStateEnum.Awaiting &&
                //            w.InProduction.BookComponent.LastSourceType == (sbyte)LastSourceType.Standard &&
                //            w.InProduction.Ware != null)
                //.ToList();
            }


            // Set encoding
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var enc1252 = Encoding.GetEncoding(1252);

            // Get components to issue
            //componentsToIssue = _bookRWController.UpdateBookRW(bookComponentRW);
            //List<Ware> wares = (_bookController.GetWares(id) as OkObjectResult).Value as List<Ware>;
            //List<ComponentDto> wares = (_bookController.GetWares(id) as OkObjectResult).Value as List<ComponentDto>;

            //foreach (ComponentDto component in wares)
            //{
            //    foreach (ComponentWareDto componentWareDto in component.ComponentWareDtos)
            //    {
            //        //int qtyToIssue = componentsToIssue.Select(x => x.)
            //        //componentWareDto.QtyToIssue = 
            //    }
            //}

            var bookOfficeNumber = (_bookController.GetBookOfficeNumber(id) as OkObjectResult).Value as string;
            var bookComponentNumber = (_bookController.GetBookComponentNumber(id) as OkObjectResult).Value as string;

            // Create a renderer for PDF that uses Unicode font encoding
            PdfGenerator pdfGenerator = new PdfGenerator();
            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true);

            // Set the MigraDoc document
            pdfRenderer.Document = pdfGenerator.GenerateRW(componentsToIssue, orderNumber, bookOfficeNumber, bookComponentNumber, subOrderNumber, genZD, listType);

            // Create the PDF document
            pdfRenderer.RenderDocument();

            // Save the PDF document...
            string filename = "rw.pdf";
            pdfRenderer.Save(filename);

            // Save to stream
            MemoryStream stream = new MemoryStream();
            pdfRenderer.Save(stream, false);

            return File(stream, "application/pdf");
        }

        [HttpGet("book/zd/{id}")]
        public IActionResult GenerateOrder(int id)
        {
            string webRootPath = _hostingEnvironment.WebRootPath;
            string newPath = Path.Combine(webRootPath, "Download/Pdf", "ZD");
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }

            // Get orderNumber from header
            string orderNumber = HttpContext.Request.Headers["orderNumber"];
            string subOrderNumber = HttpContext.Request.Headers["subOrderNumber"];

            // Set encoding
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var enc1252 = Encoding.GetEncoding(1252);

            // Get book wares
            //List<Ware> wares = (_bookController.GetWares(id) as OkObjectResult).Value as List<Ware>;
            List<ComponentDto> wares = (_bookController.GetWares(id) as OkObjectResult).Value as List<ComponentDto>;
            var bookOfficeNumber = (_bookController.GetBookOfficeNumber(id) as OkObjectResult).Value as string;
            var bookComponentNumber = (_bookController.GetBookComponentNumber(id) as OkObjectResult).Value as string; ;



            // Create a renderer for PDF that uses Unicode font encoding
            PdfGenerator pdfGenerator = new PdfGenerator();
            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true);


            // Set the MigraDoc document
            pdfRenderer.Document = pdfGenerator.GenerateZD(wares, orderNumber, bookOfficeNumber, bookComponentNumber, subOrderNumber);

            // Create the PDF document
            pdfRenderer.RenderDocument();

            // Save the PDF document...
            string filename = "zd.pdf";
            pdfRenderer.Save(filename);

            // Save to stream
            MemoryStream stream = new MemoryStream();
            pdfRenderer.Save(stream, false);


            return File(stream, "application/pdf");
        }
        #endregion
    }
}

