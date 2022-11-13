using APPR_POE.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using APPR_POE.Models;

namespace APPR_Unit_XUnitTests
{
    public class TestModels
    {
        private readonly ApplicationDbContext _context;

        public TestModels()
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

        //TEST: Admin model returns type IEnumerable
        [Fact]
        public async Task Model_Admin_ReturnsCorrectType()
        {
            var admin = await _context.tblAdmin.ToListAsync();
            Assert.IsAssignableFrom<IEnumerable<admin>>(admin);
        }

        //TEST: cashFlow model returns type IEnumerable
        [Fact]
        public async Task Model_cashFlow_ReturnsCorrectType()
        {
            var cashFlow = await _context.tblCashFlow.ToListAsync();
            Assert.IsAssignableFrom<IEnumerable<cashFlow>>(cashFlow);
        }

        //TEST: disaster model returns type IEnumerable
        [Fact]
        public async Task Model_disaster_ReturnsCorrectType()
        {
            var disaster = await _context.tblDisaster.ToListAsync();
            Assert.IsAssignableFrom<IEnumerable<disaster>>(disaster);
        }

        //TEST: Admin goodsAllocated returns type IEnumerable
        [Fact]
        public async Task Model_goodsAllocated_ReturnsCorrectType()
        {
            var goodsAllocated = await _context.tblGoodsAllocated.ToListAsync();
            Assert.IsAssignableFrom<IEnumerable<goodsAllocated>>(goodsAllocated);
        }

        //TEST: goodsDonations model returns type IEnumerable
        [Fact]
        public async Task Model_goodsDonations_ReturnsCorrectType()
        {
            var goodsDonations = await _context.tblGoodsDonations.ToListAsync();
            Assert.IsAssignableFrom<IEnumerable<goodsDonations>>(goodsDonations);
        }

        //TEST: inventory model returns type IEnumerable
        [Fact]
        public async Task Model_inventory_ReturnsCorrectType()
        {
            var inventory = await _context.tblInventory.ToListAsync();
            Assert.IsAssignableFrom<IEnumerable<inventory>>(inventory);
        }

        //TEST: Admin monetaryDonations returns type IEnumerable
        [Fact]
        public async Task Model_monetaryDonations_ReturnsCorrectType()
        {
            var monetaryDonations = await _context.tblMonetaryDonations.ToListAsync();
            Assert.IsAssignableFrom<IEnumerable<monetaryDonations>>(monetaryDonations);
        }

        //TEST: users model returns type IEnumerable
        [Fact]
        public async Task Model_users_ReturnsCorrectType()
        {
            var users = await _context.tblUser.ToListAsync();
            Assert.IsAssignableFrom<IEnumerable<users>>(users);
        }

        //TEST: wallet model returns type IEnumerable
        [Fact]
        public async Task Model_wallet_ReturnsCorrectType()
        {
            var wallet = await _context.tblWallet.ToListAsync();
            Assert.IsAssignableFrom<IEnumerable<wallet>>(wallet);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }


    }
}
