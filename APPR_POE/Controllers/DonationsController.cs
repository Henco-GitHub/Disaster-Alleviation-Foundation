using APPR_POE.Data;
using APPR_POE.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace APPR_POE.Controllers
{
    public class DonationsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public DonationsController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }
      

        public IActionResult ViewUserMonetary()
        {
            int userID = (int)HttpContext.Session.GetInt32("id");
            var userDonations = _db.tblMonetaryDonations.Where(x => x.userID == userID).ToList();

            return View(userDonations);
        }

        public IActionResult ViewUserGoods()
        {
            int userID = (int)HttpContext.Session.GetInt32("id");
            var userDonations = _db.tblGoodsDonations.Where(x => x.userID == userID).ToList();

            return View(userDonations);
        }



        public IActionResult Monetary()
        {
            return View();
        }



        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Monetary(monetaryDonations obj, IFormCollection fc)
        {
            try
            {
                monetaryDonations m = new monetaryDonations();

                //get checkBox result
                string result = fc["anonymousDonation"];

                //get current user's name
                int userID = (int)HttpContext.Session.GetInt32("id");
                var currentUser = _db.tblUser.Where(x => x.userID == userID).Select(x => x.userName).FirstOrDefault();
                

                //check for anonymous donation
                if (result == "on")
                {
                    m.userName = "Anonymous";
                }
                else
                {
                    m.userName = currentUser;
                }

                //add monetary donation
                m.donationDate = DateTime.Now;
                m.donationAmount = obj.donationAmount;
                m.userID = (int)HttpContext.Session.GetInt32("id");



                _db.tblMonetaryDonations.Add(m);
                _db.SaveChanges();

                //add to wallet
                wallet w = new wallet();

                w.amount = obj.donationAmount;
                _db.tblWallet.Add(w);

                //add to cash flow
                cashFlow c = new cashFlow();
                c.date = DateTime.Now;
                c.amount = obj.donationAmount;
                c.activity = "donation";

                _db.tblCashFlow.Add(c);

                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return RedirectToAction("ViewUserMonetary", "Donations");
        }

      

        public IActionResult Goods()
        {
            //populate list with all categories from DB
            var inputList = _db.tblGoodsDonations.ToList();
            var outputList = new List<string>();

            foreach (var item in inputList)
            {
                if (!outputList.Contains(item.goodsCategory))
                {
                    outputList.Add(item.goodsCategory);
                }
            }

            //set list to viewbag
            ViewBag.Categories = outputList;
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public object Goods(goodsDonations obj, IFormCollection fc)
        {
            try
            {
                goodsDonations g = new goodsDonations();

                //get checkBox result
                string result = fc["anonymousGoodsDonation"];

                //get current user's name
                int userID = (int)HttpContext.Session.GetInt32("id");
                var currentUser = _db.tblUser.Where(x => x.userID == userID).Select(x => x.userName).FirstOrDefault();


                //check for anonymous donation
                if (result == "on")
                {
                    g.userName = "Anonymous";
                }
                else
                {
                    g.userName = currentUser;
                }

                //insert into tblGoodsDonations
                g.goodsDonationDate = DateTime.Now;
                g.numberOfItems = obj.numberOfItems;
                g.goodsCategory = obj.goodsCategory;
                g.goodsDesc = obj.goodsDesc;

                g.userID = (int)HttpContext.Session.GetInt32("id");

                _db.tblGoodsDonations.Add(g);


                //
                //add to inventory
                //

                var inventoryCategories = _db.tblInventory.Where(x => x.goodsCategory == obj.goodsCategory).FirstOrDefault();

                //dont add category to inventory, just add amount (number of items)
                if (inventoryCategories != null)
                {
                    inventoryCategories.goodsAmount = inventoryCategories.goodsAmount + obj.numberOfItems;
                }
                //add category to inventory
                else
                {
                    //add to inventory
                    inventory i = new inventory();

                    i.goodsCategory = obj.goodsCategory;
                    i.goodsAmount = obj.numberOfItems;

                    _db.tblInventory.Add(i);
                }

                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return RedirectToAction("ViewUserGoods", "Donations");
        }
    }
}
