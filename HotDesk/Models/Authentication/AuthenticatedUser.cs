using System;
namespace HotDesk.Models.Authentication
{
    public class AuthenticatedUser
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Token { get; set; }
    }
}
