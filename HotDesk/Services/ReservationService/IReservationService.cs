using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HotDesk.Dtos;
using HotDesk.Models;
using HotDesk.Models.Dtos;

namespace HotDesk.Services.ReservationService
{
    public interface IReservationService
    {
        public Task<ServiceResponse<Reservation>> NewReservation(User user, NewReservationDto newReservation);
        public Task<ServiceResponse<Reservation>> UpdateReservation(User user, UpdatedReservationDto updatedReservation);
        public Task<ServiceResponse<List<ReservationEmployeeViewDto>>> GetReservationsOnLocationEmployee(int locationId);
        public Task<ServiceResponse<List<ReservationsAdministratorViewDto>>> GetReservationsOnLocationAdmin(int locationId);

    }
}
