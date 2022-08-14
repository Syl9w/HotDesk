using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HotDesk.Models;
using HotDesk.Models.Dtos;

namespace HotDesk.Services.LocationService
{
    public interface ILocationService
    {
        Task<ServiceResponse<IEnumerable<Location>>> GetAllLocations();
        Task<ServiceResponse<IEnumerable<Location>>> AddLocation(NewLocationDto newLocation);
        Task<ServiceResponse<IEnumerable<Location>>> DeleteLocation(int locationId);
    }
}
