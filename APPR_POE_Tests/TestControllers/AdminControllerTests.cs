using APPR_POE.Controllers;
using APPR_POE.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPR_POE_Tests.TestControllers
{
    [TestClass]
    public class AdminControllerTests
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


        [TestMethod]
        public async Task AdminController_AllocateMoney_Success()
        {
            //--Arrange--

            var dbContext = await GetDbContext();
            var adminController = new AdminController(dbContext);

            string amount = "1000";  //amount user wants to allocate
            int walletAmount = 5000;  //wallet amount  
            string disID = "1";  //disaster ID to allocate to

            //--Act--
            var result = (RedirectToRouteResult)adminController.AllocateMoney(amount, disID);

            //--Assert--
            Assert.AreEqual("Allocate", result.RouteValues["action"]);  //test correct redirect



        }
    }
}
