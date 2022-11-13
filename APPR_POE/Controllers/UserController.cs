using APPR_POE.Data;
using APPR_POE.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace APPR_POE.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;

        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Public()
        {
            //get list of ACTIVE disasters
            var today = DateTime.Today;
            var getDisasters = _db.tblDisaster.Where(x => x.endDate >= today).ToList();

            //get total monetary donations
            var totalMonetary = _db.tblMonetaryDonations.Select(x => x.donationAmount).Sum();

            //get total number of goods donations
            var totalNumberOfGoods = _db.tblGoodsDonations.Select(x => x.numberOfItems).Sum();

            //wallet higest
            double highestAmount = _db.tblWallet.Max(x => x.amount as double?) ?? 0.0;

            //wallet lowest
            double lowestAmount = _db.tblWallet.Where(x => x.amount >= 1).Min(x => x.amount as double?) ?? 0.0;

            //highest goods donation 
            var highestGoodsDonation = 0;
            try
            {
                highestGoodsDonation = _db.tblGoodsDonations.Select(x => x.numberOfItems).Max();
            }
            catch (Exception e)
            {
                highestGoodsDonation = 0;
            }


            //lowest goods donation 
            var lowestGoodsDonation = 0;
            try
            {
                lowestGoodsDonation = _db.tblGoodsDonations.Select(x => x.numberOfItems).Min();
            }
            catch (Exception e)
            {
                lowestGoodsDonation = 0;
            }


            //set goods allocated to disasters to viewbag
            var getGoods = _db.tblGoodsAllocated.ToList();



            ViewBag.totalMonetary = totalMonetary.ToString("#,##0.00");
            ViewBag.totalNumberOfGoods = totalNumberOfGoods;
            ViewBag.highestAmount = highestAmount.ToString("#,##0.00");
            ViewBag.lowestAmount = lowestAmount.ToString("#,##0.00");
            ViewBag.highestGoodsDonation = highestGoodsDonation;
            ViewBag.lowestGoodsDonation = lowestGoodsDonation;
            ViewBag.goodsAllocated = getGoods;

            return View(getDisasters);
        }

        public IActionResult Home()
        {
            return View();
        }
        public IActionResult Logout()
        {

            return RedirectToAction("Login", "User");
        }




        //REGISTER
        //GET
        public IActionResult Register()
        {
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(users obj)
        {
            var result = registerSuccessful(obj);

            if (result = true)
            {
                return RedirectToAction("Login", "User");
            }
            else
            {
                return RedirectToAction("Register", "User");
            }
        }

        public bool registerSuccessful(users obj)
        {

            bool loop = true;

            while (loop = true)
            {
                if (ModelState.IsValid)
                {
                    users u = new users();

                    u.userName = obj.userName;
                    u.userSurname = obj.userSurname;
                    u.userAge = obj.userAge;
                    u.userEmail = obj.userEmail;

                    if (obj.userPassword.ToString().Equals(obj.userPasswordConfirm.ToString()))
                    {
                        u.userPassword = Encrypt(obj.userPassword);
                        u.userPasswordConfirm = Encrypt(obj.userPasswordConfirm);

                        u.userRole = "pending";

                        loop = false;

                        _db.tblUser.Add(u);
                        _db.SaveChanges();

                        return true;

                    }
                    else if (!obj.userPassword.ToString().Equals(obj.userPasswordConfirm.ToString()))
                    {
                        TempData["message"] = "Passwords do not match";
                        ViewBag.Message2 = String.Format("Passwords do not match");

                        return false;
                    }
                }
            }

            return true;

        }

        //LOGIN
        //GET
        public IActionResult Login()
        {
            return View();
        }

        //POST
        [HttpPost, ActionName("Login")]
        [ValidateAntiForgeryToken]
        public IActionResult LoginPOST(string? userEmail, string? userPassword)
        {
            var result = AttempUserLogin(userEmail, userPassword);

            if (result = false)
            {
                return Login();
            }
            else
            {
                users u = _db.tblUser.Where(x => x.userEmail == userEmail).FirstOrDefault();

                if (u != null)
                {
                    SetSessions(u);
                }

                return RedirectToAction("Home", "User");
            }


        }



        public bool AttempUserLogin(string userEmail, string userPassword)
        {
            var userFromDb = _db.tblUser.FirstOrDefault(x => x.userEmail == userEmail);

            if (userFromDb == null)
            {
                //user does not exist
                ViewBag.Message = String.Format("Please create an account first");
                return false;
                //return Login();
            }
            if (userPassword == null)
            {
                //password not entered
                return false;
                //return Login();
            }
            if (Decrypt(userFromDb.userPassword) != userPassword)
            {
                //passwords do not match
                ViewBag.Message = String.Format("Passwords do not match");
                return false;
                //return Login();
            }
            if (userFromDb.userRole == "pending" || userFromDb.userRole == "Rejected")
            {
                getTempDataForAccountPending();
                return false;
            }

            return true;
        }

        private void getTempDataForAccountPending()
        {
            ViewBag.userName = String.Format("Account approval is still pending");
        }

        private void SetSessions(users userFromDb)
        {
            HttpContext.Session.SetInt32("id", userFromDb.userID);
            HttpContext.Session.SetString("userName", userFromDb.userName);
            HttpContext.Session.SetString("email", userFromDb.userEmail);
            HttpContext.Session.SetString("password", userFromDb.userPassword);
            HttpContext.Session.SetString("auth", userFromDb.userRole);
        }


        //LOGIN AS ADMIN
        //GET
        public IActionResult AdminLogin()
        {
            return View();
        }

        private static string Encrypt(string clearText)
        {
            string encryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        private static string Decrypt(string cipherText)
        {
            string encryptionKey = "MAKV2SPBNI99212";
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(encryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

    }
}
