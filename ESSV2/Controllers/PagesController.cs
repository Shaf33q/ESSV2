using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ESSV2.Models;
using System.Globalization;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Web.Services;

namespace ESSV2.Controllers
{
    public class PagesController : Controller
    {
        // GET: Pages
        SqliteDataClass objsqlite = new SqliteDataClass();
       
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        DateTime  indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
      
        public ActionResult ClockTime()
        {
            if (Session["UserID"] != null)
            {

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }
        public ActionResult PaySlipView()
        {
            if (Session["UserID"] != null && Session["Role"].ToString() == "Admin")
            {
                IEnumerable<int> i = indianTime.BusinessDays();
                int Bdays = i.Count();
                int Hdays = objsqlite.GetHolidaysinMonth(Convert.ToString(indianTime.Month));
                int TotalWorkDays = Bdays - Hdays;
                ViewBag.WDays = TotalWorkDays;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }
        [HttpPost]
        public JsonResult SaveFiles()
        {
            string Message, fileName, actualFileName;
            Message = fileName = actualFileName = string.Empty;
            bool flag = false;
            if (Request.Files != null)
            {
                var file = Request.Files[0];
                actualFileName = file.FileName;
                fileName = "P" + Session["UserID"].ToString() + Path.GetExtension(file.FileName);
                int size = file.ContentLength;

                try
                {
                    file.SaveAs(Path.Combine(Server.MapPath("~/ProfileImg"), fileName));
                    Message = "File uploaded successfully";

                }
                catch (Exception)
                {
                    Message = "File upload failed! Please try again";
                }

            }
            return new JsonResult { Data = new { Message = Message, Status = flag } };
        }
        public ActionResult AdminDash()
        {
            if (Session["UserID"] != null && Session["Role"].ToString() == "Admin")
            {

                int Employees = objsqlite.GetNumOfEmployees();
                ViewBag.UsersRegistered = Employees;
                int Absenties = objsqlite.GetNumOfAbsenties(indianTime.ToString("yyyy-MM-dd"));
                ViewBag.Absenties = Absenties;
                int HolidayRequests = objsqlite.GetNumOfHolReqs();
                ViewBag.HolidayCount = HolidayRequests;
                int IPs = objsqlite.GetNumOfIPs();
                ViewBag.IPs = IPs;
                int Tasks = objsqlite.GetIncompleteTasks();
                ViewBag.Tasks = Tasks;
                string Holiday= objsqlite.GetClosestHoliday(indianTime.ToString("yyyy-MM-dd"));
                ViewBag.Holiday = (Holiday != "") ? Convert.ToDateTime(Holiday).ToString("yyyy-MM-dd"):"None";
                int Payslips = objsqlite.GetPaySlipsGen(indianTime.ToString("yyyy-MM"));
                ViewBag.PaySlip = Payslips;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public ActionResult Clockin()
        {
            var Logid = Session["Logid"].ToString();
            string AttStatus= objsqlite.GetAttStatus(Convert.ToInt32(Session["UserID"]), indianTime.ToString("yyyy-MM-dd"));
            var Create = objsqlite.AttStatus(indianTime.ToString("yyMMdd")+ Session["UserID"].ToString(), Convert.ToInt32(Session["UserID"]), indianTime.ToString("yyyy-MM-dd"),"C", indianTime.GetWeekOfMonth(), Convert.ToInt32(indianTime.DayOfWeek));
            var Clockin = objsqlite.ClockinTime(Logid, indianTime.ToString("yyyy-MM-dd"), indianTime.ToString("HH:mm:ss"), Convert.ToInt32(Session["UserID"]));

            if(indianTime.DayOfWeek == DayOfWeek.Saturday|| indianTime.DayOfWeek == DayOfWeek.Sunday|| AttStatus=="AB")
            {
                var Leaves = objsqlite.EmployeeLeaves.SingleOrDefault(u => (u.UserID == Convert.ToInt32(Session["UserID"])));
                int RemMed = Convert.ToInt32(Leaves.RemMedical);
                int RemCas = Convert.ToInt32(Leaves.RemCasual);
                int Medical = Convert.ToInt32(Leaves.Medical);
                int Casual = Convert.ToInt32(Leaves.Casual);
                int Extra = Convert.ToInt32(Leaves.Extra);
                int LOP = Convert.ToInt32(Leaves.LOP);
                if (LOP >= 1)
                {
                  objsqlite.UpdateLeaves(LOP-1, RemMed, RemCas, Extra, Convert.ToInt32(Session["UserID"]));
                }
                else if (RemMed<Medical)
                {
                    objsqlite.UpdateLeaves(0,RemMed+1, RemCas, Extra, Convert.ToInt32(Session["UserID"]));
                }
                else if(RemCas< Casual)
                {
                    objsqlite.UpdateLeaves(0,Medical, RemCas + 1, Extra, Convert.ToInt32(Session["UserID"]));
                }
                else
                {
                    objsqlite.UpdateLeaves(0,Medical, Casual, Extra + 1, Convert.ToInt32(Session["UserID"]));
                }
            }
            Session["Checkin"] = true;
            TempData["Notifaction"] = "Clocked In/Time : "+ indianTime.ToString("HH:mm:ss");
            return View("ClockTime");

        }
        public ActionResult Clockout()
        {
            var Logid = Session["Logid"].ToString();
            objsqlite.AttStatus(indianTime.ToString("yyMMdd") + Session["UserID"].ToString(), Convert.ToInt32(Session["UserID"]), indianTime.ToString("yyyy-MM-dd"), "P", indianTime.GetWeekOfMonth(), Convert.ToInt32(indianTime.DayOfWeek));
            objsqlite.ClockoutTime(Logid, indianTime.ToString("HH:mm:ss"));
            Session["Checkin"] = false;
            TempData["Notifaction"] = "Clocked Out/Time : " + indianTime.ToString("HH:mm:ss");
            return View("ClockTime");


        }
     
        public ActionResult RunAttService()
        {

            List<UserAccounts> employee = objsqlite.Employees.ToList();
            foreach(UserAccounts emp in employee)
            {
                bool Absent=objsqlite.AttService(indianTime.ToString("yyMMdd") + emp.UserID.ToString(), Convert.ToInt32(emp.UserID), indianTime.ToString("yyyy-MM-dd"), indianTime.GetWeekOfMonth(), Convert.ToInt32(indianTime.DayOfWeek));
                if (Absent == true)
                {
                    string RemLeaves = objsqlite.RemainingLeaves(emp.UserID);
                    string[] Rem = RemLeaves.Split('/');
                    int RemCasual = Convert.ToInt32(Rem[1]);
                    int RemMedical = Convert.ToInt32(Rem[0]);
                    int Extra = Convert.ToInt32(Rem[2]);
                    int LOP = Convert.ToInt32(Rem[3]);                   

                    
                    if (Extra >= 1)
                    {
                        objsqlite.UpdateLeaves(0, RemMedical, RemCasual, Extra-1, emp.UserID);
                    }
                    else if (RemCasual>=1)
                    {
                        objsqlite.UpdateLeaves(0, RemMedical, RemCasual-1, Extra, emp.UserID);
                    }
                    else if (RemMedical >= 1)
                    {
                        objsqlite.UpdateLeaves(0, RemMedical-1, RemCasual, Extra, emp.UserID);
                    }
                    else
                    {
                        objsqlite.UpdateLeaves(LOP+1, RemMedical - 1, RemCasual, Extra, emp.UserID);
                    }
                }
                
            }            
            return RedirectToAction("AdminDash", "Pages");

        }
        public ActionResult Attendance()
        {
            if (Session["UserID"] != null)
            {
                var model = new AllAttendanceData();
                var lastSixMonths = Enumerable
   .Range(0, 6)
   .Select(i => DateTime.Now.AddMonths(i - 6))
   .Select(date => date.ToString("MMMM"));
                ViewBag.Labels = lastSixMonths;
                IEnumerable<BarChartData> BarChtData = objsqlite.BarChartData(Convert.ToInt32(Session["UserID"]));
              
                List<string> ChartData = new List<string>();
                foreach (string Month in lastSixMonths)
                {
                    string monthInDigit = DateTime.ParseExact(Month, "MMMM", CultureInfo.InvariantCulture).Month.ToString("00");
                   
                    var BarChartDataMonth = BarChtData.SingleOrDefault(a => a.Month == monthInDigit);
                    if (BarChartDataMonth!=null)
                    {
                        ChartData.Add(BarChartDataMonth.Count);
                    }
                    else
                    {
                        ChartData.Add("0");
                    }
                   
                    

                }
                ViewBag.ChartData = ChartData;


                model.AttendanceData = objsqlite.Attendance.Where(u => (u.UserID == Convert.ToInt32(Session["UserID"]))).GroupBy(a => a.ClockDate).Select(g => g.First()).ToList();
                model.HolidayStatus = objsqlite.GetHolidayStatus.Where(u => (u.UserID == Convert.ToInt32(Session["UserID"]))).ToList();
                //var Attdata = objsqlite.Attendance.Where(u => (u.UserID == Convert.ToInt32(Session["UserID"]))&&(u.ClockDate==DateTime.Now.Date)).FirstOrDefault();
                var usr1 = objsqlite.EmployeeLeaves.SingleOrDefault(u => (u.UserID == Convert.ToInt32(Session["UserID"])));
                if (usr1 != null)
                {
                    if (usr1.Extra == "")
                    {
                        ViewBag.Extra = "0";
                    }
                    else
                    {
                        ViewBag.Extra = usr1.Extra;
                    }

                    
                    ViewBag.MedLeaves = usr1.RemMedical + "/" + usr1.Medical;
                    ViewBag.CasLeaves = usr1.RemCasual + "/" + usr1.Casual;

                }
                return View(model);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }



        }
        public ActionResult AddTasks()
        {
            if (Session["UserID"] != null && Session["Role"].ToString() == "Admin")
            {
                List<SelectListItem> EmpNames = new List<SelectListItem>();
                List<UserAccounts> employee = objsqlite.Employees.ToList();
                foreach (UserAccounts EmployeeName in employee)
                {
                    SelectListItem EmpName = new SelectListItem
                    {
                        Text = EmployeeName.Username,
                        Value = EmployeeName.UserID.ToString()
                    };
                    EmpNames.Add(EmpName);
                }
                ViewBag.EmployeeNames = EmpNames;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }

        }
        public ActionResult RequestHoliday(AllAttendanceData AttData)
        {            
            if (Session["UserID"] != null)
            {
                if (ModelState.IsValid)
                {
                    TimeSpan RemDays = AttData.AllHoliday.ToDate - AttData.AllHoliday.FromDate;
                    string RemLeaves=objsqlite.RemainingLeaves(Convert.ToInt32(Session["UserID"]));
                    string[] Rem = RemLeaves.Split('/');
                    int RemCasual = Convert.ToInt32(Rem[1]);
                    int RemMedical = Convert.ToInt32(Rem[0]);
                    int RemTotal = RemCasual + RemMedical;
                    int DaysNo = RemDays.Days + 1;
                    var OldReq = objsqlite.GetHolidayStatus.SingleOrDefault(u => (u.UserID == Convert.ToInt32(Session["UserID"]))&& u.Status=="Pending");
                    if (OldReq==null) { 
                    if (RemTotal >= DaysNo)
                    {
                      var Req = objsqlite.HolidayRequest(indianTime.ToString("yyyy-MM-dd HH:mm:ss"), Convert.ToInt32(Session["UserID"]), AttData.AllHoliday.FromDate.ToString("yyyy-MM-dd"), AttData.AllHoliday.ToDate.ToString("yyyy-MM-dd"), AttData.AllHoliday.Reason, DaysNo);
                      TempData["Notifaction"] = "Success !!!/Holidays Request Sent Successfully";
                    }
                    else
                    {
                        TempData["Notifaction"] = "Request Error!!!/You Have Only "+ RemTotal + " Remaining Holidays";
                    }
                    }
                    else
                    {
                        TempData["Notifaction"] = "Request Error!!!/You Already Have a Pending Request";

                    }
                }
                
                return RedirectToAction("Attendance", "Pages");
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }



        }
        public ActionResult ApproveHoliday(int id,int RemDays,string From,string To)
        {
            DateTime FromDate = Convert.ToDateTime(From);
            DateTime ToDate = Convert.ToDateTime(To);
            var LDays=Enumerable.Range(0, 1 + ToDate.Subtract(FromDate).Days).Select(offset => FromDate.AddDays(offset)).ToArray();
            string RemLeaves = objsqlite.RemainingLeaves(Convert.ToInt32(Session["UserID"]));
            string[] Rem = RemLeaves.Split('/');
            int RemCasual = Convert.ToInt32(Rem[1]);
            int RemMedical = Convert.ToInt32(Rem[0]);
            int Extra= Convert.ToInt32(Rem[2]);
            int RemTotal = RemCasual + RemMedical+ Extra;
            if (RemTotal>= RemDays)
            {
                if(RemDays <= Extra)
                {
                    objsqlite.UpdateLeaves(0,RemMedical, RemCasual, Extra - RemDays, Convert.ToInt32(Session["UserID"]));
                }
                else if (RemDays <= RemCasual)
                {
                    objsqlite.UpdateLeaves(0,RemMedical, RemCasual + Extra - RemDays,0, Convert.ToInt32(Session["UserID"]));
                }
                else
                {
                    objsqlite.UpdateLeaves(0,RemTotal - RemDays, 0,0, Convert.ToInt32(Session["UserID"]));
                }
                objsqlite.ApproveHoliday(id, Session["Username"].ToString());
                foreach(DateTime Leave in LDays)
                {
                    objsqlite.AttStatus(Leave.ToString("yyMMdd") + id, id, Leave.ToString("yyyy-MM-dd"), "L", Leave.GetWeekOfMonth(), Convert.ToInt32(Leave.DayOfWeek));
                }
               
               
                TempData["Notifaction"] = "Success !!!/Leave Request Approved Successfully";
            }
            else
            {
                TempData["Notifaction"] = "Cannot be Approved!!!/Insufficient Leave Balance";
            }
            
            return RedirectToAction("AllHolidays", "Pages");


        }
        public ActionResult DisApproveHoliday(int id, int RemDays, string From, string To)
        {
            DateTime FromDate = Convert.ToDateTime(From);
            DateTime ToDate = Convert.ToDateTime(To);
            var LDays = Enumerable.Range(0, 1 + ToDate.Subtract(FromDate).Days).Select(offset => FromDate.AddDays(offset)).ToArray();
            if (RemDays != 0)
            {
                //Get Remaining Holidays
                string RemLeaves = objsqlite.RemainingLeaves(Convert.ToInt32(Session["UserID"]));
                string[] Rem = RemLeaves.Split('/');
                int RemCasual = Convert.ToInt32(Rem[1]);
                int RemMedical = Convert.ToInt32(Rem[0]);
                int Extra = Convert.ToInt32(Rem[2]);
                int RemTotal = RemCasual + RemMedical+ Extra;

                //Get Alloted Holidays
                string Leaves = objsqlite.GetAllotedLeaves(Convert.ToInt32(Session["UserID"]));
                string[] Leave = Leaves.Split('/');
                int Casual = Convert.ToInt32(Leave[1]);
                int Medical = Convert.ToInt32(Leave[0]);
               

                if (RemMedical + RemDays <= Medical)
                {
                    objsqlite.UpdateLeaves(0,RemMedical + RemDays, RemCasual, Extra, Convert.ToInt32(Session["UserID"]));
                }
                else if(Casual>= RemCasual+ ((RemDays+ RemMedical) - Medical))
                {
                    objsqlite.UpdateLeaves(0,Medical, RemCasual+RemMedical + RemDays - Medical, Extra, Convert.ToInt32(Session["UserID"]));
                }else
                {
                    objsqlite.UpdateLeaves(0,Medical, Casual, (RemTotal+ RemDays-(Medical+Casual)), Convert.ToInt32(Session["UserID"]));
                }


            }
            objsqlite.DisApproveHoliday(id, Session["Username"].ToString());
            foreach (DateTime Leave in LDays)
            {
                objsqlite.DelHolStats(Leave.ToString("yyyy-MM-dd"), "L");
            }
           
            return RedirectToAction("AllHolidays", "Pages");


        }
        public ActionResult DeleteHolRequest(int id)
        {

            objsqlite.DelHolidayRequest(id);
            return RedirectToAction("Attendance", "Pages");


        }
        public ActionResult PaySlipGenerate()
        {
            if (Session["UserID"] != null && Session["Role"].ToString() == "Admin")
            {
                IEnumerable <int> i = indianTime.BusinessDays();
                int Bdays = i.Count();
                int Hdays=objsqlite.GetHolidaysinMonth(Convert.ToString(indianTime.Month));
                int TotalWorkDays = Bdays - Hdays;
                ViewBag.WDays = TotalWorkDays;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }

        }

        public ActionResult AllHolidays()
        {
            if (Session["UserID"] != null && Session["Role"].ToString() == "Admin")
            {
                var model = new AllAttendanceData();
                model.HolidayStatus = objsqlite.GetHolidayStatus.ToList();
                var HolidayCount = model.HolidayStatus;
                var count = HolidayCount.Count(u => (u.Status == "Pending"));
                ViewBag.HolidayCount = count;
                return View(model);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }

        }

        public ActionResult MyProfile()
        {
            if (Session["UserID"] != null)
            {
                string path = Server.MapPath("~/ProfileImg/P" + Session["UserID"] + ".jpg");
                ViewBag.ProfilePic = System.IO.File.Exists(path) ? "../../ProfileImg/P" + Session["UserID"] + ".jpg" : "../../ProfileImg/nobody.jpg";
                var usr = objsqlite.EmployeeProfile.SingleOrDefault(u => (u.UserID == Convert.ToInt32(Session["UserID"])));
                if (usr != null)
                {
                    ViewBag.FirstName = usr.FirstName;
                    ViewBag.LastName = usr.LastName;
                    ViewBag.Email = usr.Email;
                    ViewBag.Mobile = usr.Mobile;
                    ViewBag.Addr1 = usr.Addr1;
                    ViewBag.Addr2 = usr.Addr2;
                    ViewBag.City = usr.City;
                    ViewBag.State = usr.State;
                    ViewBag.PinCode = usr.PinCode;
                    ViewBag.Role = usr.Role;
                    ViewBag.UserName = usr.UserName;
                    ViewBag.DOB = usr.DOB;
                    ViewBag.BldGrp = usr.BldGrp;
                    ViewBag.Exp = usr.Experience +" Months";
                    ViewBag.Designation = usr.Designation;
                    ViewBag.BankName = usr.BankName;
                    ViewBag.BankAccNo = usr.BankAccNo;
                    ViewBag.PAN = usr.PAN;

                }
                else
                {
                    var employee = objsqlite.Employees.SingleOrDefault(u => (u.UserID == Convert.ToInt32(Session["UserID"])));

                    ViewBag.FirstName = "";
                    ViewBag.LastName = "";
                    ViewBag.Email = employee.Email;
                    ViewBag.Mobile = "";
                    ViewBag.Addr1 = "";
                    ViewBag.Addr2 = "";
                    ViewBag.City = "";
                    ViewBag.State = "";
                    ViewBag.PinCode = "";
                    ViewBag.DOB = "";
                    ViewBag.Role = employee.Role;
                    ViewBag.BldGrp = "";
                    ViewBag.Exp ="";
                    ViewBag.Designation = "";
                    ViewBag.BankName = "";
                    ViewBag.BankAccNo = "";
                    ViewBag.PAN = "";
                }

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }
        public ActionResult DeleteTask(int id,int Type)
        {
            objsqlite.DeleteTask(id);
            if (Type == 1)
            {
                return RedirectToAction("MyTasks", "Pages");
            }else
            {
                return RedirectToAction("AllTasks", "Pages");
            }
           

        }
        public ActionResult CompleteTask(int id, int red)
        {

            objsqlite.CompleteTask(id);
            if (red == 2)
            {
                return RedirectToAction("MyTasks", "Pages");
            }
            else { return RedirectToAction("AllTasks", "Pages"); }

        }
        public ActionResult IncompleteTask(int id, int red)
        {

            objsqlite.IncompleteTask(id);
            if (red == 2)
            {
                return RedirectToAction("MyTasks", "Pages");
            }
            else { return RedirectToAction("AllTasks", "Pages"); }


        }
        public ActionResult MyTasks()
        {
            if (Session["UserID"] != null)
            {
                var model = new AllTaskData();
                model.TaskData = objsqlite.GetTaskData.Where(u => (u.AllotedTo == Convert.ToInt32(Session["UserID"]))).ToList();
                ViewBag.TasksCount = model.TaskData.Count(u => u.Status == "Inprogress");
                return View(model);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }

        }
        public ActionResult PaySlips()
        {
            if (Session["UserID"] != null)
            {
                var payslips=objsqlite.PaySlips.Where(u => (u.UserID == Convert.ToInt32(Session["UserID"])));
                return View(payslips);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }

        }
        public ActionResult GeneratePayslip(int id,string date)
        {
            if (Session["UserID"] != null)
            {
                var model = objsqlite.GetPaySlipData(id, Convert.ToDateTime(date).ToString("yyyy-MM-dd"));

                string path= Server.MapPath("~/ProfileImg/PaySlipNew.pdf");
                string temppath = Server.MapPath("~/ProfileImg/"+ Session["UserID"].ToString() + ".pdf");
                PdfReader rdr = new PdfReader(path);
                PdfStamper stamper = new PdfStamper(rdr,
                new System.IO.FileStream(temppath, FileMode.Create, FileAccess.ReadWrite, FileShare.None));
                model.Bonus= model.Bonus != null && model.Bonus != "" ?model.Bonus : "0";
                model.LOPAmount = model.LOPAmount != null && model.LOPAmount != "" ? model.LOPAmount : "0";
                model.PFAmount = model.PFAmount != null && model.PFAmount != "" ? model.PFAmount : "0";
                model.ESI = model.ESI != null && model.ESI != "" ? model.ESI : "0";
                model.ProfTax = model.ProfTax != null && model.ProfTax != "" ? model.ProfTax : "0";
                decimal TotalEarned = Convert.ToDecimal(model.Basic) + Convert.ToDecimal(model.HRA) + Convert.ToDecimal(model.Bonus);
                decimal TotalDeduction = Convert.ToDecimal(model.LOPAmount) + Convert.ToDecimal(model.PFAmount) + Convert.ToDecimal(model.ESI) +Convert.ToDecimal(model.ProfTax); 
                stamper.AcroFields.SetField("EmpName", model.Name);
                stamper.AcroFields.SetField("Month", Convert.ToDateTime(model.MonthnYear).Month.ToString());
                stamper.AcroFields.SetField("EmpNo", model.UserID.ToString());
                stamper.AcroFields.SetField("Designation", model.Designation);
                stamper.AcroFields.SetField("BankAcc", model.BankAccNo);
                stamper.AcroFields.SetField("PFNumber", model.PFNumber);
                stamper.AcroFields.SetField("PAN", model.PAN);
                stamper.AcroFields.SetField("WorkingDays", model.WorkingDays);
                stamper.AcroFields.SetField("LOPDays", model.LOPDays.ToString());
                stamper.AcroFields.SetField("BASIC", model.Basic);
                stamper.AcroFields.SetField("HRA", model.HRA);
                stamper.AcroFields.SetField("Bonus", model.Bonus);
                stamper.AcroFields.SetField("TotalEarn", TotalEarned.ToString());
                stamper.AcroFields.SetField("LOP", model.LOPAmount);
                stamper.AcroFields.SetField("PF", model.PFAmount);
                stamper.AcroFields.SetField("ESI", model.ESI);
                stamper.AcroFields.SetField("ProfTax", model.ProfTax);
                stamper.AcroFields.SetField("TotalDetection", TotalDeduction.ToString());
                stamper.AcroFields.SetField("NetSalary", model.Total);
                stamper.FormFlattening = true;

                stamper.Close();
                rdr.Close();
                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AppendHeader("Content-Disposition", "attachment; filename=Payslip("+ model.MonthnYear + ").pdf");
                Response.TransmitFile(temppath);
                Response.End();
                Response.Close();
                return RedirectToAction("PaySlips", "Pages");
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }

        }
        
        public ActionResult PrintSalaryData(int id,string date)
        {
            if (Session["UserID"] != null)
            {
                var model = objsqlite.GetPaySlipData(id, (Convert.ToDateTime(date)).ToString("yyyy-MM-dd"));

                string path = Server.MapPath("~/ProfileImg/PaySlipNew.pdf");
                string temppath = Server.MapPath("~/ProfileImg/" + Session["UserID"].ToString() + ".pdf");
                PdfReader rdr = new PdfReader(path);
                PdfStamper stamper = new PdfStamper(rdr,
                new System.IO.FileStream(temppath, FileMode.Create, FileAccess.ReadWrite, FileShare.None));
                model.Bonus = model.Bonus != null && model.Bonus != "" ? model.Bonus : "0";
                model.LOPAmount = model.LOPAmount != null && model.LOPAmount != "" ? model.LOPAmount : "0";
                model.PFAmount = model.PFAmount != null && model.PFAmount != "" ? model.PFAmount : "0";
                model.ESI = model.ESI != null && model.ESI != "" ? model.ESI : "0";
                model.ProfTax = model.ProfTax != null && model.ProfTax != "" ? model.ProfTax : "0";
                decimal TotalEarned = Convert.ToDecimal(model.Basic) + Convert.ToDecimal(model.HRA) + Convert.ToDecimal(model.Bonus);
                decimal TotalDeduction = Convert.ToDecimal(model.LOPAmount) + Convert.ToDecimal(model.PFAmount) + Convert.ToDecimal(model.ESI) + Convert.ToDecimal(model.ProfTax);
                stamper.AcroFields.SetField("EmpName", model.Name);
                stamper.AcroFields.SetField("Month", Convert.ToDateTime(model.MonthnYear).Month.ToString());
                stamper.AcroFields.SetField("EmpNo", model.UserID.ToString());
                stamper.AcroFields.SetField("Designation", model.Designation);
                stamper.AcroFields.SetField("BankAcc", model.BankAccNo);
                stamper.AcroFields.SetField("PFNumber", model.PFNumber);
                stamper.AcroFields.SetField("PAN", model.PAN);
                stamper.AcroFields.SetField("WorkingDays", model.WorkingDays);
                stamper.AcroFields.SetField("LOPDays", model.LOPDays.ToString());
                stamper.AcroFields.SetField("BASIC", model.Basic);
                stamper.AcroFields.SetField("HRA", model.HRA);
                stamper.AcroFields.SetField("Bonus", model.Bonus);
                stamper.AcroFields.SetField("TotalEarn", TotalEarned.ToString());
                stamper.AcroFields.SetField("LOP", model.LOPAmount);
                stamper.AcroFields.SetField("PF", model.PFAmount);
                stamper.AcroFields.SetField("ESI", model.ESI);
                stamper.AcroFields.SetField("ProfTax", model.ProfTax);
                stamper.AcroFields.SetField("TotalDetection", TotalDeduction.ToString());
                stamper.AcroFields.SetField("NetSalary", model.Total);
                stamper.FormFlattening = true;

                stamper.Close();
                rdr.Close();
                Response.Clear();
                Response.ContentType = "application/pdf";
                Response.AppendHeader("Content-Disposition", "attachment; filename=Payslip(" + model.MonthnYear + ").pdf");
                Response.TransmitFile(temppath);
                Response.End();
                Response.Close();
                return RedirectToAction("PaySlipView", "Pages");
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }

        }

        public ActionResult AllTasks()
        {
            {
                if (Session["UserID"] != null && Session["Role"].ToString() == "Admin")
                {
                    List<TaskData> Tasks = objsqlite.GetTaskData.ToList();
                    return View(Tasks);
                }
                else
                {
                    return RedirectToAction("Login", "Home");
                }

            }

        }
        [HttpGet]
        public ActionResult ViewProfile(int? id)
        {
            if (Session["UserID"] != null && Session["Role"].ToString() == "Admin")
            {
                ViewBag.ID = id;
                var usr = objsqlite.EmployeeProfile.SingleOrDefault(u => (u.UserID == id));
                if (usr != null)
                {
                    ViewBag.Name = usr.FirstName + " " + usr.LastName;
                    ViewBag.Email = usr.Email;
                    ViewBag.Mobile = usr.Mobile;
                    ViewBag.Address = usr.Addr1+", "+ usr.Addr2+", " + usr.City + ", " + usr.State + ", " + usr.PinCode;
                    ViewBag.Role = usr.Role;
                    ViewBag.UserName = usr.UserName;
                    ViewBag.DOB = Convert.ToDateTime(usr.DOB).ToString("dd-MM-yyyy");
                    ViewBag.BldGrp = usr.BldGrp;
                    ViewBag.Exp = usr.Experience +" Months";
                    ViewBag.Designation = usr.Designation;
                    ViewBag.JDate = Convert.ToDateTime(usr.JoinDate).ToString("dd-MM-yyyy");
                    ViewBag.Salary = usr.Salary;
                    ViewBag.BankName = usr.BankName;
                    ViewBag.BankAccNo = usr.BankAccNo;
                    ViewBag.PAN = usr.PAN;

                }
                var usr1 = objsqlite.EmployeeLeaves.SingleOrDefault(u => (u.UserID == id));
                if (usr1 != null)
                {
                    ViewBag.Extra = usr1.Extra;
                    ViewBag.MedLeaves = usr1.RemMedical+"/"+ usr1.Medical;
                    ViewBag.CasLeaves = usr1.RemCasual + "/" + usr1.Casual;
                    
                }

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }

        }
        public ActionResult DelHoliday(int id,string date)
        {
            var Holidays = objsqlite.DelHoliday(id);
            //DateTime Hol = Convert.ToDateTime(date);
            //objsqlite.DelHolStats(Hol.ToString("yyyy-MM-dd"),"H");
            return RedirectToAction("AddHolidays", "Pages");
        }
        
        public ActionResult AddHolidays()
        {
            if (Session["UserID"] != null && Session["Role"].ToString() == "Admin")
            {
                IEnumerable<Holidays> AllHolidays = objsqlite.GetHolidays;

            return View(AllHolidays);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
         }
        public ActionResult AddHolidayList(IEnumerable<Holidays> Holidays)
        {

            foreach (Holidays Holiday in Holidays)
            {
                DateTime HolDate = Convert.ToDateTime(Holiday.Leave_Date);
                var usr = objsqlite.InsertHolidays(Holiday.Leave_Info, HolDate.ToString("yyyy-MM-dd"));
                var employee = objsqlite.Employees;
                foreach (UserAccounts emp in employee)
                {
                    objsqlite.AttStatus(HolDate.ToString("yyMMdd") + emp.UserID, emp.UserID, HolDate.ToString("yyyy-MM-dd"), "H", HolDate.GetWeekOfMonth(), Convert.ToInt32(HolDate.DayOfWeek));

                }
            }


            return Json(new { status = "Success" });
        }
        public ActionResult JobList()
        {
            if (Session["UserID"] != null && Session["Role"].ToString() == "Admin")
            {
                IEnumerable<JobDetails> jobslist = objsqlite.GetVacancies;

                return View(jobslist);
            }
            else {
                return RedirectToAction("Login", "Home");
            }
          
        }
        [HttpGet]
        public ActionResult InsertJobDetails()
        {
            return View();
        }
        [HttpPost]
        public ActionResult InsertJobDetails(JobDetails jobdetails)
        {
            if (Session["UserID"] != null && Session["Role"].ToString() == "Admin")
            {
                if (ModelState.IsValid)
                {
                    var NoRegistererror = objsqlite.InsertVacancies(jobdetails.JobTitle, jobdetails.AboutJob, jobdetails.Industry, jobdetails.FunctionalArea, jobdetails.Role, jobdetails.JobKeyskills, jobdetails.JobLocation, jobdetails.JobSalary, jobdetails.DesiredCandidateDetails, jobdetails.CandidateEducationDetails, jobdetails.CandidateLanguageKnown, jobdetails.CandidateExperience);

                    if (NoRegistererror == true)
                    {
                        ViewBag.Message = "<div class='callout callout-success'><p><i class='fa fa-check'></i>  Job Posted Successfully !!!</p></div>";
                        return RedirectToAction("JobList", "Pages");
                    }
                    else
                    {
                        ViewBag.Message = "<div class='callout callout-danger'><p><i class='fa fa-warning'></i> Sorry Try Again !!!</p></div>";
                        return RedirectToAction("JobList", "Pages");
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
        public ActionResult DeleteVacancy(int Id)
        {
            bool resultdata = objsqlite.DelVacancy(Id);
            if (resultdata == true)
            {
                ViewBag.Message = "<div class='callout callout-danger'><p><i class='fa fa-warning'></i> Sorry Try Again !!!</p></div>";
                return RedirectToAction("JobList", "Pages");
            }
            else
            {
                ViewBag.Message = "<div class='callout callout-danger'><p><i class='fa fa-warning'></i> Sorry Try Again !!!</p></div>";
                return RedirectToAction("JobList", "Pages");
            }
        }

    }
    static class DateTimeExtensions
    {
        static GregorianCalendar _gc = new GregorianCalendar();
        public static int GetWeekOfMonth(this DateTime time)
        {
            DateTime first = new DateTime(time.Year, time.Month, 1);
            return time.GetWeekOfYear() - first.GetWeekOfYear() + 1;
        }

        static int GetWeekOfYear(this DateTime time)
        {
            return _gc.GetWeekOfYear(time, CalendarWeekRule.FirstDay, DayOfWeek.Sunday);
        }
        public static IEnumerable<int> BusinessDays(this DateTime date)

    {

        var days = (from d in DaysInMonth(date)

                    let day = new DateTime(date.Year, date.Month, d).DayOfWeek

                    where

                        day != DayOfWeek.Saturday

                        && day != DayOfWeek.Sunday
                    select d);

        return days;

    }

 

    private static IEnumerable<int> DaysInMonth(DateTime date)

    {

        return Enumerable.Range(1, DateTime.DaysInMonth(date.Year, date.Month));

    }

    }

    
}