using APPR_POE.Models;
using Microsoft.EntityFrameworkCore;

namespace APPR_POE.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
           : base(options) { }

        public DbSet<users> tblUser { get; set; }

        public DbSet<admin> tblAdmin { get; set; }

        public DbSet<monetaryDonations> tblMonetaryDonations { get; set; }

        public DbSet<goodsDonations> tblGoodsDonations { get; set; }

        public DbSet<disaster> tblDisaster { get; set; }

        public DbSet<wallet> tblWallet { get; set; }
        public DbSet<inventory> tblInventory { get; set; }
        public DbSet<cashFlow> tblCashFlow { get; set; }
        public DbSet<goodsAllocated> tblGoodsAllocated { get; set; }



    }
}
