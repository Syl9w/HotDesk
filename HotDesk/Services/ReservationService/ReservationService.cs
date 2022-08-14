using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotDesk.DataAccess;
using HotDesk.Dtos;
using HotDesk.Models;
using HotDesk.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace HotDesk.Services.ReservationService
{
    public class ReservationService: IReservationService
    {
        private readonly HotDeskDbContext _context;

        public ReservationService(HotDeskDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<List<ReservationsAdministratorViewDto>>> GetReservationsOnLocationAdmin(int locationId)
        {
            var location = await _context.Locations.FirstOrDefaultAsync(l => l.LocationId == locationId);
            var response = new ServiceResponse<List<ReservationsAdministratorViewDto>>();
            if (location == null)
            {
                response.Message = "Location not found";
                response.Success = false;
                return response;
            }
            var reservations =
                from reservation in _context.Reservations
                where reservation.Desk.Location.LocationId == locationId
                select (new ReservationsAdministratorViewDto
                {
                    DeskId = reservation.Desk.DeskId,
                    UserId = reservation.User.UserId,
                    From = reservation.From,
                    To = reservation.To
                });

            response.Data = await reservations.ToListAsync();
            return response;
        }

        public async Task<ServiceResponse<List<ReservationEmployeeViewDto>>> GetReservationsOnLocationEmployee(int locationId)
        {
            var location = await _context.Locations.FirstOrDefaultAsync(l => l.LocationId == locationId);
            var response = new ServiceResponse<List<ReservationEmployeeViewDto>>();
            if (location == null)
            {
                response.Message = "Location not found";
                response.Success = false;
                return response;
            }

            var reservations =
                from reservation in _context.Reservations
                where reservation.Desk.Location.LocationId == locationId
                select (new ReservationEmployeeViewDto
                {
                    DeskId = reservation.Desk.DeskId,
                    From = reservation.From,
                    To = reservation.To
                });
            response.Data = await reservations.ToListAsync();
            return response;
        }

        public async Task<ServiceResponse<Reservation>> NewReservation(User user, NewReservationDto newReservation)
        {
            var response = new ServiceResponse<Reservation>();
            //check if desk is available
            var desk = await _context.Desks.FirstOrDefaultAsync(d => d.DeskId == newReservation.DeskId);
            if (desk == null || desk.Unavailable)
            { 
                response.Message = "Desk does not exist or unavailable";
                response.Success = false;
                return response;
            }

            //check duration of the new reservation
            if ((newReservation.To - newReservation.From).TotalDays > 7)
            {
                response.Message = "Too long period of reservation! Please choose shorter period";
                response.Success = false;
                return response;
            }
            //check if reservation date is future date and if end of reservation is after the beggining date
            if (newReservation.From > newReservation.To || newReservation.From < DateTime.UtcNow)
            {
                response.Message = "Please choose correct start and end date";
                response.Success = false;
                return response;
            }


            //check if desk is available for this period
            var reservations =
                from reservation in _context.Reservations
                where reservation.Desk == desk
                && !reservation.Cancelled
                && reservation.To > newReservation.From
                && reservation.From < newReservation.To
                select reservation;
            if (reservations.ToList().Count > 0)
            {
                response.Message = "Your period overlaps reserved periods!";
                response.Success = false;
                return response;
            }

            var res = new Reservation
            {
                Desk = desk,
                From = newReservation.From,
                To = newReservation.To,
                User = user,
                Cancelled = false
            };

            await _context.Reservations.AddAsync(res);
            await _context.SaveChangesAsync();
            response.Data = res;
            return response;

        }

        public async Task<ServiceResponse<Reservation>> UpdateReservation(User user, UpdatedReservationDto updatedReservation)
        {
            var response = new ServiceResponse<Reservation>();

            var res = await _context.Reservations.FirstOrDefaultAsync(r => r.ReservationId == updatedReservation.ReservationId);
            if (res == null)
            {
                response.Message = "Reservation does not exist!";
                response.Success = false;
                return response;
            }
               
            if (res.User != user)
            {
                response.Message = "You are not owner of the reservation!";
                response.Success = false;
                return response;
            }
                
            var desk = await _context.Desks.FirstOrDefaultAsync(d => d.DeskId == updatedReservation.DeskId);
            if (desk == null || desk.Unavailable)
            {
                response.Message = "Desk does not exists or unavailable!";
                response.Success = false;
                return response;
            }
           

            if ((res.From - DateTime.UtcNow).Days <= 1)
            {
                response.Message = "Too late to change desk!";
                response.Success = false;
                return response;
            }

            var reservations =
                from reservation in _context.Reservations
                where reservation.Desk == desk
                && !reservation.Cancelled
                && reservation.To > res.From
                && reservation.From < res.To
                select reservation;
            if (reservations.ToList().Count > 0)
            {
                response.Message = "Your period overlaps reserved periods!";
                response.Success = false;
                return response;
            }

            res.Desk = desk;
            await _context.SaveChangesAsync();
            response.Data = res;
            return response;
        }
    }
}
