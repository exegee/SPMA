using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SPMA.Data;
using AutoMapper;
using SPMA.Dtos.Production;
using SPMA.Models.Warehouse;
using SPMA.Models.Production;

namespace SPMA.Controllers.Production
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehouseItemController : ControllerBase
    {
        #region Properties  

        private ApplicationDbContext _context;
        private readonly IMapper _mapper;
        #endregion

        #region Constructor
         public WarehouseItemController(ApplicationDbContext dbContext, IMapper mapper)
        {
            _context = dbContext;
            _mapper = mapper;
        }
        #endregion

        #region Http Action Request
        [HttpGet("getItems")]
        public IActionResult GetItems()
        {

            List<WarehouseItemDto> surplusItemDto =
                _mapper.Map<List<WarehouseItem>, List<WarehouseItemDto>>(
                _context.WarehouseItems.Include(w=>w.Ware)
                .Include(w=>w.Component)
                .Include(w=>w.ParentRWItem)
                .ThenInclude(rw=>rw.InProduction)
                .ThenInclude(ip=>ip.OrderBook)
                .ToList());

            return Ok(surplusItemDto);
        }

        [HttpPost("postItem")]
        public IActionResult AddItem(WarehouseItemDto newSurplusItem)
        {
            if (newSurplusItem == null || newSurplusItem.Ware == null)
            {
                BadRequest("sended object is invalid");
            }
            Ware wareInDB = _context.Wares.Where(w => w.Code == newSurplusItem.Ware.Code).FirstOrDefault();
            Component componentInDB = _context.Components.Where(c => c.ComponentId == newSurplusItem.Component.ComponentId).FirstOrDefault();
            WarehouseItem surplusItemToDB = _mapper.Map<WarehouseItemDto, WarehouseItem>(newSurplusItem);
            surplusItemToDB.Ware = wareInDB;
            surplusItemToDB.Component = componentInDB;

            _context.WarehouseItems.Add(surplusItemToDB);
            _context.SaveChanges();

            return Ok();
        }

        [HttpPost("copyItem")]
        public IActionResult CopyItem([FromBody]int SurplusItemId)
        {
            if (SurplusItemId > 0 )
            {
                BadRequest("sended object is null");
            }
            WarehouseItem itemInDB = _context.WarehouseItems
                .Include(wi => wi.Ware)
                .Include(wi => wi.Component)
                .Where(wi => wi.WarehouseItemId == SurplusItemId)
                .FirstOrDefault();

            if(itemInDB != null)
            {
                WarehouseItem newItem = new WarehouseItem();
                newItem.Component = itemInDB.Component;
                newItem.Ware = itemInDB.Ware;
                newItem.ComponentQty = itemInDB.ComponentQty;
                newItem.WareQty = itemInDB.WareQty;
                newItem.WareQtySum = itemInDB.WareQtySum;
                newItem.AddedBy = itemInDB.AddedBy;
                newItem.Comment = itemInDB.Comment;
                newItem.AddedDate = DateTime.UtcNow;

                _context.WarehouseItems.Add(newItem);
                _context.SaveChanges();
            }
            else
            {
                BadRequest("Item not found");
            }
            

            return Ok();
        }


        [HttpPut("edit")]
        public IActionResult EditItem(WarehouseItemDto editedItem)
        {
            if (editedItem == null || editedItem?.ReservedQty!=0)
            {
                BadRequest("cant change reserved object");
            }

            Ware wareInDB = _context.Wares.Where(w => w.Code == editedItem.Ware.Code).FirstOrDefault();
            Component componentInDB = _context.Components.Where(c => c.ComponentId == editedItem.Component.ComponentId).FirstOrDefault();
            WarehouseItem surplusItemInDB = _context.WarehouseItems
                .Include(s => s.Component)
                .Include(s => s.Ware)
                .Where(s => s.WarehouseItemId == editedItem.WarehouseItemId)
                .SingleOrDefault();

            surplusItemInDB.Ware = wareInDB;
            surplusItemInDB.Component = componentInDB;
            surplusItemInDB.AddedBy = editedItem.AddedBy;
            surplusItemInDB.AddedDate = editedItem.AddedDate;
            surplusItemInDB.Comment = editedItem.Comment;
            surplusItemInDB.ComponentQty = editedItem.ComponentQty;
            surplusItemInDB.WareQtySum = editedItem.WareQtySum;
            surplusItemInDB.WareQty = editedItem.WareQty;

            _context.SaveChanges();

            return Ok();
        }


        [HttpDelete("delete/{itemId}")]
        public IActionResult Deleteitem(int itemId)
        {
            WarehouseItem itemInDB = _context.WarehouseItems
                .SingleOrDefault(s => s.WarehouseItemId == itemId);

            if (itemInDB == null || itemInDB?.ReservedQty!=0)
            {
                return BadRequest("cant delete reserved item");
            }


            _context.WarehouseItems.Remove(itemInDB);
            _context.SaveChanges();

            return Ok();
        }

        [HttpGet("reservation")]
        public IActionResult GetComponentFromWarehouseItems(string componentNumber)
        {
            List<WarehouseItem> components = _context.WarehouseItems
                .Where(w => w.Component.Number == componentNumber).Include(w => w.Ware)
                .Include(w => w.Component)
                .Include(w => w.ParentRWItem)
                .ThenInclude(rw => rw.InProduction)
                .ThenInclude(ip => ip.OrderBook)
                .ToList();

            return Ok(components);
        }

        #endregion

    }
}
