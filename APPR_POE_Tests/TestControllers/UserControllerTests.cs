using APPR_POE.Controllers;
using APPR_POE.Data;
using APPR_POE.Models;
using EllipticCurve.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace APPR_POE_Tests.TestControllers
{
    [TestClass]
    public class UserControllerTests
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
        public async Task UserController_Register_Success()
        {

            //--Arrange--
            var actualUser = new users()
            {
                userID = 1,
                userName = "TestName",
                userSurname = "TestSurname",
                userAge = 21,
                userEmail = "test@gmail.com",
                userPassword = "pass",
                userPasswordConfirm = "pass",
                userRole = "pending"
            };
            var dbContext = await GetDbContext();
            var userController = new UserController(dbContext);

            //--Act--
            var result = userController.Register(actualUser);
            var type = result.GetType();
            var stringTest = "string";
            var ar = userController.Register() as ViewResult;
            int integerTest = 1;

            //actual values
            var actualUserName = actualUser.userName;
            var actualUserSurname = actualUser.userSurname;
            var actualUserAge = actualUser.userAge;
            var actualUserEmail = actualUser.userEmail;
            var actualUserPassword = actualUser.userPassword;
            var actualUserPasswordConfirm = actualUser.userPasswordConfirm;
            var actualuserRole = actualUser.userRole;



            //--Assert--

            //check that result is not null
            Assert.IsNotNull(result);

            //check that method returns RedirectActionMethod
            Assert.AreEqual(type.ToString(), "Microsoft.AspNetCore.Mvc.RedirectToActionResult");

            //check that userAge is of int type
            Assert.AreEqual(integerTest.GetType(), actualUserAge.GetType());

            //check that userName, userSurname, userEmail, userPassword, userPasswordConfirm and userRole are of string type
            Assert.AreEqual(stringTest.GetType(), actualUserName.GetType());
            Assert.AreEqual(stringTest.GetType(), actualUserSurname.GetType());
            Assert.AreEqual(stringTest.GetType(), actualUserEmail.GetType());
            Assert.AreEqual(stringTest.GetType(), actualUserPassword.GetType());
            Assert.AreEqual(stringTest.GetType(), actualUserPasswordConfirm.GetType());
            Assert.AreEqual(stringTest.GetType(), actualuserRole.GetType());

            //Assert.AreEqual("Passwords match", userController.ViewData["message"]);
        }

    }
}