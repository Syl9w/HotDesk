using System;

namespace HotDesk.Dtos
{
    public class NewReservationDto
    {
        public int DeskId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
