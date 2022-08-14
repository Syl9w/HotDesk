using Xunit;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using HotDesk.DataAccess;
using HotDesk.Models;
using HotDesk.Services.DeskService;
using HotDesk.Models.Dtos;

namespace HotDeskTest
{
    public class DeskServiceTests
    {
        private readonly DbContextOptions<HotDeskDbContext> dbOptions = new DbContextOptionsBuilder<HotDeskDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

        //GetAll
        [Fact]
        public async Task  TestGetAll_WithExistingDesks_ReturnsDesks()
        {
            //arrange
            
            using (var db = new HotDeskDbContext(dbOptions))
            {              
                var desk1 = new Desk
                { DeskId = 1, Location = null, Unavailable = false };

                db.Set<Desk>().Add(desk1);
                await db.SaveChangesAsync();

                var service = new DeskService(db);

                //act
                var result = await service.GetAll();

                //assert
                Assert.NotEmpty(result.Data);
                Assert.Null(result.Message);
                Assert.True(result.Success);
            }
        }

        //GetAvailable
        [Fact]
        public async Task TestGetAvailable_WithoutAvailableDesks_ReturnsEmptyData()
        {
            //arrange
            using (var db = new HotDeskDbContext(dbOptions))
            {                
                var desk1 = new Desk
                { DeskId = 1, Location = null, Unavailable = true };
                                
                db.Set<Desk>().Add(desk1);
                await db.SaveChangesAsync();

                var service = new DeskService(db);

            //act
                var result = await service.GetAvailable();

            //assert
                Assert.Empty(result.Data);
                Assert.Null(result.Message);
                Assert.True(result.Success);
            }
        }

        [Fact]
        public async Task TestGetAvailable_WithAvailableDesks_ReturnsNotEmptyData()
        {
            //arrange
            using (var db = new HotDeskDbContext(dbOptions))
            {
                var desk1 = new Desk
                { DeskId = 1, Location = null, Unavailable = false };

                db.Set<Desk>().Add(desk1);
                await db.SaveChangesAsync();

                var service = new DeskService(db);

                //act
                var result = await service.GetAvailable();

                //assert
                Assert.NotEmpty(result.Data);
                Assert.Null(result.Message);
                Assert.True(result.Success);
            }
        }

        //GetUnAvailable
        [Fact]
        public async Task TestGetUnavailable_WithoutUnavailableDesks_ReturnsEmptyData()
        {
            //arrange
            using (var db = new HotDeskDbContext(dbOptions))
            {
                var desk1 = new Desk
                { DeskId = 1, Location = null, Unavailable = false };

                db.Set<Desk>().Add(desk1);
                await db.SaveChangesAsync();

                var service = new DeskService(db);

                //act
                var result = await service.GetUnAvailable();

                //assert
                Assert.Empty(result.Data);
                Assert.Null(result.Message);
                Assert.True(result.Success);
            }
        }

        [Fact]
        public async Task TestGetUnavailable_WithUnavailableDesks_ReturnsNotEmptyData()
        {
            //arrange
            using (var db = new HotDeskDbContext(dbOptions))
            {
                var desk1 = new Desk
                { DeskId = 1, Location = null, Unavailable = true };

                db.Set<Desk>().Add(desk1);
                await db.SaveChangesAsync();

                var service = new DeskService(db);

                //act
                var result = await service.GetUnAvailable();

                //assert
                Assert.NotEmpty(result.Data);
                Assert.Null(result.Message);
                Assert.True(result.Success);
            }
        }

        //GetDeskOnLocation
        [Fact]
        public async Task TestGetDeskOnLocation_WithExistingLocationWithDesk_ReturnsNotEmptyData()
        {
            using (var db = new HotDeskDbContext(dbOptions))
            {
                var location1 = new Location
                { LocationId = 1, LocationName = "Main Hall" };
                var desk1 = new Desk
                { DeskId = 1, Location = location1, Unavailable = true };

                db.Set<Location>().Add(location1);
                db.Set<Desk>().Add(desk1);
                await db.SaveChangesAsync();

                var service = new DeskService(db);

                //act
                var result = await service.GetDesksOnLocation(1);

                //assert
                Assert.NotEmpty(result.Data);
                Assert.Null(result.Message);
                Assert.True(result.Success);
            }
        }

        [Fact]
        public async Task TestGetDeskOnLocation_WithExistingLocationWithoutDesk_ReturnsEmptyData()
        {
            using (var db = new HotDeskDbContext(dbOptions))
            {
                var location1 = new Location
                { LocationId = 1, LocationName = "Main Hall" };
                
                db.Set<Location>().Add(location1);
            
                await db.SaveChangesAsync();

                var service = new DeskService(db);

                //act
                var result = await service.GetDesksOnLocation(1);

                //assert
                Assert.Empty(result.Data);
                Assert.Null(result.Message);
                Assert.True(result.Success);
            }
        }

        [Fact]
        public async Task TestGetDeskOnLocation_WithoutExistingLocation_ReturnsFailureWithMessage()
        {
            using (var db = new HotDeskDbContext(dbOptions))
            {
                var service = new DeskService(db);
                //act
                var result = await service.GetDesksOnLocation(1);

                //assert
                Assert.Null(result.Data);
                Assert.Equal($"Location with id 1 not found!", result.Message);
                Assert.False(result.Success);
            }
        }

        //AddDesk
        [Fact]
        public async Task TestAddDesk_WithExistingLocation_ReturnsDesksList()
        {
            using (var db = new HotDeskDbContext(dbOptions))
            {
                var location1 = new Location
                { LocationId = 1, LocationName = "Main Hall" };
                db.Set<Location>().Add(location1);
                await db.SaveChangesAsync();
                var service = new DeskService(db);

                var newDesk = new NewDeskDto { LocationId = 1 };
                var desk = new Desk { DeskId = 1, Location = location1, Unavailable = false };
                //act
                var result = await service.AddDesk(newDesk);

                //assert
                Assert.NotEmpty(result.Data);
                Assert.Null(result.Message);
                Assert.True(result.Success);
            }
        }

        [Fact]
        public async Task TestAddDesk_WithoutExistingLocation_ReturnsFailureWithMessage()
        {
            using (var db = new HotDeskDbContext(dbOptions))
            {
                var service = new DeskService(db);
                var newDesk = new NewDeskDto { LocationId = 1 };
                //act
                var result = await service.AddDesk(newDesk);

                //assert
                Assert.Null(result.Data);
                Assert.Equal("Location not found", result.Message);
                Assert.False(result.Success);
            }
        }

        //DeleteDesk
        [Fact]
        public async Task TestDeleteDesk_WithExistingDesk_ReturnsEmptyList()
        {
            using (var db = new HotDeskDbContext(dbOptions))
            {
                var desk1 = new Desk
                { DeskId = 1, Location = null, Unavailable = true };

                db.Set<Desk>().Add(desk1);
                await db.SaveChangesAsync();

                var service = new DeskService(db);

                //act
                var result = await service.DeleteDesk(1);

                //assert
                Assert.Empty(result.Data);
                Assert.Null(result.Message);
                Assert.True(result.Success);
            }
        }

        [Fact]
        public async Task TestDeleteDesk_WithNonExistingDesk_ReturnsFailureWithMessage()
        {
            using (var db = new HotDeskDbContext(dbOptions))
            {                
                var service = new DeskService(db);

                //act
                var result = await service.DeleteDesk(1);

                //assert
                Assert.Null(result.Data);
                Assert.Equal("Desk with id 1 not found!", result.Message);
                Assert.False(result.Success);
            }
        }

        [Fact]
        public async Task TestDeleteDesk_WithDeskWithFutureReservation_ReturnsFailureWithMessage()
        {
            using (var db = new HotDeskDbContext(dbOptions))
            {
                var desk1 = new Desk
                { DeskId = 1, Location = null, Unavailable = true };
                var reservarion1 = new Reservation
                {
                    ReservationId = 1,
                    Cancelled = false,
                    Desk = desk1,
                    From = new DateTime(2023, 08, 12),
                    To = new DateTime(2023, 08, 13),
                    User = null
                };
                db.Set<Reservation>().Add(reservarion1);
                db.Set<Desk>().Add(desk1);
                await db.SaveChangesAsync();

                var service = new DeskService(db);

                //act
                var result = await service.DeleteDesk(1);

                //assert
                Assert.Null(result.Data);
                Assert.Equal("Desk has reservations!", result.Message);
                Assert.False(result.Success);
            }
        }

        [Fact]
        public async Task TestDeleteDesk_WithDeskWithPastReservation_ReturnsEmptyData()
        {
            using (var db = new HotDeskDbContext(dbOptions))
            {
                var desk1 = new Desk
                { DeskId = 1, Location = null, Unavailable = true };
                var reservarion1 = new Reservation
                {
                    ReservationId = 1,
                    Cancelled = false,
                    Desk = desk1,
                    From = new DateTime(2021, 08, 12),
                    To = new DateTime(2021, 08, 13),
                    User = null
                };
                db.Set<Reservation>().Add(reservarion1);
                db.Set<Desk>().Add(desk1);
                await db.SaveChangesAsync();

                var service = new DeskService(db);

                //act
                var result = await service.DeleteDesk(1);

                //assert
                Assert.Empty(result.Data);
                Assert.Null(result.Message);
                Assert.True(result.Success);
            }
        }

        //UpdateAvailability
        [Fact]
        public async Task TestUpdateAvailability_WithExistingDesk_ReturnsUpdatedDesk()
        {
            using (var db = new HotDeskDbContext(dbOptions))
            {
                var desk1 = new Desk
                { DeskId = 1, Location = null, Unavailable = true };

                db.Set<Desk>().Add(desk1);
                await db.SaveChangesAsync();
                var updatedDesk = new UpdatedDesk { DeskId = 1, Unavailable = false };
                var service = new DeskService(db);

                //act
                var result = await service.UpdateAvailability(updatedDesk);

                //assert
                Assert.NotNull(result.Data);
                Assert.Null(result.Message);
                Assert.True(result.Success);
            }
        }

        [Fact]
        public async Task TestUpdateAvailability_WithNonExistingDesk_ReturnsFailureWithMessage()
        {
            using (var db = new HotDeskDbContext(dbOptions))
            {
                
                var updatedDesk = new UpdatedDesk { DeskId = 1, Unavailable = false };
                var service = new DeskService(db);

                //act
                var result = await service.UpdateAvailability(updatedDesk);

                //assert
                Assert.Null(result.Data);
                Assert.Equal("Desk with id 1 not found!",result.Message);
                Assert.False(result.Success);
            }
        }

        [Fact]
        public async Task TestUpdateAvailability_WithExistingFutureReservation_ReturnsFailureWithMessage()
        {
            using (var db = new HotDeskDbContext(dbOptions))
            {
                var desk1 = new Desk
                { DeskId = 1, Location = null, Unavailable = false };
                var reservarion1 = new Reservation
                {
                    ReservationId = 1,
                    Cancelled = false,
                    Desk = desk1,
                    From = new DateTime(2023, 08, 12),
                    To = new DateTime(2023, 08, 13),
                    User = null
                };
                db.Set<Reservation>().Add(reservarion1);
                db.Set<Desk>().Add(desk1);
                await db.SaveChangesAsync();
                var updatedDesk = new UpdatedDesk { DeskId = 1, Unavailable = true };
                var service = new DeskService(db);

                //act
                var result = await service.UpdateAvailability(updatedDesk);

                //assert
                Assert.Null(result.Data);
                Assert.Equal("The dest was reserved! You cannot make it Unavailable", result.Message);
                Assert.False(result.Success);
            }
        }

        [Fact]
        public async Task TestUpdateAvailability_WithExistingPastReservation_ReturnsUpdatedDesk()
        {
            using (var db = new HotDeskDbContext(dbOptions))
            {
               
                var desk1 = new Desk
                { DeskId = 1, Location = null, Unavailable = false };
                var reservarion1 = new Reservation
                {
                    ReservationId = 1,
                    Cancelled = false,
                    Desk = desk1,
                    From = new DateTime(2021, 08, 12),
                    To = new DateTime(2021, 08, 13),
                    User = null
                };
                db.Set<Reservation>().Add(reservarion1);
                db.Set<Desk>().Add(desk1);
                await db.SaveChangesAsync();
                var updatedDesk = new UpdatedDesk { DeskId = 1, Unavailable = true };
                var service = new DeskService(db);

                //act
                var result = await service.UpdateAvailability(updatedDesk);

                //assert
                Assert.NotNull(result.Data);
                Assert.Null( result.Message);
                Assert.True(result.Success);
            }
        }
    }
}
