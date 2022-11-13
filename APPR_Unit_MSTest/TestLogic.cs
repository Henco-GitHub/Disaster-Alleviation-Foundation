using APPR_POE.Controllers;
using APPR_POE.Data;
using APPR_POE.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPR_Unit_MSTest
{

    [TestClass]
    public class TestLogic
    {
        private readonly ApplicationDbContext _context;

        public TestLogic()
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

        }


        //TEST: test that an admin user can allocate money to an active disaster
        [TestMethod]
        public void AdminController_AllocateMoney_Successful()
        {
            //--Arrange--

            //simulate a wallet
            wallet w = new wallet();
            w.walletId = 1;
            w.amount = 10000;

            int walletAmount = w.amount;
            int amountToAllocate = 1000;

            AdminController adminController = new AdminController(_context);

            //--Assert--
            Assert.AreEqual(true, adminController.allocateToDisaster(walletAmount, amountToAllocate));
        }

        //TEST: test that an admin user cannot allocate more money to a disaster than what is available
        [TestMethod]
        public void AdminController_AllocateMoney_Fail()
        {
            //--Arrange--

            //simulate a wallet
            wallet w = new wallet();
            w.walletId = 1;
            w.amount = 10000;

            int walletAmount = w.amount;
            int amountToAllocate = 100000;

            AdminController adminController = new AdminController(_context);

            //--Assert--
            Assert.AreEqual(false, adminController.allocateToDisaster(walletAmount, amountToAllocate));
        }


        //TEST: test that an admin user can allocate goods a specific disaster (enough available in inventory)
        [TestMethod]
        public void AdminController_AllocateGoods_Success()
        {
            //--Arrange--
            inventory i = new inventory();
            i.goodsId = 1;
            i.goodsCategory = "Clothes";
            i.goodsDescription = "just clothes";
            i.goodsAmount = 100;
            i.allocatedTo = "10";

            int inventoryAmount = i.goodsAmount;
            int amountOfGoodsToAllocate = 50;

            AdminController adminController = new AdminController(_context);


            //--Assert--
            Assert.AreEqual(true, adminController.allocateGoodsCheckInventory(i, amountOfGoodsToAllocate));
        }


        //TEST: test that an admin user cant allocate goods a specific disaster (not enough available in inventory)
        [TestMethod]
        public void AdminController_AllocateGoods_Fail()
        {
            //--Arrange--
            inventory i = new inventory();
            i.goodsId = 1;
            i.goodsCategory = "Clothes";
            i.goodsDescription = "just clothes";
            i.goodsAmount = 100;
            i.allocatedTo = "10";

            int inventoryAmount = i.goodsAmount;
            int amountOfGoodsToAllocate = 500;

            AdminController adminController = new AdminController(_context);


            //--Assert--
            Assert.AreEqual(false, adminController.allocateGoodsCheckInventory(i, amountOfGoodsToAllocate));
        }


        //TEST: test that admin can add to inventory
        [TestMethod]
        public void AdminController_AddToInventory_Success()
        {
            //--Arrange--
            inventory i = new inventory();
            i.goodsId = 1;
            i.goodsCategory = "Clothes";
            i.goodsDescription = "just clothes";
            i.goodsAmount = 100;
            i.allocatedTo = "10";

            string newCategory = "Food";
            string newDescription = "canned foods";

            AdminController adminController = new AdminController(_context);

            //--Assert--
            Assert.AreEqual(false, adminController.checkInventoryDefineNewCategory(newCategory, newDescription));
        }


        //TEST: check whether there are enough funds to purchase goods
        [TestMethod]
        public void AdminController_SufficientFunds_Success()
        {
            //--Arrange--
            int walletAmount = 10000;
            int totalCostOfPurchase = 100;

            AdminController adminController = new AdminController(_context);

            //--Assert--
            Assert.AreEqual(true, adminController.checkFundsToPurchaseGoods(walletAmount, totalCostOfPurchase));
        }


        //TEST: check whether there are enough funds to purchase goods (FAIL)
        [TestMethod]
        public void AdminController_SufficientFunds_Fail()
        {
            //--Arrange--
            int walletAmount = 10000;
            int totalCostOfPurchase = 100000;

            AdminController adminController = new AdminController(_context);

            //--Assert--
            Assert.AreEqual(false, adminController.checkFundsToPurchaseGoods(walletAmount, totalCostOfPurchase));
        }
    }
}
