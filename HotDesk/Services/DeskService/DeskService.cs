using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using HotDesk.DataAccess;
using HotDesk.Models;
using Microsoft.EntityFrameworkCore;
using HotDesk.Models.Dtos;

namespace HotDesk.Services.DeskService
{
    public class DeskService:IDeskService
    {
        private readonly HotDeskDbContext _context;

        public DeskService(HotDeskDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<List<Desk>>> AddDesk(NewDeskDto newDesk)
        {
            var response = new ServiceResponse<List<Desk>>();
            //check if location exists
            var location = _context.Locations.FirstOrDefault(l => l.LocationId == newDesk.LocationId);
            if(location == null)
            {
                response.Success = false;
                response.Message = "Location not found";
                return response;
            }
            var desk = new Desk
            {
                Location = location,
                Unavailable = false
            };
            await _context.Desks.AddAsync(desk);
            await _context.SaveChangesAsync();
            response.Data = await _context.Desks.ToListAsync();
            return response;
        }

        public async Task<ServiceResponse<List<Desk>>> DeleteDesk(int deskId)
        {
            var response = new ServiceResponse<List<Desk>>();
            //check if desk exists
            var deskToDelete = _context.Desks.FirstOrDefault(d => d.DeskId == deskId);
            if (deskToDelete == null)
            {
                response.Success = false;
                response.Message = $"Desk with id {deskId} not found!";
                return response;
            }
            //check if desk has reservations
            if (_context.Reservations.FirstOrDefault(r => r.Desk == deskToDelete && r.To > DateTime.UtcNow) != null)
            {
                response.Success = false;
                response.Message = "Desk has reservations!";
                return response;
            }

            _context.Desks.Remove(deskToDelete);
            await _context.SaveChangesAsync();
            response.Data = await _context.Desks.ToListAsync();
            return response;
        }

        public async Task<ServiceResponse<List<Desk>>> GetAll()
        {
            var response = new ServiceResponse<List<Desk>>();
            response.Data = await _context.Desks.ToListAsync();
            return response ;
        }

        public async Task<ServiceResponse<List<Desk>>> GetAvailable()
        {
            var desks =  
                from desk in _context.Desks
                where !desk.Unavailable
                select desk;

            var response = new ServiceResponse<List<Desk>>();
            response.Data = await desks.ToListAsync();
            return response;
        }

        public async Task<ServiceResponse<List<Desk>>> GetDesksOnLocation(int locationId)
        {
            var response = new ServiceResponse<List<Desk>>();
            var location = _context.Locations.FirstOrDefault(l => l.LocationId == locationId);
            if (location == null)
            {
                response.Success = false;
                response.Message = $"Location with id {locationId} not found!";
                return response;
            }
            var desks = _context.Desks.Where(d => d.Location == location);
            response.Data = await desks.ToListAsync();
            return response;
        }

        public async Task<ServiceResponse<List<Desk>>> GetUnAvailable()
        {
            var desks =
                from desk in _context.Desks
                where desk.Unavailable
                select desk;

            var response = new ServiceResponse<List<Desk>>();
            response.Data = await desks.ToListAsync();
            return response;
        }

        public async Task<ServiceResponse<Desk>> UpdateAvailability(UpdatedDesk updatedDesk)
        {

            var response = new ServiceResponse<Desk>();

            var currentDesk = await _context.Desks.FirstOrDefaultAsync(d => d.DeskId == updatedDesk.DeskId);
            if (currentDesk == null)
            {
                response.Message = $"Desk with id {updatedDesk.DeskId} not found!";
                response.Success = false;
                return response;
            }

            if (updatedDesk.Unavailable)
            {
                var reservations = await _context.Reservations.FirstOrDefaultAsync(r => r.Desk.DeskId == updatedDesk.DeskId && r.From > DateTime.UtcNow);
                if (reservations != null)
                {
                    response.Message = "The dest was reserved! You cannot make it Unavailable";
                    response.Success = false;
                    return response;
                }
            }
           

            currentDesk.Unavailable = updatedDesk.Unavailable;
            await _context.SaveChangesAsync();
            response.Data = currentDesk;
            return response;
        }
    }
}
