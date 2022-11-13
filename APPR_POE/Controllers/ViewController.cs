using APPR_POE.Data;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace APPR_POE.Controllers
{
    public class ViewController : Controller
    {

        private readonly ApplicationDbContext _context;

        public ViewController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult ViewMonetary()
        {
            var monetaryDonations = _context.tblMonetaryDonations.ToList();
            return View(monetaryDonations);
        }

        public IActionResult ViewGoods()
        {
            var goodsDonations = _context.tblGoodsDonations.ToList();
            return View(goodsDonations);
        }

        public IActionResult ViewDisasters()
        {
            var disaster = _context.tblDisaster.ToList();
            return View(disaster);
        }
    }
}
