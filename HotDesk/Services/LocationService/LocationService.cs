using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotDesk.DataAccess;
using HotDesk.Models;
using HotDesk.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace HotDesk.Services.LocationService
{
    public class LocationService:ILocationService
    {
        private readonly HotDeskDbContext _context;

        public LocationService(HotDeskDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<IEnumerable<Location>>> AddLocation(NewLocationDto newLocation)
        {
            var location = new Location { LocationName = newLocation.LocationName };
            await _context.Locations.AddAsync(location);
            await _context.SaveChangesAsync();
            var response = new ServiceResponse<IEnumerable<Location>>();
            response.Data = _context.Locations;
            return response;
        }

        public async Task<ServiceResponse<IEnumerable<Location>>> DeleteLocation(int locationId)
        {
            var response = new ServiceResponse<IEnumerable<Location>>();
            var locationToDelete = await _context.Locations.FirstOrDefaultAsync(l => l.LocationId == locationId);
            if (locationToDelete == null)
            {
                response.Message = "Locatoion does not exist";
                response.Success = false;
                return response;
            }

            var desks = await _context.Desks.FirstOrDefaultAsync(d => d.Location == locationToDelete);
            if (desks != null)
            {
                response.Message = "Location has a desk!";
                response.Success = false;
                return response;
            }

            _context.Locations.Remove(locationToDelete);
            await _context.SaveChangesAsync();
            response.Data = _context.Locations;
            return response;
        }

        public async Task<ServiceResponse<IEnumerable<Location>>> GetAllLocations()
        {
            var response = new ServiceResponse<IEnumerable<Location>>();
            response.Data =  _context.Locations;
            return response;
        }
    }
}
