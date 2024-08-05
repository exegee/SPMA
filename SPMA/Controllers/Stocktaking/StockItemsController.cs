using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPMA.Core.ExtensionMethods;
using SPMA.Core.ExtensionMethods.Paging;
using SPMA.Data;
using SPMA.Models.Stocktaking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SPMA.Controllers.Stocktaking
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockItemsController : ControllerBase
    {
        #region Properties
        private ApplicationDbContext _context;
        #endregion

        #region Constructor
        public StockItemsController(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }
        #endregion

       [HttpGet]
       public IActionResult GetStockItems(string orderBy, string sortOrder, [FromQuery] int page, [FromQuery] int pageSize, string filter)
        {
            if (orderBy == null) orderBy = "StockItemId";
            bool asc = true;
            if (sortOrder == "desc")
            {
                asc = false;
            }

            var stockItems = _context.StockItems
                .Select(si => new StockItem
                {
                    StockItemId = si.StockItemId,
                    Code = si.Code,
                    Name = si.Name,
                    Type = si.Type,
                    PitQty = si.PitQty,
                    ActualQty = si.ActualQty,
                    DiffQty = si.DiffQty,
                    Unit = si.Unit,
                    Comment = si.Comment,
                    DateAdded = si.DateAdded
                }).Where(item => (
                                 EF.Functions.Like(item.Name, $"%{filter}%") ||
                                 EF.Functions.Like(item.Code, $"%{filter}%")
                                 )
                 )
                .OrderBy(orderBy, asc)
                .GetPaged(page, pageSize);

            PagedResult<StockItem> pagedStockItems = stockItems;


       
            return Ok(pagedStockItems);

        }

       [HttpPatch]
       public IActionResult UpdateStockItemQty(StockItem stockItem)
        {
            if (stockItem == null)
                return BadRequest();

            var stockItemInDb = _context.StockItems.Where(si => si.Code == stockItem.Code).FirstOrDefault();

            stockItemInDb.PitQty = stockItem.PitQty;
            stockItemInDb.DiffQty = stockItem.PitQty - stockItem.ActualQty;
            stockItemInDb.Comment = stockItem.Comment;
            stockItemInDb.DateAdded = DateTime.Now;

            _context.SaveChanges();

            return Ok(stockItem);
        }

        [HttpGet("count")]
        public IActionResult GetStockItemsCount()
        {
            var stockItemsCount = _context.StockItems.Count();

            return Ok(stockItemsCount);
        }

        [HttpDelete]
        public IActionResult DeleteAllStockItems()
        {
            _context.Database.ExecuteSqlCommand("TRUNCATE TABLE StockItems");

            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult DeteleStockItem(int id)
        {
            var stockItemInDb = _context.StockItems.SingleOrDefault(si => si.StockItemId == id);

            var stockItem = stockItemInDb;
            if (stockItemInDb == null)
                return Ok(false);
            // Type = 2 item removed flag
            stockItem.Type = 2;
            _context.StockItems.Remove(stockItem);
            _context.SaveChanges();
            return Ok(stockItem);
        }

        [HttpGet("export")]
        public IActionResult ExportTxtFile()
        {
            List<StockItem> stockItems = _context.StockItems.ToList();

            MemoryStream memoryStream = new MemoryStream();
            TextWriter tw = new StreamWriter(memoryStream, Encoding.Default);

            foreach(StockItem item in stockItems)
            {
                tw.WriteLine(item.Code + ";" + item.PitQty);
            }
            tw.Flush();
            tw.Close();

            return File(memoryStream.GetBuffer(), "text/plain", "filex.txt" );
        }

        [HttpPost]
        public IActionResult AddNewStockItem(StockItem stockItem)
        {
            stockItem.DateAdded = DateTime.Now;
            stockItem.Type = 1;

            _context.StockItems.Add(stockItem);
            var recordsSaved = _context.SaveChanges();

            if (recordsSaved > 0)
            {
                return Ok(true);
            }
            else
            {
                return Ok(false);
            }
            
        }
    }
}
