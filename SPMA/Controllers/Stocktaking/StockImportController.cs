using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using SPMA.Data;
using SPMA.Models.Stocktaking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SPMA.Controllers.Stocktaking
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockImportController : ControllerBase
    {
        #region Properties
        private IHostingEnvironment _hostingEnvironment;
        private ApplicationDbContext _context;
        #endregion

        #region Constructor
        public StockImportController(IHostingEnvironment hostingEnvironment, ApplicationDbContext dbContext)
        {
            _hostingEnvironment = hostingEnvironment;
            _context = dbContext;
        }
        #endregion

        [HttpGet]
        public IActionResult Get(string fileName)
        {
            // Set the upload paths
            string webRootPath = _hostingEnvironment.WebRootPath;
            string newPath = Path.Combine(webRootPath, "Upload\\xlsPIT", fileName);

            // Get file info
            FileInfo fileInfo = new FileInfo(newPath);
            if (!fileInfo.Exists)
                return BadRequest("Cannot find specified file");

            using (ExcelPackage package = new ExcelPackage(fileInfo))
            {
                ExcelWorksheet workSheet = package.Workbook.Worksheets.First();

                // Iterate each row in xls file
                for (var rowNum = 2; rowNum <= workSheet.Dimension.End.Row; rowNum++)
                {
                    StockItem stockItem = new StockItem
                    {
                        Code = Convert.ToString(workSheet.GetValue(rowNum, 2)),
                        Name = Convert.ToString(workSheet.GetValue(rowNum, 3)),
                        PitQty = Convert.ToInt16(workSheet.GetValue(rowNum, 4)),
                        Unit = Convert.ToString(workSheet.GetValue(rowNum, 7))
                    };
                    _context.StockItems.Add(stockItem);
                }
            }
            _context.SaveChanges();
            return Ok(true);
        }
    }
}
