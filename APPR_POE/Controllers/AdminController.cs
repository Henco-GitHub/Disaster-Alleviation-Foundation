using APPR_POE.Data;
using APPR_POE.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Superpower;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection.Metadata;

namespace APPR_POE.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AdminController(ApplicationDbContext db)
        {
            _db = db;
        }


        public IActionResult Index()
        {
            return View();
        }

        //ADMIN LOGIN
        //GET
        public IActionResult AdminLogin()
        {
            return View();
        }


        //POST
        [HttpPost, ActionName("AdminLogin")]
        [ValidateAntiForgeryToken]
        public IActionResult AdminLoginPOST(string? adminEmail, string? adminPassword)
        {
            var adminFromDb = _db.tblAdmin.FirstOrDefault(x => x.adminEmail == adminEmail);

            if (adminFromDb == null)
            {
                //user does not exist
                return AdminLogin();
            }
            if (adminPassword == null)
            {
                //invalid password
                return AdminLogin();
            }
            if (adminFromDb.adminPassword != adminPassword)
            {
                //passwords do not match
                return AdminLogin();
            }

            HttpContext.Session.SetString("admin", "admin");
            return RedirectToAction("Allocate", "Admin");
        }

        //ADMIN DASHBOARD
        //GET
        public IActionResult AdminDashboard()
        {
            var users = _db.tblUser.ToList();

            var currentFunds = _db.tblWallet.Select(x => x.amount).Sum();

            ViewBag.currentFunds = currentFunds;

            return View(users);

        }



        //admin approves user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Approve(int? userID, users obj)
        {
            if (userID == null)
            {
                return RedirectToAction("AdminDashboard");
            }

            var user = _db.tblUser.Where(u => u.userID == userID).FirstOrDefault();
            user.userRole = "Approved";
            _db.SaveChanges();

            return RedirectToAction("AdminDashboard");
        }


        //admin rejects user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reject(int? userID)
        {
            if (userID == null)
            {
                return RedirectToAction("AdminDashboard");
            }

            //Code to Approve User
            var user = _db.tblUser.Where(u => u.userID == userID).FirstOrDefault();
            user.userRole = "Rejected";
            _db.SaveChanges();

            //return to dashboard
            return RedirectToAction("AdminDashboard");
        }


        public IActionResult Allocate()
        {
            //get list of ACTIVE disasters
            var today = DateTime.Today;
            var disasters = _db.tblDisaster.Where(x => x.endDate >= today).ToList();

            //get goodsCategories
            //populate list with all categories from DB

            var inputList = _db.tblInventory.ToList();
            var outputList = new List<string>();

            foreach (var item in inputList)
            {
                if (!outputList.Contains(item.goodsCategory))
                {
                    outputList.Add(item.goodsCategory + "\t(" + item.goodsAmount + " left)");
                }
            }
            var currentFunds = _db.tblWallet.Select(x => x.amount).Sum();
            ViewBag.currentFunds = currentFunds;

            //set list to viewbag
            ViewBag.Categories = outputList;

            //set goods allocated to disasters to viewbag
            var getGoods = _db.tblGoodsAllocated.ToList();

            ViewBag.goodsAllocated = getGoods;

            return View(disasters);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AllocateMoney(string? amount, string? disID)
        {
            if (String.IsNullOrWhiteSpace(amount) || String.IsNullOrWhiteSpace(disID))
            {
                return View();
            }

            int id = Convert.ToInt32(disID);
            int getAmount = (int)Convert.ToInt64(amount);

            var allocateToTbl = _db.tblDisaster.Where(u => u.disasterID == id).FirstOrDefault();

            //get total from wallet
            var getWallet = _db.tblWallet.Select(x => x.amount).Sum();

            disaster d = new disaster();
            wallet w = new wallet();
            int walletAmount = getWallet;


            var result = allocateToDisaster(getWallet, getAmount);


            if (result == false)
            {
                SetSessionForInsufficientFunds();
                return RedirectToAction("Allocate");
            }
            else
            {

                allocateToTbl.moneyAllocated = allocateToTbl.moneyAllocated + getAmount;

                //subtract from wallet
                w.amount = -getAmount;
                _db.tblWallet.Add(w);

                //add to cash flow
                cashFlow c = new cashFlow();
                c.date = DateTime.Now;
                c.amount = getAmount;
                c.activity = "outgoing";

                _db.tblCashFlow.Add(c);

                _db.SaveChanges();


                return RedirectToAction("Allocate");
            }
        }

        public bool allocateToDisaster(int getWallet, int getAmount)
        {
            if (getAmount > getWallet)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void SetSessionForInsufficientFunds()
        {
            HttpContext.Session.SetString("insufficientFunds", "1");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AllocateGoods(string? goodsCat, int? amountOfGoods, string? disID)
        {
            //get goods selected by user from front-end
            string category = goodsCat.ToString();

            int index = category.IndexOf("\t");
            if (index >= 0)
                category = category.Substring(0, index);

            //get amount selected by user from front-end
            int amount = Convert.ToInt32(amountOfGoods);

            int id = Convert.ToInt32(disID);

            var inventory = checkInventoryNotNull(category);

            var result = allocateGoodsCheckInventory(inventory, amount);

            //if available
            if (result == true)
            {
                //subtract amount from inventory
                inventory.goodsAmount = inventory.goodsAmount - amount;
                inventory.allocatedTo = inventory.allocatedTo + "," + id.ToString();

                //allocate goodsName and goodsAmount (add, not just set) to disaster table
                var updateDisasterTable = _db.tblDisaster.Where(u => u.disasterID == id).FirstOrDefault();
                updateDisasterTable.goodsAllocated = category + "-" + amount + "," + updateDisasterTable.goodsAllocated;


                //write to goodsAllocated table
                var getGoodsAllocatedTable = _db.tblGoodsAllocated.Where(x => x.goodsCategory == category && x.disasterIdAllocatedTo == id).FirstOrDefault();

                if (getGoodsAllocatedTable == null)
                {
                    getGoodsAllocatedTable = new goodsAllocated();
                    //add new record
                    getGoodsAllocatedTable.goodsCategory = category;
                    getGoodsAllocatedTable.goodsAmount = amount;
                    getGoodsAllocatedTable.disasterIdAllocatedTo = id;

                    _db.tblGoodsAllocated.Add(getGoodsAllocatedTable);
                }
                else
                {
                    //update
                    getGoodsAllocatedTable.goodsAmount = getGoodsAllocatedTable.goodsAmount + amount;

                    _db.tblGoodsAllocated.Update(getGoodsAllocatedTable);
                }

                _db.SaveChanges();
            }
            //if not available
            else
            {
                //notify user that the amount that the user wants to allocate is not available
                TempData["amountNotAvailable"] = "1";
                return RedirectToAction("Allocate");
            }

            _db.SaveChanges();
            return RedirectToAction("Allocate");
        }


        public bool allocateGoodsCheckInventory(inventory inventory, int amount)
        {
            //if available
            if (inventory != null && inventory.goodsAmount >= amount)
            {
                return true;
            }
            //if not available
            else
            {
                return false;
            }
        }


        public inventory checkInventoryNotNull(string category)
        {
            //check that the amount the user wants to allocate, is available
            var getInventory = _db.tblInventory.Where(x => x.goodsCategory == category).FirstOrDefault();

            return getInventory;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddToInventory(string? selectedGoods, string? newCategoryOfGoods, string? newDesc, int? goodsAmount, int? itemCost)
        {
            #region inputs

            //get goods selected by user from front-end
            string category = selectedGoods;

            //get new goods category by user from front-end
            string newCategory = newCategoryOfGoods;

            //get new goods description by user from front-end
            string newDescription = newDesc;

            //get amount selected by user from front-end
            int amount = Convert.ToInt32(goodsAmount);

            //get cost per item from front-end
            int costPerItem = Convert.ToInt32(itemCost);

            #endregion

            try
            {
                var checkCategory = checkInventoryDefineNewCategory(newCategory, newDescription);

                //if user defines new category
                if (checkCategory == true)
                {
                    //check if category already exists

                    //get goodsCategory list from Inventory
                    var goodsList = _db.tblInventory.Select(x => x.goodsCategory).ToList();

                    if (goodsList.Contains(newCategory))
                    {
                        TempData["categoryExists"] = "1";
                        return RedirectToAction("Allocate");
                    }
                    else
                    {
                        //add to inventory
                        inventory i = new inventory();

                        i.goodsCategory = newCategory;
                        i.goodsDescription = newDescription;
                        i.goodsAmount = amount;

                        _db.tblInventory.Add(i);


                        //subract from wallet
                        //total cost
                        int totalCost = amount * costPerItem;

                        int getWallet = _db.tblWallet.Select(x => x.amount).Sum();


                        var checkFunds = checkFundsToPurchaseGoods(getWallet, totalCost);

                        //check funds
                        if (checkFunds == true)
                        {
                            //sufficient funds
                            int walletAmount = getWallet;

                            //subtract from wallet
                            wallet w = new wallet();
                            w.amount = -totalCost;
                            _db.tblWallet.Add(w);


                            //add to cash flow
                            cashFlow c = new cashFlow();
                            c.date = DateTime.Now;
                            c.amount = totalCost;
                            c.activity = "outgoing";

                            _db.tblCashFlow.Add(c);

                            _db.SaveChanges();
                        }
                        else
                        {
                            TempData["insufficientFundsToPurchaseGoods"] = "1";
                            return RedirectToAction("Allocate");
                        }


                    }
                }
                //if user uses a category from the list given
                else
                {
                    //add amount of items to the specific category
                    var getCategory = _db.tblInventory.FirstOrDefault(x => x.goodsCategory == selectedGoods);
                    getCategory.goodsAmount = getCategory.goodsAmount + amount;


                    //subract from wallet
                    //total cost
                    int totalCost = amount * costPerItem;

                    var getWallet = _db.tblWallet.Select(x => x.amount).Sum();



                    //check funds
                    if (getWallet >= totalCost)
                    {
                        //sufficient funds
                        int walletAmount = getWallet;

                        wallet w = new wallet();
                        w.amount = -totalCost;
                        _db.tblWallet.Add(w);


                        //add to cash flow
                        cashFlow c = new cashFlow();
                        c.date = DateTime.Now;
                        c.amount = totalCost;
                        c.activity = "outgoing";

                        _db.tblCashFlow.Add(c);

                        _db.SaveChanges();
                    }
                    else
                    {

                        TempData["insufficientFundsToPurchaseGoods"] = "1";
                        return RedirectToAction("Allocate");
                    }


                    _db.SaveChanges();
                }

                return RedirectToAction("ViewInventory");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return RedirectToAction("ViewInventory");
        }



        public bool checkFundsToPurchaseGoods(int getWallet, int totalCost)
        {
            if (getWallet >= totalCost)
            {
                //sufficient funds
                return true;
            }
            else
            {
                return false;
            }
        }


        public bool checkInventoryDefineNewCategory(string newCategory, string newDescription)
        {
            if (newCategory != null && newDescription != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public IActionResult ViewHistory()
        {
            #region get values from db
            //wallet total
            double currentFunds = _db.tblWallet.Select(x => x.amount).Sum();

            //wallet higest
            double highestAmount = _db.tblWallet.Max(x => x.amount as double?) ?? 0.0;

            //wallet lowest
            double lowestAmount = _db.tblWallet.Where(x => x.amount >= 1).Min(x => x.amount as double?) ?? 0.0;

            //monetary donations list
            var monetaryDonationsList = _db.tblMonetaryDonations.ToList().OrderByDescending(x => x.donationDate);

            //goods donations list
            var goodsDonationsList = _db.tblGoodsDonations.ToList().OrderByDescending(x => x.goodsDonationDate);
            #endregion

            //set Viewbag values
            ViewBag.currentFunds = currentFunds;
            ViewBag.highestAmount = highestAmount;
            ViewBag.lowestAmount = lowestAmount;

            ViewBag.latestMonetaryDonation = monetaryDonationsList.FirstOrDefault();
            ViewBag.latestGoodsDonation = goodsDonationsList.FirstOrDefault();

            ViewBag.mList = monetaryDonationsList;
            ViewBag.gList = goodsDonationsList;

            return View();

        }

        public IActionResult FullBreakdown()
        {
            //get current funds (for navbar)
            var currentFunds = _db.tblWallet.Select(x => x.amount).Sum();
            ViewBag.currentFunds = currentFunds;

            var getCashFlow = _db.tblCashFlow.ToList().OrderByDescending(x => x.date);

            ViewBag.cashFlow = getCashFlow;

            return View(getCashFlow);
        }


        public IActionResult ViewInventory()
        {
            //wallet total
            var currentFunds = _db.tblWallet.Select(x => x.amount).Sum();
            var inventory = _db.tblInventory.ToList();

            ViewBag.inventory = inventory;
            //get goodsCategories
            //populate list with all categories from DB

            var inputList = _db.tblInventory.ToList();
            var outputList = new List<string>();

            foreach (var item in inputList)
            {
                if (!outputList.Contains(item.goodsCategory))
                {
                    outputList.Add(item.goodsCategory);
                }
            }

            ViewBag.currentFunds = currentFunds;

            //set list to viewbag
            ViewBag.Categories = outputList;
            return View();
        }
    }
}
