using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SPMA.Data;
using SPMA.Dtos.Production;
using SPMA.Models.Production;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPMA.Controllers.Production
{
    public class ReservationController : Controller
    {
        #region Properties
        private ApplicationDbContext _context;
        private readonly IMapper _mapper;
        #endregion


        #region Constructor
        public ReservationController(ApplicationDbContext dbContext,
            IMapper mapper)
        {
            _context = dbContext;
            _mapper = mapper;
        }
        #endregion

        [HttpGet]
        public IActionResult GetReservation(int reservationId)
        {
            ReservedItem reservation = _context.ReservedItems
                .Where(r => r.ReservedItemId == reservationId)
                .FirstOrDefault();

            _context.SaveChanges();

            return Ok(reservation);
        }

        [HttpPost("add")]
        public IActionResult AddReservation(ReservedItemDto reservationDto)
        {
            if(reservationDto == null)
            {
                BadRequest("sended object is null");
            }

            ReservedItem reservation = _mapper.Map<ReservedItemDto, ReservedItem>(reservationDto);

            _context.ReservedItems.Add(reservation);
            _context.SaveChanges();

            return Ok(reservation);
        }

        [HttpDelete("delete")]
        public IActionResult DeleteReservation(int reservationId)
        {
            ReservedItem reservation = _context.ReservedItems
                .Where(r => r.ReservedItemId == reservationId)
                .FirstOrDefault();

            _context.ReservedItems.Remove(reservation);
            _context.SaveChanges();

            return Ok(reservation.ReservedItemId);
        }

    }
}
