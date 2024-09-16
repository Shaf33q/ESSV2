using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ESSV2.Models;

namespace ESSV2.Controllers
{
    public class HomeController : Controller
    {
        //Start GLOBALS
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
        SqliteDataClass objsqlite = new SqliteDataClass();
        //End GLOBALS
        [HttpGet]
        public ActionResult Login()
        {
            ViewBag.LoginError = "";
            return View();
        }
        [HttpPost]
        public ActionResult Login(UserAccounts user)
        {
            ViewBag.LoginError = "";
           
            //var PublicIP = objsqlite.GetMacID.SingleOrDefault(u => u.MacID == "1231231");
            //if (PublicIP != null)
            //{
            var usr = objsqlite.Employees.SingleOrDefault(u => (u.Username == user.Username || u.Email == user.Username) && u.Password == user.Password);

            if (usr != null)
            {
                Session["UserID"] = usr.UserID;
                string path = Server.MapPath("~/ProfileImg/P" + Session["UserID"] + ".jpg");
                var CheckClockin = objsqlite.Attendance.SingleOrDefault(a => a.UserID == usr.UserID && a.Clockout == null && a.ClockDate.ToString("yyyy-MM-dd") == indianTime.ToString("yyyy-MM-dd"));
                if (CheckClockin != null)
                {
                    Session["Logid"] = CheckClockin.LogID;
                    Session["Checkin"] = true;
                }
                else
                {
                    var Logid = indianTime.ToString("yyMMddHHmm") + Session["UserID"].ToString();
                    Session["Logid"] = Logid;
                    Session["Checkin"] = false;
                }
                Session["UserName"] = usr.Username;
                Session["Role"] = usr.Role;
                Session["Fisrt"] = 0;
                Session["Pic"] = System.IO.File.Exists(path) ? "../../ProfileImg/P" + Session["UserID"] + ".jpg" : "../../ProfileImg/nobody.jpg";
                TempData["Notifaction"] = "Welcome "+ usr.Username+"/";
                return RedirectToAction("ClockTime", "Pages");
            }
            else
            {
                ViewBag.LoginError= "UserName or Password is Incorrect!!!";
                return View();
            }
            //}
            //    else
            //    {
            //        ModelState.AddModelError("", "You are not Authorizred to Login With this Computer" + GetIPAddress());

            //    }
           
        }
        
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login", "Home");

        }

        [HttpGet]
        public ActionResult Register(int? id)
        {
            if (Session["UserID"] != null && Session["Role"].ToString() == "Admin")
            {
                if (id!=null)
                {
                    UserAccounts employee = objsqlite.Employees.SingleOrDefault(a => a.UserID == id);

                    return View(employee);

                }
                else {
                    return View();
                }
                
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
           

        }

        [HttpPost]
        public ActionResult Register(UserAccounts account)
        {
            
            if (Session["UserID"] != null && Session["Role"].ToString() == "Admin")
            {
                if (ModelState.IsValid)
                {
                    var NoRegistererror = objsqlite.RegisterUser(account.Email, account.Username, account.Password, account.Role, indianTime.ToString("yyyy-MM-dd"));

                    if (NoRegistererror == true)
                    {
                        ViewBag.Message = "<div class='callout callout-success'><p><i class='fa fa-check'></i> " + account.Username + " Registered Successfully !!!</p></div>";
                        return View();
                    }
                    else
                    {
                        ViewBag.Message = "<div class='callout callout-danger'><p><i class='fa fa-warning'></i> Username/E-mail Already Taken !!!</p></div>";
                        return View();
                    }
                }
                
                else
                {
                    ViewBag.Message = "";
                    return View();
                }
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }
        [HttpGet]
        public ActionResult EditUser(int id)
        {
            if (Session["UserID"] != null && Session["Role"].ToString() == "Admin")
            {

                UserAccounts employee = objsqlite.Employees.SingleOrDefault(a => a.UserID == id);

                return View(employee);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }



        }
        [HttpPost]
        public ActionResult EditUser(UserAccounts account)
        {
            if (Session["UserID"] != null && Session["Role"].ToString() == "Admin")
            {
                if (ModelState.IsValid)
                {
                    var NoRegistererror = objsqlite.UpdateUser(account.UserID, account.Email, account.Username, account.Password, account.Role);
                    if (NoRegistererror == true)
                    {


                        ViewBag.Message = "<div class='callout callout-success'><p><i class='fa fa-check'></i> " + account.Username + " Edited Successfully !!!</p></div>"; 

                    }
                    else
                    {
                        ViewBag.Message = "<div class='callout callout-danger'><p><i class='fa fa-warning'></i> Error Editing Account !!!</p></div>";
                        
                    }
                   
                }
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }



        }
        public ActionResult ViewUser(string Absenties)
        {
            if (Session["UserID"] != null && Session["Role"].ToString() == "Admin")
            {
                List<UserAccounts> employee;
                if (Absenties=="True")
                {
                    employee = objsqlite.Absenties.ToList();
                    ViewData["Type"] = "Absenties";
                }
                else
                {
                    employee = objsqlite.Employees.ToList();
                    ViewData["Type"] = "Users";
                }
                

                return View(employee);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }
        public ActionResult DeleteUser(int id)
        {

            objsqlite.DeleteUser(id);
            return RedirectToAction("ViewUser", "Home");


        }
        [HttpGet]
        public ActionResult Routers()
        {
            if (Session["UserID"] != null && Session["Role"].ToString() == "Admin")
            {
                var model = new AllCompData();
                model.CompDatae = objsqlite.GetRouters.ToList();
                return View(model);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }
        [HttpPost]
        public ActionResult Routers(AllCompData AllCompDat)
        {
            if (Session["UserID"] != null && Session["Role"].ToString() == "Admin")
            {
                if (ModelState.IsValid)
                {
                    var Req = objsqlite.AddIP(AllCompDat.CompData.IP.ToString(), AllCompDat.CompData.Device.ToString());

                }

                AllCompDat.CompDatae = objsqlite.GetRouters.ToList();
                if (AllCompDat.CompDatae.Count() == 0)
                {
                    AllCompDat.CompDatae = Enumerable.Empty<ComputerDat>();
                }
                return View(AllCompDat);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }
        public ActionResult DeleteRouter(int id)
        {
            objsqlite.DelRouters(id);
            return RedirectToAction("Routers", "Home");

        }
    }
}