using System;
using HotDesk.Models;
using Microsoft.EntityFrameworkCore;

namespace HotDesk.DataAccess
{
    public static class DataMock
    {
        public static HotDeskDbContext GetDbContextWithData()
        {
            var options = new DbContextOptionsBuilder<HotDeskDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new HotDeskDbContext(options);
            var user1 = new User
            {
                UserId = 1,
                Email = "SAkhmetkaliyev",
                FirstName = "Sultan",
                LastName = "Akhmetkaliyev",
                Role = "Admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123")
            };
            var user2 = new User
            {
                UserId = 2,
                Email = "EMusk",
                FirstName = "Elon",
                LastName = "Musk",
                Role = "Employee",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123")
            };
            var user3 = new User
            {
                UserId = 3,
                Email = "JBezos",
                FirstName = "Jeff",
                LastName = "Bezos",
                Role = "Employee",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123")
            };
            var location1 = new Location
            { LocationId = 1, LocationName = "Main Hall" };
            var location2 = new Location
            { LocationId = 2, LocationName = "Conference Hall" };
            var location3 = new Location
            { LocationId = 3, LocationName = "Coworking Center" };
            var desk1 = new Desk
            { DeskId = 1, Location = location1, Unavailable=false};
            var desk2 = new Desk
            { DeskId = 2, Location = location2, Unavailable=true };
            var reservarion1 = new Reservation
            {
                ReservationId = 1,
                Cancelled = false,
                Desk = desk1,
                From = new DateTime(2022, 08, 12),
                To = new DateTime(2022, 08, 13),
                User=user2
            };
            context.Users.Add(user1);
            context.Users.Add(user2);
            context.Users.Add(user3);
            context.Locations.Add(location1);
            context.Locations.Add(location2);
            context.Locations.Add(location3);
            context.Desks.Add(desk1);
            context.Desks.Add(desk2);
            context.Reservations.Add(reservarion1);
            context.SaveChanges();
            return context;
        }
    }
}
