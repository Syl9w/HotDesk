using System;
using System.Threading.Tasks;
using HotDesk.DataAccess;
using HotDesk.Models;
using HotDesk.Models.Dtos;
using HotDesk.Services.LocationService;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HotDeskTest
{
    public class LocationServiceTests
    {
        private readonly DbContextOptions<HotDeskDbContext> dbOptions = new DbContextOptionsBuilder<HotDeskDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

        //GetAllLocations
        [Fact]
        public async Task TestGetAllLocations_WithExistingLocation_ReturnsLocations()
        {
            //arrange

            using (var db = new HotDeskDbContext(dbOptions))
            {
                var location1 = new Location
                { LocationId = 1, LocationName = "Main Hall" };
                db.Set<Location>().Add(location1);
                await db.SaveChangesAsync();
                var service = new LocationService(db);

                //act
                var result = await service.GetAllLocations();

                //assert
                Assert.NotEmpty(result.Data);
                Assert.Null(result.Message);
                Assert.True(result.Success);
            }
        }
        [Fact]
        public async Task TestGetAllLocations_WithEmptyLocation_ReturnsEmptyData()
        {
            //arrange

            using (var db = new HotDeskDbContext(dbOptions))
            {
                var service = new LocationService(db);

                //act
                var result = await service.GetAllLocations();

                //assert
                Assert.Empty(result.Data);
                Assert.Null(result.Message);
                Assert.True(result.Success);
            }
        }

        //AddLocation
        [Fact]
        public async Task TestAddLocation_WithData_ReturnLocations()
        {
            using (var db = new HotDeskDbContext(dbOptions))
            {
                var service = new LocationService(db);

                var newLocation = new NewLocationDto { LocationName="Hall"};

                //act
                var result = await service.AddLocation(newLocation);

                //assert
                Assert.NotEmpty(result.Data);
                Assert.Null(result.Message);
                Assert.True(result.Success);
            }
        }

        //DeleteLocation
        [Fact]
        public async Task TestDeleteLocation_WithExistingLocationWithoutDesk_ReturnsEmptyData()
        {
            using (var db = new HotDeskDbContext(dbOptions))
            {
                var location1 = new Location
                { LocationId = 1, LocationName = "Main Hall" };
                db.Set<Location>().Add(location1);
                await db.SaveChangesAsync();
                var service = new LocationService(db);

                //act
                var result = await service.DeleteLocation(1);

                //assert
                Assert.Empty(result.Data);
                Assert.Null(result.Message);
                Assert.True(result.Success);
            }
        }

        [Fact]
        public async Task TestDeleteLocation_WithExistingLocationWithDesk_ReturnsFailureWithMessage()
        {
            using (var db = new HotDeskDbContext(dbOptions))
            {
                var location1 = new Location
                { LocationId = 1, LocationName = "Main Hall" };
                db.Set<Location>().Add(location1);
                var desk1 = new Desk
                { DeskId = 1, Location = location1, Unavailable = false };
                db.Set<Desk>().Add(desk1);
                await db.SaveChangesAsync();
                var service = new LocationService(db);

                //act
                var result = await service.DeleteLocation(1);

                //assert
                Assert.Null(result.Data);
                Assert.Equal("Location has a desk!", result.Message);
                Assert.False(result.Success);
            }
        }

        [Fact]
        public async Task TestDeleteLocation_WithNonExistingLocation_ReturnsFailureWithMessage()
        {
            using (var db = new HotDeskDbContext(dbOptions))
            {
                var service = new LocationService(db);

                //act
                var result = await service.DeleteLocation(1);

                //assert
                Assert.Null(result.Data);
                Assert.Equal("Locatoion does not exist", result.Message);
                Assert.False(result.Success);
            }
        }
    }
}
