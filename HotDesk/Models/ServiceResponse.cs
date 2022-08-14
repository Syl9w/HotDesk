using System;
using System.Collections.Generic;
using HotDesk.Models.Dtos;

namespace HotDesk.Models
{
    public class ServiceResponse<T>
    {
        public T Data { get; set; }
        public bool Success { get; set; } = true;
        public string Message { get; set; } = null;

        public static implicit operator ServiceResponse<T>(ServiceResponse<List<ReservationsAdministratorViewDto>> v)
        {
            throw new NotImplementedException();
        }

        public static implicit operator ServiceResponse<T>(List<ReservationsAdministratorViewDto> v)
        {
            throw new NotImplementedException();
        }
    }
}
