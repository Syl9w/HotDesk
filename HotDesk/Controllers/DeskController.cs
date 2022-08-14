using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotDesk.DataAccess;
using HotDesk.Models;
using HotDesk.Models.Dtos;
using HotDesk.Services.DeskService;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HotDesk.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeskController : ControllerBase
    {
        private readonly HotDeskDbContext _context;
        private readonly IDeskService _deskService;

        public DeskController(HotDeskDbContext context, IDeskService deskService)
        {
            _context = context;
            _deskService = deskService;
        }
        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<Desk>>>> GetAll() =>
            Ok(await _deskService.GetAll());

        [HttpGet("available")]
        public async Task<ActionResult<ServiceResponse<List<Desk>>>> GetAvailableAsync() =>
            Ok(await _deskService.GetAvailable());

        [HttpGet("unavailable")]
        public async Task<ActionResult<ServiceResponse<List<Desk>>>> GetUnavailable() =>
            Ok(await _deskService.GetUnAvailable());

        [HttpGet("{locationId}")]
        public async Task<ActionResult<ServiceResponse<List<Desk>>>> GetDesksOnLocation(int locationId) =>
            Ok(await _deskService.GetDesksOnLocation(locationId));

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<List<Desk>>>> AddDesk(NewDeskDto newDesk)
        {
            var response = new ServiceResponse<List<Desk>>();
            //check user role
            var user = _context.Users.First(x => x.UserId.ToString() == User.Identity.Name);
            if (user.Role != "Admin")
            {
                response.Success = false;
                response.Message = "You are not an Administrator";
                return response;
            }
            
            return Ok(await _deskService.AddDesk(newDesk));
        }

        [HttpDelete("{deskId}")]
        public async Task<ActionResult<ServiceResponse<List<Desk>>>> DeleteDesk(int deskId)
        {
            var response = new ServiceResponse<List<Desk>>();
            //check user role
            var user = _context.Users.First(x => x.UserId.ToString() == User.Identity.Name);
            if (user.Role != "Admin")
            {
                response.Success = false;
                response.Message = "You are not an Administrator";
                return BadRequest(response);
            }

            return Ok(await _deskService.DeleteDesk(deskId));
        }

        [HttpPut]
        public async Task<ActionResult<ServiceResponse<Desk>>> UpdateAvailability(UpdatedDesk updatedDesk)
        {
            //check user role
            var user = _context.Users.First(x => x.UserId.ToString() == User.Identity.Name);
            if (user.Role != "Admin")
                return BadRequest("You are not an Administrator");

            
            return Ok(await _deskService.UpdateAvailability(updatedDesk));
        }
    }
}
