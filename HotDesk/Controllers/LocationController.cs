using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotDesk.DataAccess;
using HotDesk.Models;
using HotDesk.Models.Dtos;
using HotDesk.Services.LocationService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HotDesk.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly HotDeskDbContext _context;
        private readonly ILocationService _locationService;

        public LocationController(HotDeskDbContext context, ILocationService locationService)
        {
            _context = context;
            _locationService = locationService;
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<IEnumerable<Location>>>> GetAll() =>
            Ok(await _locationService.GetAllLocations());

        
        [HttpPost]
        public async Task<ActionResult<IEnumerable<Location>>> AddLocation(NewLocationDto newLocation)
        {
            return Ok(await _locationService.AddLocation(newLocation));
        }

        [HttpDelete("{locationId}")]
        public async Task<ActionResult<IEnumerable<Location>>> DeleteLocation(int locationId)
        {
            //check user role
            var user = await _context.Users.FirstAsync(x => x.UserId.ToString() == User.Identity.Name);
            if (user.Role != "Admin")
                return BadRequest("You are not an Administrator!");

            return Ok(await _locationService.DeleteLocation(locationId));
        }
        
    }
}
