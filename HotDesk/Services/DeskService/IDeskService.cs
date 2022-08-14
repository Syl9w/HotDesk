using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HotDesk.DataAccess;
using HotDesk.Models;
using HotDesk.Models.Dtos;

namespace HotDesk.Services.DeskService
{
    public interface IDeskService
    {
        Task<ServiceResponse<List<Desk>>> GetAll();
        Task<ServiceResponse<List<Desk>>> GetAvailable();
        Task<ServiceResponse<List<Desk>>> GetUnAvailable();
        Task<ServiceResponse<List<Desk>>> GetDesksOnLocation(int locationId);
        Task<ServiceResponse<List<Desk>>> AddDesk(NewDeskDto newDesk);
        Task<ServiceResponse<List<Desk>>> DeleteDesk(int deskId);
        Task<ServiceResponse<Desk>> UpdateAvailability(UpdatedDesk updatedDesk);
    }
}
