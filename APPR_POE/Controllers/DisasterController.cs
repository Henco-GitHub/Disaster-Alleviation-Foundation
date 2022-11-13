using APPR_POE.Data;
using APPR_POE.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace APPR_POE.Controllers
{
    public class DisasterController : Controller
    {
        private readonly ApplicationDbContext _db;

        public DisasterController(ApplicationDbContext db)
        {
            _db = db;
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ViewUserDisasters()
        {
            int userID = (int)HttpContext.Session.GetInt32("id");
            var userDisasters = _db.tblDisaster.Where(x => x.userID == userID).ToList();


            return View(userDisasters);
        }


        public IActionResult CaptureDisaster()
        {
            return View();
        }


        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CaptureDisaster(disaster obj)
        {
            if (ModelState.IsValid)
            {
                obj.userID = (int)HttpContext.Session.GetInt32("id");
                _db.tblDisaster.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("ViewUserDisasters", "Disaster");
            }
            return RedirectToAction("CaptureDisaster");
        }
    }
}
