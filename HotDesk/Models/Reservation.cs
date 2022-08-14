using System;
namespace HotDesk.Models
{
    public class Reservation
    {
        public int ReservationId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public bool Cancelled { get; set; }
        public User User { get; set; }
        public Desk Desk { get; set; }
    }
}
