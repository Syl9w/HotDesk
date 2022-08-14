using System;
using HotDesk.Models;
using Microsoft.EntityFrameworkCore;

namespace HotDesk.DataAccess
{
    public class HotDeskDbContext: DbContext 
    {
        public HotDeskDbContext(DbContextOptions<HotDeskDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Desk>().HasKey(p => p.DeskId);
            modelBuilder.Entity<Location>().HasKey(p => p.LocationId);
            modelBuilder.Entity<Reservation>().HasKey(p => p.ReservationId);
            modelBuilder.Entity<User>().HasKey(p => p.UserId);
        }
        public DbSet<Desk> Desks { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
