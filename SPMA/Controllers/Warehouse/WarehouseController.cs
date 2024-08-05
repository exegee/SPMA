using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SPMA.Data;
using SPMA.Dtos.Warehouse;
using SPMA.Models.Warehouse;
using System.Collections.Generic;
using System.Linq;

namespace SPMA.Controllers.Warehouse
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehouseController : ControllerBase
    {
        #region Properties
        private ApplicationDbContext _context;
        private readonly IMapper _mapper;
        #endregion

        #region Constructor
        public WarehouseController(ApplicationDbContext dbContext, IMapper mapper)
        {
            _context = dbContext;
            _mapper = mapper;
        }
        #endregion

        #region Http Action Requests
        [HttpGet]
        //GET /api/warehouse
        public IActionResult GetWares()
        {
            var wares = _context.Wares.Select(_mapper.Map<Ware, WareDto>).ToList();

            return Ok(wares);
        }

        [HttpPost("addware")]
        //POST /api/warehouse
        public IActionResult AddWare(WareDto wareDto)
        {
            if (wareDto == null)
                return BadRequest();

            var wareInDb = _context.Wares.SingleOrDefault(w => w.Code == wareDto.Code);

            if (wareInDb != null)
                return Ok(wareInDb);

            var ware = _mapper.Map<WareDto, Ware>(wareDto);

            _context.Wares.Add(ware);
            _context.SaveChanges();

            wareInDb = _context.Wares.SingleOrDefault(w => w.Code == wareDto.Code);

            return Ok(wareInDb);
        }

        [HttpPost("addwares")]
        //POST /api/warehouse
        public IActionResult AddWares(WareDto[] wareDtos)
        {
            if (wareDtos == null)
                return NotFound();
            //Add all wares into the database if they do not exist
            foreach (WareDto wareDto in wareDtos)
            {
                var wareInDb = _context.Wares.FirstOrDefault(c => c.Code == wareDto.Code);

                if (wareInDb == null)
                {
                    _context.Wares.Add(_mapper.Map<WareDto, Ware>(wareDto));
                }
            }
            if (_context.ChangeTracker.HasChanges())
                _context.SaveChanges();

            return Ok();
        }


        #endregion
    }
}