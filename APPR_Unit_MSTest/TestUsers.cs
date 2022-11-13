using APPR_POE.Controllers;
using APPR_POE.Data;
using APPR_POE.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace APPR_Unit_MSTest
{
    [TestClass]
    public class TestUsers
    {
        private readonly ApplicationDbContext _context;

        public TestUsers()
        {
            var serviceProvider = new ServiceCollection()
            .AddEntityFrameworkInMemoryDatabase()
            .BuildServiceProvider();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .UseInternalServiceProvider(serviceProvider)
                .Options;

            _context = new ApplicationDbContext(options);
            _context.Database.EnsureCreated();

            //popualte database with appropriate records (stored locally in memory)
            PopulateDatabase();
        }

        //TEST: login users (role: approved)
        [TestMethod]
        public void UserController_Login_Success()
        {
            //--Arrange--
            //test users
            //approved user
            users approvedUser = new users();
            approvedUser.userEmail = "henco@gmail.com";
            approvedUser.userPassword = "q";

            UserController userController = new UserController(_context);

            //--Assert--
            //test for valid user that is approved
            Assert.AreEqual(true, userController.AttempUserLogin(approvedUser.userEmail, approvedUser.userPassword));
        }


        //TEST: login users (role: rejected, pending)
        [TestMethod]
        public void UserController_Login_Fail()
        {
            //--Arrange--
            //test users

            //rejected user
            users rejectedUser = new users();
            rejectedUser.userEmail = "test456@gmail.com";
            rejectedUser.userPassword = "test456";

            //pending user
            users pendingUser = new users();
            pendingUser.userEmail = "test123@gmail.com";
            pendingUser.userPassword = "test123";

            //approved user with wrong password
            users userWrongPass = new users();
            userWrongPass.userEmail = "test123@gmail.com";
            userWrongPass.userPassword = "test12";

            UserController userController = new UserController(_context);

            //--Assert--
            //test for valid user that is rejected
            Assert.AreEqual(false, userController.AttempUserLogin(rejectedUser.userEmail, rejectedUser.userPassword));

            //test for valid user that is pending
            Assert.AreEqual(false, userController.AttempUserLogin(pendingUser.userEmail, pendingUser.userPassword));

            //test for valid user with wrong password
            Assert.AreEqual(false, userController.AttempUserLogin(userWrongPass.userEmail, userWrongPass.userPassword));
        }


        //TEST: succesfully register a new user
        [TestMethod]
        public void UserController_Register_Success()
        {
            //--Arrange--

            //new user to register
            users userRegister = new users();
            userRegister.userName = "Bill";
            userRegister.userSurname = "Fence";
            userRegister.userEmail = "billfence@gmail.com";
            userRegister.userAge = 90;
            userRegister.userPassword = "pass";
            userRegister.userPasswordConfirm = "pass";

            UserController userController = new UserController(_context);

            //--Assert--

            //check that user can register with correct details (password and confirm passwords are the same)
            Assert.AreEqual(true, userController.registerSuccessful(userRegister));
        }

        
        //populate local DB with appropriate records (stored in memory)
        private void PopulateDatabase()
        {
            //--user creation--

            //approved user
            users approvedUser = new users();
            approvedUser.userID = 1;
            approvedUser.userName = "Henco";
            approvedUser.userSurname = "Barkhuizen";
            approvedUser.userAge = 21;
            approvedUser.userEmail = "henco@gmail.com";
            approvedUser.userPassword = "at8DH0DvWQybsTHpm3q9Hg==";
            approvedUser.userRole = "Approved";

            //rejected user
            users rejectedUser = new users();
            rejectedUser.userID = 2;
            rejectedUser.userName = "test456";
            rejectedUser.userSurname = "789";
            rejectedUser.userAge = 23;
            rejectedUser.userEmail = "test456@gmail.com";
            rejectedUser.userPassword = "PTTp/QMBCLK920UfelOptg==";
            rejectedUser.userRole = "Rejected";

            //pending user
            users pendingUser = new users();
            pendingUser.userID = 3;
            pendingUser.userName = "test123";
            pendingUser.userSurname = "456";
            pendingUser.userAge = 17;
            pendingUser.userEmail = "test123@gmail.com";
            pendingUser.userPassword = "r0Eg2vJPfk1psmncxD+Jaw==";
            pendingUser.userRole = "pending";

            //add users to context
            _context.Add(approvedUser);
            _context.Add(rejectedUser);
            _context.Add(pendingUser);

            //save local DB
            _context.SaveChanges();
        }
    }
}
