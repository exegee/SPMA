using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPMA.Controllers.Warehouse;
using SPMA.Data;
using SPMA.Dtos.Production;
using SPMA.Dtos.Warehouse;
using SPMA.Models.Production;
using SPMA.Models.Warehouse;
using System;
using System.Collections.Generic;
using System.Linq;
using SPMA.Core.ExtensionMethods.Components;
using SPMA.Core.ExtensionMethods.Paging;

namespace SPMA.Controllers.Production
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComponentsController : ControllerBase
    {
        #region Properties
        private ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly WarehouseController _warehouseController;
        #endregion

        #region Constructor
        public ComponentsController(ApplicationDbContext dbContext, IMapper mapper, WarehouseController warehouseController)
        {
            _context = dbContext;
            _mapper = mapper;
            _warehouseController = warehouseController;
        }
        #endregion

        #region Http Action Requests

        //GET /api/components
        [HttpGet]
        public IActionResult GetComponents()
        {
            return Ok(_context.Components.ToList());
        }

        //GET /api/components/ware/id
        [HttpGet("ware/{number}")]
        public IActionResult GetComponentWare(string number)
        {
            var componentInDb = _context.Components.Where(c => c.Number == number)
                .Include(i => i.Ware).SingleOrDefault();

            if (componentInDb == null)
                return Ok(new ComponentDto());

            ComponentDto componentDto = _mapper.Map<Component, ComponentDto>(componentInDb);
            return Ok(componentDto);
        }

        //PATCH /api/components/ware
        [HttpPatch("ware")]
        public IActionResult GetComponentWare2(ComponentDto component)
        {
            var number = component.Number;
            var componentInDb = _context.Components.Where(c => c.Number == number)
                .Include(i => i.Ware).SingleOrDefault();

            if (componentInDb == null)
                return Ok(new ComponentDto());

            ComponentDto componentDto = _mapper.Map<Component, ComponentDto>(componentInDb);
            return Ok(componentDto);
        }

        [HttpGet ("filteredcomponents/{filterStr}")]
        public IActionResult GetFilteredComponents(string filterStr)
        {


            var components = _context.Components
                .Select(o => new ComponentDto
                {
                    Name = o.Name,
                    Number = o.Number,
                    MaterialType = o.MaterialType,
                    Comment = o.Comment,
                    Cost = o.Cost,
                    Weight = o.Weight,
                    Description = o.Description,
                    ComponentType = o.ComponentType,
                    ComponentId = o.ComponentId,
                    LastSourceType = o.LastSourceType,
                    Author = o.Author,
                    AddedBy = o.AddedBy,
                    ModifiedDate = o.ModifiedDate,
                    Ware = _mapper.Map<Ware, WareDto>(o.Ware),
                    WareQuantity = o.WareQuantity,
                    WareLength = o.WareLength,
                    WareUnit = o.WareUnit,
                    LastTechnology = o.LastTechnology,
                    IsAdditionallyPurchasable = o.IsAdditionallyPurchasable
                })
                .Where(item => (EF.Functions.Like(item.Name, $"%{filterStr}%") |
                               EF.Functions.Like(item.Number, $"%{filterStr}%"))&&
                               item.ComponentType ==0 && item.LastSourceType<3)
                .OrderBy("Number", true).ToList();

            return Ok(components);
        }

        [HttpDelete("ware/{id}")]
        //DELETE /api/components/wares/
        public IActionResult DeleteComponentWare(int id)
        {
            var componentInDb = _context.Components.Include(x => x.Ware)
                .SingleOrDefault(o => o.ComponentId == id);

            if (componentInDb == null)
                return NotFound();

            // Clear component ware
            componentInDb.Ware = null;
            componentInDb.WareLength = 0;
            componentInDb.WareQuantity = 0;
            componentInDb.WareUnit = null;

            // Save changes to database
            _context.SaveChanges();
            return Ok();
        }

        [HttpPost]
        //POST /api/components
        public IActionResult AddComponent(ComponentDto componentDto)
        {
            if (componentDto == null)
                return BadRequest();

            // Check for component in database
            var componentInDb = _context.Components.Where(c => c.Number == componentDto.Number).Include(c => c.Ware).FirstOrDefault();

            if (componentInDb != null)
                return Ok(componentInDb);

            var component = _mapper.Map<ComponentDto, Component>(componentDto);
            component.Ware = null;
            component.ModifiedDate = DateTime.Now;
            _context.Components.Add(component);

            _context.SaveChanges();
        

            //Get the component
            componentInDb = _context.Components.FirstOrDefault(c => c.Number == componentDto.Number);

            return Ok(componentInDb);
        }


        [HttpPatch("inproduction")]
        //GET /api/components/inproduction
        public IActionResult GetInProduction(ComponentDto component)
        {

            List<InProduction> componentsInProduction = _context.InProduction
                .Include(c => c.Component)
                .Include(c => c.OrderBook)
                .ThenInclude(ob => ob.Order)
                .Where(x => x.Component.Number == component.Number &&
                       x.ProductionState.ProductionStateCode > 1 &&
                       x.ProductionState.ProductionStateCode < 4 &&
                       x.OrderBook.Order.State != 10 &&
                       x.PlannedQty > (x.BookQty*x.OrderBook.PlannedQty*x.OrderBook.Order.PlannedQty)).ToList();
            //.Select(v => new InProductionDto
            // {
            //     Component = _mapper.Map<Component, ComponentDto>(v.Component),
            //     OrderBook = _mapper.Map<OrderBook, OrderBookDto>(v.OrderBook),

            // })


            //.ThenInclude(b => b.Book)
            //.Where(c => c.Component.Number == component.Number
            //& c.PlannedQty > c.Component.BookComponents.Where(v => v.Book == c.OrderBook.Book && v.Component == c.Component).Select(bcc => bcc.Quantity).Single()).ToList();

            List<InProductionDto> componentsInProductionDto = _mapper.Map<List<InProduction>, List<InProductionDto>>(componentsInProduction);

            //componentsInProductionDto.ForEach(c =>
            //{
            //    c.SubOrderNumber = c.OrderBook.Number;
            //});

            return Ok(componentsInProductionDto);
        }

        //GET /api/components/id
        [HttpGet("{id}")]
        public IActionResult GetComponent(int id)
        {
            var componentInDb = _context.Components.SingleOrDefault(c => c.ComponentId == id);

            return Ok(componentInDb);
        }

        [HttpPost("ware")]
        //POST /api/components/ware
        public IActionResult AddWare(ComponentDto componentDto)
        {
            if (componentDto == null)
                return BadRequest();

            Component componentInDb = (AddComponent(componentDto) as OkObjectResult).Value as Component;

            componentInDb.ModifiedDate = DateTime.Now;

            if (componentInDb.Ware != null)
                return BadRequest();

            Ware wareInDb = (_warehouseController.AddWare(componentDto.Ware) as OkObjectResult).Value as Ware;

            componentInDb.Ware = wareInDb;
            componentInDb.WareLength = componentDto.Ware.Length;
            componentInDb.WareQuantity = componentDto.Ware.Quantity;
            componentInDb.WareUnit = componentDto.Ware.Unit;
            _context.SaveChanges();
            componentInDb = _context.Components.SingleOrDefault(c => c.Number == componentDto.Number);

            return Ok(componentInDb);
        }

        // Updates component, LastSourceType for now
        [HttpPatch]
        public IActionResult UpdateComponent(ComponentDto componentDto)
        {
            // Search for component in database or add it if it does not exist
            var number = componentDto.Number;
            var componentInDb = (AddComponent(componentDto) as OkObjectResult).Value as Component;
            
            // Update selected values
            componentInDb.LastSourceType = (sbyte)componentDto.LastSourceType;
            componentInDb.IsAdditionallyPurchasable = componentDto.IsAdditionallyPurchasable;

            // Save changes to component in database
            _context.SaveChanges();

            return Ok();
        }
            #endregion


            #region Methods

            #endregion


            #region Classes
            #endregion
        }
    }