using System;
namespace HotDesk.Models.Dtos
{
    public class ReservationEmployeeViewDto
    {
        public int DeskId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
