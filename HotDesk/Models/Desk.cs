using System;
namespace HotDesk.Models
{
    public class Desk
    {
        public int DeskId { get; set; }
        public Location Location { get; set; }
        public bool Unavailable { get; set; }
    }
}
