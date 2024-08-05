using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SPMA.Data;
using SPMA.Dtos.Production;
using SPMA.Models.Production;
using Microsoft.EntityFrameworkCore;

namespace SPMA.Controllers.Production
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookComponentController : ControllerBase
    {
        #region Properties
        private ApplicationDbContext _context;
        private readonly IMapper _mapper;
        #endregion

        #region Constructor
        public BookComponentController(ApplicationDbContext dbContext, IMapper mapper)
        {
            _context = dbContext;
            _mapper = mapper;
        }
        #endregion

        #region Http Action Requests
        [HttpGet]
        public IActionResult GetBookComponent(int bookId, int componentId)
        {
            BookComponent bookComponent = _context.BookComponents.Where(bc => bc.BookId == bookId & bc.ComponentId == componentId)
                .OrderBy(x => x.Order).FirstOrDefault();
            BookComponentDto bookComponentDto = _mapper.Map<BookComponent, BookComponentDto>(bookComponent);
            return Ok(bookComponentDto);
        }

        [HttpGet("getcomponentnumber")]
        public IActionResult GetComponentNumber(string officenumber)
        {

            BookComponent bookComponent = _context.BookComponents
                 .Include(bc => bc.Book)
                 .Include(bc => bc.Component)
                 .Where(bc => bc.Book.OfficeNumber == officenumber &&
                              bc.Level==0)
                 .SingleOrDefault();


            return new JsonResult(bookComponent.Component.Number);
        }
        #endregion
    }
}