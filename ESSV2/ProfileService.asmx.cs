using System;
using System.Web;
using System.Web.Services;
using System.Data.SQLite;
using System.Web.Script.Services;
using System.Data;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using ESSV2.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Globalization;

namespace ESSV2
{
    /// <summary>
    /// Summary description for ProfileService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [ScriptService]
    public class ProfileService : System.Web.Services.WebService
    {
        static string strDB = "\"" + HttpContext.Current.Server.MapPath("~/App_Data/ESSDB.s3db") + "\"";
        string strConnection = @"data source=" + strDB + ";Version=3;";




        [WebMethod]

        public string AddPaySlips(string MonthnYear, int UserID,
                string Salary, string HRA, string Basic,
                int LOP, int WDays, string BankName, string AccNo, string PerDay, string Bonus, string ESI,
                string ProfTax, string PFAmount, string PFNumber, string PAN, string Total, string Name, string Designation)
        {
            string PayslipID = MonthnYear.Replace("-", "") + UserID;
            SQLiteConnection con = new SQLiteConnection(strConnection);
            SQLiteCommand cmd;
            try
            {
                con.Open();
                cmd = con.CreateCommand();
                cmd.CommandText = "INSERT OR REPLACE INTO TBL_PaySlips(PayslipID,MonthnYear, UserID, Salary, HRA, Basic,LOP,WorkDays,BankName,AccNo,PerDay,Bonus,ESI,ProfTax,PFAmount,PFNumber,PAN,Total,Name,Designation)VALUES(@PayslipID,@MonthnYear, @UserID, @Salary, @HRA, @Basic,@LOP,@WorkDays,@BankName,@AccNo,@PerDay,@Bonus,@ESI,@ProfTax,@PFAmount,@PFNumber,@PAN,@Total,@Name,@Designation)";
                cmd.Parameters.AddWithValue("@PayslipID", PayslipID);
                cmd.Parameters.AddWithValue("@MonthnYear", MonthnYear + "-01");
                cmd.Parameters.AddWithValue("@UserID", UserID);
                cmd.Parameters.AddWithValue("@Salary", Salary);
                cmd.Parameters.AddWithValue("@HRA", HRA);
                cmd.Parameters.AddWithValue("@Basic", Basic);
                cmd.Parameters.AddWithValue("@LOP", LOP);
                cmd.Parameters.AddWithValue("@WorkDays", WDays);
                cmd.Parameters.AddWithValue("@BankName", BankName);
                cmd.Parameters.AddWithValue("@AccNo", AccNo);
                cmd.Parameters.AddWithValue("@PerDay", PerDay);
                cmd.Parameters.AddWithValue("@Bonus", Bonus);
                cmd.Parameters.AddWithValue("@ESI", ESI);
                cmd.Parameters.AddWithValue("@ProfTax", ProfTax);
                cmd.Parameters.AddWithValue("@PFAmount", PFAmount);
                cmd.Parameters.AddWithValue("@PFNumber", PFNumber);
                cmd.Parameters.AddWithValue("@PAN", PAN);
                cmd.Parameters.AddWithValue("@Total", Total);
                cmd.Parameters.AddWithValue("@Name", Name);
                cmd.Parameters.AddWithValue("@Designation", Designation);
                cmd.ExecuteNonQuery();
                return "Record Inserted Successfully";
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }

            }
        }
        [WebMethod]

        public string AddTasks(string TaskName,
                string Deadline, string DateAlloted, string AllotedBy,
                int AllotedTo, string Priority, string Description)
        {

            SQLiteConnection con = new SQLiteConnection(strConnection);
            SQLiteCommand cmd;
            try
            {
                if (Deadline == "No deadline")
                {
                    Deadline = "1900-01-01";
                }
                con.Open();
                cmd = con.CreateCommand();
                cmd.CommandText = "INSERT INTO TBL_TasksAlloted(TaskName, Deadline, DateAlloted, AllotedBy, AllotedTo,Status,Description,Priority)VALUES(@TaskName, @Deadline, @DateAlloted, @AllotedBy, @AllotedTo,'Inprogress',@Description,@Priority)";
                cmd.Parameters.AddWithValue("@TaskName", TaskName);
                cmd.Parameters.AddWithValue("@Deadline", Deadline);
                cmd.Parameters.AddWithValue("@DateAlloted", DateAlloted);
                cmd.Parameters.AddWithValue("@AllotedBy", AllotedBy);
                cmd.Parameters.AddWithValue("@AllotedTo", AllotedTo);
                cmd.Parameters.AddWithValue("@Description", Description);
                cmd.Parameters.AddWithValue("@Priority", Priority);
                cmd.ExecuteNonQuery();
                return "Record Inserted Successfully";
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }

            }
        }

        [WebMethod]

        public string AddSelfTasks(string TaskName,
                string Deadline, string DateAlloted, string AllotedBy,
                int AllotedTo, string Priority, string Description)
        {

            SQLiteConnection con = new SQLiteConnection(strConnection);
            SQLiteCommand cmd;
            try
            {
                con.Open();
                cmd = con.CreateCommand();
                cmd.CommandText = "INSERT INTO TBL_TasksAlloted(TaskName, Deadline, DateAlloted, AllotedBy, AllotedTo,Status,Description,Priority)VALUES(@TaskName, @Deadline, @DateAlloted, @AllotedBy, @AllotedTo,'Self',@Description,@Priority)";
                cmd.Parameters.AddWithValue("@TaskName", TaskName);
                cmd.Parameters.AddWithValue("@Deadline", Deadline);
                cmd.Parameters.AddWithValue("@DateAlloted", DateAlloted);
                cmd.Parameters.AddWithValue("@AllotedBy", AllotedBy);
                cmd.Parameters.AddWithValue("@AllotedTo", AllotedTo);
                cmd.Parameters.AddWithValue("@Description", Description);
                cmd.Parameters.AddWithValue("@Priority", Priority);
                cmd.ExecuteNonQuery();
                return "Record Inserted Successfully";
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }

            }
        }

        [WebMethod]
        public void InsertProfileData(string UserID, string RowName, string Value)
        {
            using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
            {
                try
                {

                    sqlitecon.Open();
                    SQLiteCommand cmd = new SQLiteCommand("insert or ignore into TBL_Profile(UserID)values('" + UserID + "')", sqlitecon);
                    cmd.ExecuteNonQuery();
                    SQLiteCommand cmd1 = new SQLiteCommand("UPDATE TBL_Profile SET '" + RowName + "' = '" + Value + "' WHERE UserID ='" + UserID + "'", sqlitecon);
                    cmd1.ExecuteNonQuery();
                    sqlitecon.Close();
                    Context.Response.Write(true);
                }
                catch (Exception)
                {
                    sqlitecon.Close();
                    Context.Response.Write(false);
                }
            }
        }
        [WebMethod]
        public bool InsertLeaveData(string UserID, string RowName, string Value)
        {
            using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
            {
                try
                {
                    sqlitecon.Open();
                    SQLiteCommand cmd = new SQLiteCommand("insert or ignore into TBL_AllotedLeaves(UserID)values('" + UserID + "')", sqlitecon);
                    cmd.ExecuteNonQuery();
                    SQLiteCommand cmd1;
                    if (RowName == "RemMedical/Medical" || RowName == "RemCasual/Casual")
                    {
                        string[] ColumnNames = RowName.Split('/');
                        string[] Values = Value.Split('/');
                        cmd1 = new SQLiteCommand("UPDATE TBL_AllotedLeaves SET '" + ColumnNames[0] + "' = '" + Values[0] + "','" + ColumnNames[1] + "' = '" + Values[1] + "' WHERE UserID ='" + UserID + "'", sqlitecon);
                    }
                    else
                    {
                        cmd1 = new SQLiteCommand("UPDATE TBL_AllotedLeaves SET '" + RowName + "' = '" + Value + "' WHERE UserID ='" + UserID + "'", sqlitecon);
                    }

                    cmd1.ExecuteNonQuery();
                    sqlitecon.Close();
                    return true;
                }
                catch (Exception)
                {
                    sqlitecon.Close();
                    return false;
                }
            }
        }
        [WebMethod]
        public void UpdateUserData(string UserID, string RowName, string Value)
        {
            using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
            {
                try
                {
                    sqlitecon.Open();
                    SQLiteCommand cmd1 = new SQLiteCommand("UPDATE TBL_Userdata SET '" + RowName + "' = '" + Value + "' WHERE ID ='" + UserID + "'", sqlitecon);
                    cmd1.ExecuteNonQuery();
                    sqlitecon.Close();
                    Context.Response.Write(true);
                }
                catch (Exception)
                {
                    sqlitecon.Close();
                    Context.Response.Write(false);
                }
            }
        }
        [WebMethod]
        public bool CompleteTask(string TaskId, string Res)
        {
            using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
            {
                try
                {
                    sqlitecon.Open();
                    SQLiteCommand cmd1 = new SQLiteCommand("Update TBL_TasksAlloted SET Status = 'Completed',Resolution= '" + Res + "' where TaskID = '" + TaskId + "'", sqlitecon);
                    cmd1.ExecuteNonQuery();
                    sqlitecon.Close();
                    return true;
                }
                catch (Exception)
                {
                    sqlitecon.Close();
                    return false;
                }
            }
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetMonthData(string MonthNo, string YearNo, int UserID)
        {
            using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
            {
                try
                {
                    List<AttCalData> AttCalendarDatas = new List<AttCalData>();
                    SQLiteCommand cmd1 = new SQLiteCommand("select * from TBL_Attendance where UserID='" + UserID + "' AND strftime('%Y', AttDate) = '" + YearNo + "' AND strftime('%m', AttDate) = '" + MonthNo + "'", sqlitecon);
                    sqlitecon.Open();
                    SQLiteDataReader rdr = cmd1.ExecuteReader();
                    while (rdr.Read())
                    {
                        AttCalData AttCalendarData = new AttCalData();
                        AttCalendarData.Day = Convert.ToInt32(rdr["Day"]);
                        AttCalendarData.Week = Convert.ToInt32(rdr["Week"]);
                        AttCalendarData.ClockDate = ConvertDate(rdr["AttDate"].ToString());
                        AttCalendarData.Status = rdr["Status"].ToString();
                        AttCalendarDatas.Add(AttCalendarData);
                    }
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    Context.Response.Write(js.Serialize(AttCalendarDatas));


                }
                catch (Exception)
                {
                    sqlitecon.Close();

                }
            }
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetDayClockData(string Date, int UserID)
        {
            using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
            {
                try
                {

                    List<SelectedDayData> SelDayDatas = new List<SelectedDayData>();
                    SQLiteCommand cmd1 = new SQLiteCommand("SELECT ClockIn,ClockOut  FROM TBL_AttendanceDetails  Where UserID='" + UserID + "' AND ClockDate='" + Date + "'", sqlitecon);
                    sqlitecon.Open();
                    SQLiteDataReader rdr = cmd1.ExecuteReader();
                    while (rdr.Read())
                    {
                        SelectedDayData SelDayData = new SelectedDayData();
                        SelDayData.ClockIn = Convert.ToDateTime(rdr["ClockIn"]).ToString("hh:mm:ss tt");
                        string SelDayClockOut = rdr["ClockOut"].ToString();
                        DateTime ClockIn = ConvertDate(rdr["ClockIn"].ToString());
                        string Time = "";

                        if (!string.IsNullOrEmpty(SelDayClockOut))
                        {
                            DateTime ClockOut = ConvertDate(rdr["ClockOut"].ToString());
                            TimeSpan ClockTime = ClockOut - ClockIn;
                            Time = ClockTime.ToString();
                        }

                        SelDayData.ClockOut = string.IsNullOrEmpty(SelDayClockOut) ? "" : Convert.ToDateTime(rdr["ClockOut"]).ToString("hh:mm:ss tt");
                        SelDayData.Clocktime = string.IsNullOrEmpty(SelDayClockOut) ? "" : Time;
                        SelDayDatas.Add(SelDayData);
                    }
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    Context.Response.Write(js.Serialize(SelDayDatas));


                }
                catch (Exception)
                {
                    sqlitecon.Close();

                }
            }
        }
        [WebMethod]
        public void GetEmployees()
        {
            using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
            {
                try
                {

                    List<UserAccounts> GetEmployeeNames = new List<UserAccounts>();
                    SQLiteCommand cmd1 = new SQLiteCommand("SELECT ID,Username FROM TBL_Userdata", sqlitecon);
                    sqlitecon.Open();
                    SQLiteDataReader rdr = cmd1.ExecuteReader();
                    while (rdr.Read())
                    {
                        UserAccounts EmployeeName = new UserAccounts();
                        EmployeeName.UserID = Convert.ToInt32(rdr["ID"]);
                        EmployeeName.Username = rdr["Username"].ToString();
                        GetEmployeeNames.Add(EmployeeName);
                    }
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    Context.Response.Write(js.Serialize(GetEmployeeNames));


                }
                catch (Exception)
                {
                    sqlitecon.Close();

                }
            }
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetHoliday(string Date)
        {
            using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
            {
                try
                {

                    string Holiday = String.Empty;
                    SQLiteCommand cmd1 = new SQLiteCommand("SELECT Desc FROM TBL_Holidays where Date='" + Date + "'", sqlitecon);
                    sqlitecon.Open();
                    Holiday = Convert.ToString(cmd1.ExecuteScalar());
                    Context.Response.Write(Holiday);

                }
                catch (Exception)
                {
                    sqlitecon.Close();

                }
            }
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetWeeklyHours(int ID, string Date)
        {
            using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
            {
                try
                {

                    string TotalHours = String.Empty;
                    SQLiteCommand cmd1 = new SQLiteCommand("select(((sum(ClockTime)/60)/60)%60)||'.'||substr('0'||((sum(ClockTime)/60)%60), -2, 2) as TotalHours from TBL_AttendanceDetails  where UserID= '" + ID + "' AND strftime('%Y', ClockDate) = strftime('%Y', '" + Date + "') AND strftime('%W', ClockDate)=strftime('%W', '" + Date + "')", sqlitecon);
                    sqlitecon.Open();
                    TotalHours = Convert.ToString(cmd1.ExecuteScalar());
                    Context.Response.Write(TotalHours);

                }
                catch (Exception)
                {
                    sqlitecon.Close();

                }
            }
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetTaskDetails(int TaskID)
        {
            using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
            {
                try
                {

                    GetTaskData TaskDatae = new GetTaskData();
                    SQLiteCommand cmd1 = new SQLiteCommand("SELECT * from TBL_TasksAlloted  Where TaskID='" + TaskID + "'", sqlitecon);
                    sqlitecon.Open();
                    SQLiteDataReader rdr = cmd1.ExecuteReader();
                    while (rdr.Read())
                    {

                        TaskDatae.TaskID = Convert.ToInt32(rdr["TaskID"]);
                        TaskDatae.AllotedBy = rdr["AllotedBy"].ToString();
                        TaskDatae.Priority = rdr["Priority"].ToString();
                        TaskDatae.Status = rdr["Status"].ToString();
                        TaskDatae.TaskName = rdr["TaskName"].ToString();
                        TaskDatae.Description = rdr["Description"].ToString();
                        TaskDatae.Deadline = rdr["Deadline"].ToString();
                        TaskDatae.DateAlloted = rdr["DateAlloted"].ToString();
                        TaskDatae.Resolution = rdr["Resolution"].ToString();

                    }
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    Context.Response.Write(js.Serialize(TaskDatae));


                }
                catch (Exception)
                {
                    sqlitecon.Close();

                }
            }
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetGenUsers(string MonYear)
        {
            List<int> Payslips = new List<int>();
            using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
            {
                try
                {
                    string[] MY = MonYear.Split('-');
                    SQLiteCommand cmd1 = new SQLiteCommand(" select UserID from TBL_Payslips Where strftime('%m', MonthnYear)='" + MY[1] + "' and strftime('%Y', MonthnYear)='" + MY[0] + "'", sqlitecon);
                    sqlitecon.Open();
                    SQLiteDataReader rdr = cmd1.ExecuteReader();
                    while (rdr.Read())
                    {
                        int Users = Convert.ToInt32(rdr["UserID"]);
                        Payslips.Add(Users);

                    }
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    Context.Response.Write(js.Serialize(Payslips));


                }
                catch (Exception)
                {
                    sqlitecon.Close();

                }
            }
        }

        [WebMethod]

        public void GetSalaryData(string MonYear)
        {
            List<SalaryData> EmployeeSalarydata = new List<SalaryData>();
            using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
            {
                try
                {
                    string[] MY = MonYear.Split('-');
                    SQLiteCommand cmd1 = new SQLiteCommand("Select LOP,TP.UserID,FirstName,LastName,Designation,Salary,BankName,BankAccNo,PAN,PF from TBL_Profile TP  Inner Join TBL_AllotedLeaves TAL on TP.UserID = TAL.UserID where TP.UserID not in(select UserID from TBL_Payslips Where strftime('%m', MonthnYear)='" + MY[1] + "' and strftime('%Y', MonthnYear)='" + MY[0] + "')", sqlitecon);
                    sqlitecon.Open();
                    SQLiteDataReader rdr = cmd1.ExecuteReader();
                    while (rdr.Read())
                    {

                        SalaryData SalDat = new SalaryData();
                        SalDat.UserID = Convert.ToInt32(rdr["UserID"]);
                        SalDat.Name = rdr["FirstName"].ToString() + " " + rdr["LastName"].ToString();
                        SalDat.PAN = rdr["PAN"].ToString();
                        SalDat.BankName = rdr["BankName"].ToString();
                        SalDat.BankAccNo = rdr["BankAccNo"].ToString();
                        SalDat.Designation = rdr["Designation"].ToString();
                        SalDat.Salary = rdr["Salary"].ToString();
                        SalDat.LOPDays = Convert.ToInt32(rdr["LOP"]);
                        string path = Server.MapPath("~/ProfileImg/P" + SalDat.UserID + ".jpg");
                        SalDat.UserImg = System.IO.File.Exists(path) ? "P" + SalDat.UserID : "nobody";
                        EmployeeSalarydata.Add(SalDat);

                    }
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    Context.Response.Write(js.Serialize(EmployeeSalarydata));


                }
                catch (Exception)
                {
                    sqlitecon.Close();

                }

            }
        }
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetGenSalaryData(string MonYear)
        {
            List<SalaryData> EmployeeSalarydata = new List<SalaryData>();
            using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
            {
                try
                {
                    string[] MY = MonYear.Split('-');
                    SQLiteCommand cmd1 = new SQLiteCommand(" select* from TBL_Payslips Where strftime('%m', MonthnYear)='" + MY[1] + "' and strftime('%Y', MonthnYear)='" + MY[0] + "'", sqlitecon);
                    sqlitecon.Open();
                    SQLiteDataReader rdr = cmd1.ExecuteReader();
                    while (rdr.Read())
                    {

                        SalaryData SalDat = new SalaryData();
                        SalDat.UserID = Convert.ToInt32(rdr["UserID"]);
                        SalDat.Salary = rdr["Salary"].ToString();
                        SalDat.HRA = rdr["HRA"].ToString();
                        SalDat.Basic = rdr["Basic"].ToString();
                        SalDat.LOPDays = Convert.ToInt32(rdr["LOP"]);
                        SalDat.WorkingDays = rdr["WorkDays"].ToString();
                        SalDat.BankName = rdr["BankName"].ToString();
                        SalDat.BankAccNo = rdr["AccNo"].ToString();
                        SalDat.PerDay = rdr["PerDay"].ToString();
                        SalDat.Bonus = rdr["Bonus"].ToString();
                        SalDat.ESI = rdr["ESI"].ToString();
                        SalDat.ProfTax = rdr["ProfTax"].ToString();
                        SalDat.PFAmount = rdr["PFAmount"].ToString();
                        SalDat.PFNumber = rdr["PFNumber"].ToString();
                        SalDat.PAN = rdr["PAN"].ToString();
                        SalDat.Total = rdr["Total"].ToString();
                        SalDat.Name = rdr["Name"].ToString();
                        SalDat.Designation = rdr["Designation"].ToString();
                        SalDat.MonthnYear = rdr["MonthnYear"].ToString();
                        string path = Server.MapPath("~/ProfileImg/P" + SalDat.UserID + ".jpg");
                        SalDat.UserImg = System.IO.File.Exists(path) ? "P" + SalDat.UserID : "nobody";
                        EmployeeSalarydata.Add(SalDat);

                    }
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    Context.Response.Write(js.Serialize(EmployeeSalarydata));


                }
                catch (Exception)
                {
                    sqlitecon.Close();

                }
            }
        }
        [WebMethod]
        public List<JobDetails> GetVacancies()
        {
            List<JobDetails> VacanciesList = new List<JobDetails>();

            using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
            {
                SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM TBL_Jobs ", sqlitecon);
                sqlitecon.Open();
                SQLiteDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    JobDetails jobdetails = new JobDetails();
                    jobdetails.Id = Convert.ToInt32(rdr["Id"]);
                    jobdetails.JobTitle = rdr["Title"].ToString();
                    jobdetails.AboutJob = rdr["AboutJob"].ToString();
                    jobdetails.Industry = rdr["Industry"].ToString();
                    jobdetails.FunctionalArea = rdr["FunctionalArea"].ToString();
                    jobdetails.Role = rdr["Role"].ToString();
                    jobdetails.JobKeyskills = rdr["Keyskills"].ToString();
                    jobdetails.JobLocation = rdr["Location"].ToString();
                    jobdetails.JobSalary = rdr["Salary"].ToString();
                    jobdetails.DesiredCandidateDetails = rdr["Cand_Details"].ToString();
                    jobdetails.CandidateEducationDetails = rdr["Cand_Edu_Details"].ToString();
                    jobdetails.CandidateLanguageKnown = rdr["LanguageKnown"].ToString();
                    jobdetails.CandidateExperience = rdr["Experience"].ToString();
                    VacanciesList.Add(jobdetails);
                }


            }
          
            return VacanciesList;

        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void DelSalaryData(string ID)
        {

            using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
            {
                try
                {
                    sqlitecon.Open();
                    SQLiteCommand cmd = new SQLiteCommand("Delete from TBL_Payslips where PayslipID='" + ID + "'", sqlitecon);
                    cmd.ExecuteNonQuery();
                    sqlitecon.Close();
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    Context.Response.Write(js.Serialize("success"));


                }
                catch (Exception)
                {
                    sqlitecon.Close();

                }
            }
        }
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        public static DateTime ConvertDate(string myDate)
        {
            DateTime result = DateTime.MinValue;
            if (myDate != null)
            {
                try
                {
                    result = DateTime.Parse(myDate);

                    //result = TimeZoneInfo.ConvertTimeFromUtc(result, INDIAN_ZONE);
                }
                catch (Exception)
                {
                    throw new ApplicationException("DateTime conversion error");
                }
            }
            return (result);
        }


    }

}
