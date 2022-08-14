using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotDesk.DataAccess;
using HotDesk.Dtos;
using HotDesk.Models;
using HotDesk.Models.Dtos;
using HotDesk.Services.ReservationService;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HotDesk.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly HotDeskDbContext _context;
        private readonly IReservationService _reservationService;

        public ReservationController(HotDeskDbContext context, IReservationService reservationService)
        {
            _context = context;
            _reservationService = reservationService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Reservation>> GetMyReservations() =>
            Ok(_context.Reservations.Where(r => r.User.UserId.ToString() == User.Identity.Name));

        [HttpPost]
        public async Task<ActionResult<Reservation>> NewReservation([FromBody] NewReservationDto newReservation)
        {
            var user = _context.Users.First(x => x.UserId.ToString() == User.Identity.Name);

            return Ok(await _reservationService.NewReservation(user, newReservation));
        }

        [HttpPut]
        public async Task<ActionResult<Reservation>> UpdateDesk(UpdatedReservationDto updatedReservation)
        {
            var user = _context.Users.First(x => x.UserId.ToString() == User.Identity.Name);
            return Ok(await _reservationService.UpdateReservation(user, updatedReservation));
        }

        [HttpGet("{locationId}")]
        public async Task<ActionResult> GetReservationsOnLocation(int locationId)
        {
            var user = _context.Users.First(x => x.UserId.ToString() == User.Identity.Name);
            if(user.Role=="Admin")
            {
                return Ok(await _reservationService.GetReservationsOnLocationAdmin(locationId));
            }
            else
            {
                return Ok(await _reservationService.GetReservationsOnLocationEmployee(locationId));
            }
        }
    }
}
