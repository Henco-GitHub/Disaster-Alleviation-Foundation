using APPR_POE.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace APPR_Unit_Tests
{
    public class UnitTest1
    {


        //db context || mimic data
        private async Task<ApplicationDbContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var databaseContext = new ApplicationDbContext(options);
            databaseContext.Database.EnsureCreated();
            return databaseContext;
        }


        [Test]
        public async Task TestInventory_NotNull()
        {
            //Arrange




            //Act





            //Assert




        }

    }
}
