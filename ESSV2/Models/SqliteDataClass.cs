using System.Web;
using System.Data;
using System.Data.SQLite;
using System;
using System.Collections.Generic;

namespace ESSV2.Models
{
    public class SqliteDataClass
    {
        static string strDB = "\"" + HttpContext.Current.Server.MapPath("~/App_Data/ESSDB.s3db") + "\"";
        string strConnection = @"data source=" + strDB + ";Version=3;";
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");

        public bool RegisterUser(string Email, string Username, string Password,string Role,string date)
        {
            using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
            {
                try
                {
                    sqlitecon.Open();
                    SQLiteCommand cmd = new SQLiteCommand("insert into TBL_Userdata(Email,Username,Password,Role,JoinDate)values('" + Email + "','" + Username + "','" + Password + "','" + Role + "','"+ date + "')", sqlitecon);
                    cmd.ExecuteNonQuery();
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
        //public bool UserRequest(string Email, string Password)
        //{
        //    using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
        //    {
        //        try
        //        {
        //            sqlitecon.Open();
        //            SQLiteCommand cmd = new SQLiteCommand("insert into TBL_UserRequest(Email,Password,Status)values('" + Email + "','" + Password + "','Pending')", sqlitecon);
        //            cmd.ExecuteNonQuery();
        //            sqlitecon.Close();
        //            return true;
        //        }
        //        catch (Exception)
        //        {
        //            sqlitecon.Close();
        //            return false;
        //        }
        //    }
        //}
        public bool UpdateUser(int id,string Email, string Username, string Password, string Role)
        {
            using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
            {
                try
                {
                    sqlitecon.Open();
                    SQLiteCommand cmd = new SQLiteCommand("Update TBL_Userdata SET Email='"+Email+"',Username='"+ Username + "',Password='"+ Password + "',Role='"+ Role + "' where ID='" + id + "'", sqlitecon);
                    cmd.ExecuteNonQuery();
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
        public bool UpdateLeaves(int LOP,int RemMed,int RemCas,int Extra, int UserID)
        {
            using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
            {
                try
                {
                    
                    sqlitecon.Open();
                    SQLiteCommand cmd = new SQLiteCommand("Update TBL_AllotedLeaves SET LOP='" + LOP + "', RemMedical='" + RemMed + "',RemCasual='" + RemCas + "',Extra='" + Extra + "' where UserID='" + UserID + "'", sqlitecon);
                    cmd.ExecuteNonQuery();
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
        public bool InsertHolidays(string Desc, string Date)
        {
            using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
            {
                try
                {
                    sqlitecon.Open();
                    SQLiteCommand cmd = new SQLiteCommand("insert or replace into TBL_Holidays(Date,Desc)values('" + Date + "','" + Desc + "')", sqlitecon);
                    cmd.ExecuteNonQuery();
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
       
        public bool AttStatus(string Dayid, int ID, string Date,string ClockType,int Week,int day)
        {
            using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
            {
                try
                {
                    sqlitecon.Open();
                    SQLiteCommand cmd = new SQLiteCommand("insert or replace into TBL_Attendance(DayID,UserID,AttDate,Status,Week,Day)values('" + Dayid + "','" + ID + "','" + Date + "','" + ClockType + "','" + Week + "','" + day + "')", sqlitecon);
                    cmd.ExecuteNonQuery();
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
        public bool DelHoliday(int ID)
        {
            using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
            {
                try
                {
                    sqlitecon.Open();
                    SQLiteCommand cmd = new SQLiteCommand("Delete from TBL_Holidays where HOLID='" + ID + "'", sqlitecon);
                    cmd.ExecuteNonQuery();
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
        public bool DelHolidayRequest(int ID)
        {
            using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
            {
                try
                {
                    sqlitecon.Open();
                    SQLiteCommand cmd = new SQLiteCommand("Delete from TBL_HolidayRequest where ReqID='" + ID + "'", sqlitecon);
                    cmd.ExecuteNonQuery();
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
        public bool DelHolStats(string date,string status)
        {
            using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
            {
                try
                {
                    sqlitecon.Open();
                    SQLiteCommand cmd = new SQLiteCommand("Delete from TBL_Attendance where Status='"+ status + "'and AttDate='" + date + "'", sqlitecon);
                    cmd.ExecuteNonQuery();
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
        
        public int GetHolidaysinMonth(string Month)
        {
            using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
            {
                try
                {
                    sqlitecon.Open();
                    SQLiteCommand cmd = new SQLiteCommand("SELECT Count(*) FROM TBL_Holidays where strftime('%m', Date)='"+ Month + "'and strftime('%w', Date) NOT IN('0', '6')", sqlitecon);
                    int Holiday = Convert.ToInt32(cmd.ExecuteScalar());
                    sqlitecon.Close();
                    return Holiday;
                }
                catch (Exception)
                {
                    sqlitecon.Close();
                    return 0;
                }
            }
        }
        public string GetClosestHoliday(string Today)
        {
            using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
            {
                try
                {
                    sqlitecon.Open(); 
                    SQLiteCommand cmd = new SQLiteCommand("SELECT Date FROM TBL_Holidays WHERE Date > '" + Today + "' ORDER BY Date Limit 1", sqlitecon);
                    string Holiday = Convert.ToString(cmd.ExecuteScalar());
                    sqlitecon.Close();
                    return Holiday;
                }
                catch (Exception)
                {
                    sqlitecon.Close();
                    return "";
                }
            }
        }
        public bool DelRouters(int ID)
        {
            using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
            {
                try
                {
                    sqlitecon.Open();
                    SQLiteCommand cmd = new SQLiteCommand("Delete from TBL_WhiteList where ID='" + ID + "'", sqlitecon);
                    cmd.ExecuteNonQuery();
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
        public bool AttService(string Dayid, int ID, string Date,int Week,int Day)
        {
            using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
            {
                try
                {
                    sqlitecon.Open();
                    SQLiteCommand cmd = new SQLiteCommand("INSERT OR IGNORE INTO TBL_Attendance(DayID,UserID,AttDate,Status,Week,Day)values('" + Dayid + "','" + ID + "','" + Date + "','AB','" + Week + "','" + Day + "')", sqlitecon);
                    int i=cmd.ExecuteNonQuery();
                    sqlitecon.Close();
                    return Convert.ToBoolean(i);
                }
                catch (Exception)
                {
                    sqlitecon.Close();
                    return false;
                }
            }
        }
        public int GetNumOfEmployees()
        {
            using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
            {
                try
                {
                    sqlitecon.Open();
                    SQLiteCommand cmd = new SQLiteCommand("Select count(*) from TBL_Userdata", sqlitecon);
                    int employees = Convert.ToInt32(cmd.ExecuteScalar());
                    sqlitecon.Close();
                    return employees;
                }
                catch (Exception)
                {
                    sqlitecon.Close();
                    return 0;
                }
            }
        }
        public int GetPaySlipsGen(string MonYear)
        {
            using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
            {
                try
                {
                    sqlitecon.Open();
                    string[] MY = MonYear.Split('-');
                    SQLiteCommand cmd = new SQLiteCommand(" select count(*) from TBL_Payslips Where strftime('%m', MonthnYear)='" + MY[1] + "' and strftime('%Y', MonthnYear)='" + MY[0] + "'", sqlitecon);
                    int Tasks = Convert.ToInt32(cmd.ExecuteScalar());
                    sqlitecon.Close();
                    return Tasks;
                }
                catch (Exception)
                {
                    sqlitecon.Close();
                    return 0;
                }
            }
        }
        public int GetIncompleteTasks()
        {
            using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
            {
                try
                {
                    sqlitecon.Open();
                    SQLiteCommand cmd = new SQLiteCommand("select count(*) from TBL_TasksAlloted where Status = 'Inprogress'", sqlitecon);
                    int Tasks = Convert.ToInt32(cmd.ExecuteScalar());
                    sqlitecon.Close();
                    return Tasks;
                }
                catch (Exception)
                {
                    sqlitecon.Close();
                    return 0;
                }
            }
        }
        public int GetNumOfHolReqs()
        {
            using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
            {
                try
                {
                    sqlitecon.Open();
                    SQLiteCommand cmd = new SQLiteCommand("Select count(*) from TBL_HolidayRequest where Status='Pending'", sqlitecon);
                    int HolReqs = Convert.ToInt32(cmd.ExecuteScalar());
                    sqlitecon.Close();
                    return HolReqs;
                }
                catch (Exception)
                {
                    sqlitecon.Close();
                    return 0;
                }
            }
        }
        public int GetNumOfIPs()
        {
            using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
            {
                try
                {
                    sqlitecon.Open();
                    SQLiteCommand cmd = new SQLiteCommand("Select count(*) from TBL_WhiteList", sqlitecon);
                    int IPs = Convert.ToInt32(cmd.ExecuteScalar());
                    sqlitecon.Close();
                    return IPs;
                }
                catch (Exception)
                {
                    sqlitecon.Close();
                    return 0;
                }
            }
        }

        public int GetNumOfAbsenties(string Date)
        {
            using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
            {
                try
                {
                    sqlitecon.Open();
                    SQLiteCommand cmd = new SQLiteCommand("select count(*) As Absenties from TBL_Attendance where AttDate = '" + Date + "' and Status = 'AB'", sqlitecon);
                    int absenties = Convert.ToInt32(cmd.ExecuteScalar());
                    sqlitecon.Close();
                    return absenties;
                }
                catch (Exception)
                {
                    sqlitecon.Close();
                    return 0;
                }
            }
        }
        public string GetAttStatus(int UserID,string Date)
        {
            using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
            {
                try
                {
                    sqlitecon.Open();
                    SQLiteCommand cmd = new SQLiteCommand("Select Status from TBL_Attendance where AttDate = '"+ Date + "' and UserID = '"+ UserID + "'", sqlitecon);
                    string Status = Convert.ToInt32(cmd.ExecuteScalar()).ToString();
                    sqlitecon.Close();
                    return Status;
                }
                catch (Exception)
                {
                    sqlitecon.Close();
                    return "";
                }
            }
        }

       
        public bool AddMyTask(int ID, string Date,string Summary,string Heading)
        {
            using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
            {
                try
                {
                    sqlitecon.Open();
                    SQLiteCommand cmd = new SQLiteCommand("insert into TBL_MyTasks(UserID,TaskSummary,TaskHeading,Date)values('"+ ID + "','" + Summary + "','" + Heading + "','" + Date + "')", sqlitecon);
                    cmd.ExecuteNonQuery();
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
        public bool HolidayRequest(string AppliedOn, int UserID, string FromDate, string ToDate, string Reason,int Days)
        {
            using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
            {
                try
                {
                    sqlitecon.Open();
                    SQLiteCommand cmd = new SQLiteCommand("insert into TBL_HolidayRequest(AppliedOn,FromDate,ToDate,Reason,UserID,Status,Days)values('"+ AppliedOn + "','"+ FromDate + "','"+ ToDate + "','"+ Reason + "','"+ UserID + "','Pending','" + Days + "')", sqlitecon);
                    cmd.ExecuteNonQuery();
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
        public bool ClockinTime(string Logid, string Date, string Clockin,int UserID)
        {
            using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
            {
                try
                {
                    sqlitecon.Open();
                    SQLiteCommand cmd = new SQLiteCommand("insert into TBL_AttendanceDetails(LogID,ClockDate,ClockIn,UserID)values('" + Logid + "','" + Date + "','" + Clockin + "','"+ UserID + "')", sqlitecon);
                    cmd.ExecuteNonQuery();
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
        public bool AddIP(string IP, string Device)
        {
            using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
            {
                try
                {
                    sqlitecon.Open();
                    SQLiteCommand cmd = new SQLiteCommand("insert into TBL_WhiteList(IP,DeviceName)values('" + IP + "','" + Device + "')", sqlitecon);
                    cmd.ExecuteNonQuery();
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
        public bool ClockoutTime(string Logid, string Clockout)
        {
            using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
            {
                try
                {
                    sqlitecon.Open();
                    SQLiteCommand cmd = new SQLiteCommand("UPDATE TBL_AttendanceDetails SET ClockOut = '" + Clockout + "',ClockTime= (strftime('%s','" + Clockout + "') - strftime('%s',ClockIn)) WHERE LOGID = '" + Logid + "'", sqlitecon);
                    cmd.ExecuteNonQuery();
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
        public bool ApproveHoliday(int id,string Username)
        {
            using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
            {
                try
                {
                    sqlitecon.Open();
                    SQLiteCommand cmd = new SQLiteCommand(" Update TBL_HolidayRequest SET ApprovedBy='"+ Username + "',Status = 'Approved' where ReqID = '"+ id + "'", sqlitecon);
                    cmd.ExecuteNonQuery();
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
        public bool DisApproveHoliday(int id, string Username)
        {
            using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
            {
                try
                {
                    sqlitecon.Open();
                    SQLiteCommand cmd = new SQLiteCommand(" Update TBL_HolidayRequest SET ApprovedBy='" + Username + "',Status = 'Disapproved' where ReqID = '" + id + "'", sqlitecon);
                    cmd.ExecuteNonQuery();
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
        public bool CompleteTask(int id)
        {
            using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
            {
                try
                {
                    sqlitecon.Open();
                    SQLiteCommand cmd = new SQLiteCommand(" Update TBL_TasksAlloted SET Status = 'Completed' where TaskID = '" + id + "'", sqlitecon);
                    cmd.ExecuteNonQuery();
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
        public bool DeleteTask(int id)
        {
            using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
            {
                try
                {
                    sqlitecon.Open();
                    SQLiteCommand cmd = new SQLiteCommand("Delete from TBL_TasksAlloted where TaskID = '" + id + "'", sqlitecon);
                    cmd.ExecuteNonQuery();
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
        public bool IncompleteTask(int id)
        {
            using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
            {
                try
                {
                    sqlitecon.Open();
                    SQLiteCommand cmd = new SQLiteCommand(" Update TBL_TasksAlloted SET Status = 'Inprogress' where TaskID = '" + id + "'", sqlitecon);
                    cmd.ExecuteNonQuery();
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
        //public bool UpdateRequest(string MacID,string Status)
        //{
        //    using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
        //    {
        //        try
        //        {
        //            sqlitecon.Open();
        //            SQLiteCommand cmd = new SQLiteCommand(" Update TBL_UserRequest SET Status = '"+ Status + "' where MacID = '" + MacID + "'", sqlitecon);
        //            cmd.ExecuteNonQuery();
        //            sqlitecon.Close();
        //            return true;
        //        }
        //        catch (Exception)
        //        {
        //            sqlitecon.Close();
        //            return false;
        //        }
        //    }
        //}
        public bool DeleteUser(int id)
        {
            using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
            {
                try
                {
                    sqlitecon.Open();
                    SQLiteCommand cmd = new SQLiteCommand(" Delete from TBL_Userdata where ID='" + id + "'", sqlitecon);
                    cmd.ExecuteNonQuery();
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
       
    public IEnumerable<Holidays> GetHolidays
{
    get
    {

        List<Holidays> AllHolidays = new List<Holidays>();

        using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
        {
            DataSet ValidateLogin = new DataSet();
            SQLiteCommand cmd = new SQLiteCommand("select * from TBL_Holidays", sqlitecon);
            sqlitecon.Open();
            SQLiteDataReader rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                Holidays HolData = new Holidays();
                HolData.HolID = Convert.ToInt32(rdr["HOLID"]);
                DateTime HolDate = Convert.ToDateTime(rdr["Date"]);
                HolData.Leave_Date = HolDate.ToString("yyyy-MM-dd");
                HolData.Leave_Info = rdr["Desc"].ToString();
                AllHolidays.Add(HolData);
            }


        }
        return AllHolidays;
    }

}
public IEnumerable<AttendanceData> Attendance
        {
            get
            {

                List<AttendanceData> AttendanceDataResult = new List<AttendanceData>();

                using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
                {
                    DataSet ValidateLogin = new DataSet();
                    SQLiteCommand cmd = new SQLiteCommand("SELECT LogID,UserID,ClockDate,ClockIn,ClockOut  FROM TBL_AttendanceDetails", sqlitecon);
                    sqlitecon.Open();
                    SQLiteDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        AttendanceData AttData = new AttendanceData();
                        AttData.LogID = rdr["LogID"].ToString();
                        AttData.UserID = Convert.ToInt32(rdr["UserID"]);
                        AttData.ClockDate = Convert.ToDateTime(rdr["ClockDate"]);
                        AttData.Clockin = Convert.ToDateTime(rdr["ClockIn"]);
                        string AttClockOut = rdr["ClockOut"].ToString();
                        AttData.Clockout = string.IsNullOrEmpty(AttClockOut) ? (DateTime?)null : Convert.ToDateTime(AttClockOut);
                        AttendanceDataResult.Add(AttData);
                    }


                }
                return AttendanceDataResult;
            }

        }
        public IEnumerable<UserAccounts> Employees
        {
            get
            {

                List<UserAccounts> employeesdata = new List<UserAccounts>();
                
                using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
                {
                    
                    SQLiteCommand cmd = new SQLiteCommand("select * from TBL_Userdata", sqlitecon);
                    sqlitecon.Open();
                    SQLiteDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        UserAccounts employee = new UserAccounts();
                        employee.UserID = Convert.ToInt32(rdr["ID"]);
                        employee.Email = rdr["Email"].ToString();
                        employee.Username = rdr["Username"].ToString();
                        employee.Password = rdr["Password"].ToString();
                        employee.Role = rdr["Role"].ToString();
                        employeesdata.Add(employee);
                    }


                }
                return employeesdata;
            }

        }

        public IEnumerable<AllotedLeaves> EmployeeLeaves
        {
            get
            {

                List<AllotedLeaves> Leavesdata = new List<AllotedLeaves>();

                using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
                {
                    DataSet ValidateLogin = new DataSet();
                    SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM TBL_AllotedLeaves", sqlitecon);
                    sqlitecon.Open();
                    SQLiteDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        AllotedLeaves Leaves = new AllotedLeaves();
                        Leaves.UserID = Convert.ToInt32(rdr["UserID"]);
                        Leaves.Extra = rdr["Extra"].ToString();
                        Leaves.Medical = rdr["Medical"].ToString();
                        Leaves.RemMedical = rdr["RemMedical"].ToString();
                        Leaves.Casual = rdr["Casual"].ToString();
                        Leaves.RemCasual = rdr["RemCasual"].ToString();
                        Leaves.LOP = rdr["LOP"].ToString();
                        Leavesdata.Add(Leaves);
                    }


                }
                return Leavesdata;
            }

        }
        public IEnumerable<UserAccounts> Absenties
        {
            get
            {
                DateTime indianTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                List<UserAccounts> employeesdata = new List<UserAccounts>();

                using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
                {

                    SQLiteCommand cmd = new SQLiteCommand("select * from TBL_Attendance TA inner Join TBL_Userdata TU on TA.UserID= TU.ID where  TA.AttDate= '"+indianTime.ToString("yyyy-MM-dd")+ "' and TA.Status='AB'", sqlitecon);
                    sqlitecon.Open();
                    SQLiteDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        UserAccounts employee = new UserAccounts();
                        employee.UserID = Convert.ToInt32(rdr["ID"]);
                        employee.Email = rdr["Email"].ToString();
                        employee.Username = rdr["Username"].ToString();
                        employee.Password = rdr["Password"].ToString();
                        employee.Role = rdr["Role"].ToString();
                        employeesdata.Add(employee);
                    }


                }
                return employeesdata;
            }

        }
        public IEnumerable<ProfileData> EmployeeProfile
        {
            get
            {

                List<ProfileData> EmployeeProfilesdata = new List<ProfileData>();

                using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
                {
                    DataSet ValidateLogin = new DataSet();
                    SQLiteCommand cmd = new SQLiteCommand("SELECT * FROM TBL_Profile INNER JOIN TBL_Userdata ON TBL_Profile.UserID = TBL_Userdata.ID", sqlitecon);
                    sqlitecon.Open();
                    SQLiteDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        ProfileData profile = new ProfileData();
                        profile.UserID = Convert.ToInt32(rdr["UserID"]);
                        profile.FirstName = rdr["FirstName"].ToString();
                        profile.LastName = rdr["LastName"].ToString();
                        profile.Email = rdr["Email"].ToString();
                        profile.Addr1 = rdr["Addrline1"].ToString();
                        profile.Addr2 = rdr["Addrline2"].ToString();
                        profile.Mobile = rdr["Mobile"].ToString();
                        profile.PinCode = rdr["Pincode"].ToString();
                        profile.State = rdr["State"].ToString();
                        profile.City = rdr["City"].ToString();
                        profile.UserName = rdr["Username"].ToString();
                        profile.Password = rdr["Password"].ToString();
                        profile.Role = rdr["Role"].ToString();
                        profile.BldGrp = rdr["BloodGroup"].ToString();
                        profile.Designation = rdr["Designation"].ToString();
                        profile.JoinDate = rdr["JoinDate"].ToString();
                        profile.BankName = rdr["BankName"].ToString();
                        profile.BankAccNo = rdr["BankAccNo"].ToString();
                        profile.PAN = rdr["PAN"].ToString();
                        profile.Salary = rdr["Salary"].ToString();
                        if (profile.JoinDate != "")
                        {
                            DateTime JoinDate = Convert.ToDateTime(rdr["JoinDate"]);
                            profile.JoinDate = JoinDate.ToString();
                            profile.Experience = Math.Abs((DateTime.Now.Month - JoinDate.Month) + 12 * (DateTime.Now.Year - JoinDate.Year)).ToString();
                        }
                        else
                        {
                            profile.JoinDate = "";
                            profile.Experience = "";
                        }
                       
                       
                        string DOB = rdr["DOB"].ToString();
                        profile.DOB = string.IsNullOrEmpty(DOB) ? (DateTime?)null : Convert.ToDateTime(DOB);


                        EmployeeProfilesdata.Add(profile);
                    }


                }
                return EmployeeProfilesdata;
            }

        }
        public IEnumerable<SalaryData> EmployeeSalary
        {
            get
            {

                List<SalaryData> EmployeeSalarydata = new List<SalaryData>();

                using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
                {
                    
                    SQLiteCommand cmd = new SQLiteCommand("Select UserID,FirstName,LastName,Designation,Salary,BankName,BankAccNo,PAN,PF from TBL_Profile", sqlitecon);
                    sqlitecon.Open();
                    SQLiteDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        SalaryData SalDat = new SalaryData();
                        SalDat.UserID = Convert.ToInt32(rdr["UserID"]);
                        SalDat.Name = rdr["FirstName"].ToString()+" "+rdr["LastName"].ToString();
                        SalDat.PAN = rdr["PAN"].ToString();
                        SalDat.BankName = rdr["BankName"].ToString();
                        SalDat.BankAccNo = rdr["BankAccNo"].ToString();
                        SalDat.Designation = rdr["Designation"].ToString();
                        SalDat.Salary = rdr["Salary"].ToString();
                        EmployeeSalarydata.Add(SalDat);
                    }


                }
                return EmployeeSalarydata;
            }

        }
        public IEnumerable<TaskData> GetTaskData
        {
            get
            {

                List<TaskData> TaskDatas = new List<TaskData>();

                using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
                {
                    SQLiteCommand cmd = new SQLiteCommand("select  TAS.*,PRO.UserName from TBL_TasksAlloted TAS inner join  TBL_Userdata PRO on TAS.AllotedTo = PRO.ID  order by TAS.DateAlloted  desc", sqlitecon);
                    sqlitecon.Open();
                    SQLiteDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        TaskData Task = new TaskData();
                        Task.TaskID = Convert.ToInt32(rdr["TaskID"]);
                        Task.TaskName = rdr["TaskName"].ToString();
                        Task.Description = rdr["Description"].ToString();
                        Task.Deadline = Convert.ToDateTime(rdr["Deadline"]);
                        Task.DateAlloted = Convert.ToDateTime(rdr["DateAlloted"]);
                        Task.AllotedBy = rdr["AllotedBy"].ToString();
                        Task.Status = rdr["Status"].ToString();
                        Task.AllotedTo = Convert.ToInt32(rdr["AllotedTo"]);
                        Task.Priority = rdr["Priority"].ToString();
                        Task.AllotedUser = rdr["UserName"].ToString();
                        TaskDatas.Add(Task);
                    }

                }
                return TaskDatas;
            }

        }
        //public IEnumerable<MyTaskData> GetMyTaskData
        //{
        //    get
        //    {

        //        List<MyTaskData> TaskDatas = new List<MyTaskData>();

        //        using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
        //        {
        //            SQLiteCommand cmd = new SQLiteCommand("select * from TBL_MyTasks", sqlitecon);
        //            sqlitecon.Open();
        //            SQLiteDataReader rdr = cmd.ExecuteReader();
        //            while (rdr.Read())
        //            {
        //                MyTaskData Task = new MyTaskData();
        //                Task.MyTaskID = Convert.ToInt32(rdr["MyTaskID"]);
        //                Task.UserID = Convert.ToInt32(rdr["UserID"]);
        //                Task.MyTaskHeading = rdr["TaskHeading"].ToString();
        //                Task.MyTaskSummary = rdr["TaskSummary"].ToString();
        //                Task.Date = Convert.ToDateTime(rdr["Date"]);
        //                TaskDatas.Add(Task);
        //            }

        //        }
        //        return TaskDatas;
        //    }

        //}
        public IEnumerable<ComputerDat> GetRouters
        {
            get
            {

                List<ComputerDat> CompIDs = new List<ComputerDat>();

                using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
                {
                    SQLiteCommand cmd = new SQLiteCommand("select * from TBL_WhiteList", sqlitecon);
                    sqlitecon.Open();
                    SQLiteDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        ComputerDat CompID = new ComputerDat();
                        CompID.IP = rdr["IP"].ToString();
                        CompID.Device = rdr["DeviceName"].ToString();
                        CompID.ID = Convert.ToInt32(rdr["ID"].ToString());
                        CompIDs.Add(CompID);
                    }

                }
                return CompIDs;
            }

        }
        
        public IEnumerable<BarChartData> BarChartData(int UserID)     
        {
                List<BarChartData> ChtDatas = new List<BarChartData>();

                using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
                {
                    SQLiteCommand cmd = new SQLiteCommand("SELECT Count(*) As Count, strftime('%m', AttDate) as Month FROM TBL_Attendance where UserID='"+ UserID + "' Group by strftime('%m', AttDate)", sqlitecon);
                    sqlitecon.Open();
                    SQLiteDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        BarChartData ChtData = new BarChartData();
                        ChtData.Count = rdr["Count"].ToString();
                        ChtData.Month = rdr["Month"].ToString();
                        ChtDatas.Add(ChtData);
                    }

                }
                return ChtDatas;
            

        }
        public SalaryData GetPaySlipData(int UserID,string Month)
        {

            SalaryData SalDat = new SalaryData();
            using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
            {
               
                SQLiteCommand cmd = new SQLiteCommand("select * from TBL_Payslips where UserID='"+ UserID +"' and MonthnYear='"+ Month + "'", sqlitecon);
                sqlitecon.Open();
                SQLiteDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                  
                    SalDat.UserID = Convert.ToInt32(rdr["UserID"]);
                    SalDat.Salary = rdr["Salary"].ToString();
                    SalDat.MonthnYear = (Convert.ToDateTime(rdr["MonthnYear"])).ToString("yyyy-MM-dd");
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

                  
                }

            }
            return SalDat;


        }
        public string RemainingLeaves(int UserID)
        {

            string LeaveDay=string.Empty;
            using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
            {
                SQLiteCommand cmd = new SQLiteCommand(" select RemMedical, RemCasual,Extra,LOP from TBL_AllotedLeaves where UserID = '"+ UserID + "'", sqlitecon);
                sqlitecon.Open();
                SQLiteDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {                    
                    LeaveDay = rdr["RemMedical"].ToString()+"/"+ rdr["RemCasual"].ToString() + "/" + rdr["Extra"].ToString() + "/" + rdr["LOP"].ToString();
                 }

            }
            return LeaveDay;


        }
        public string GetAllotedLeaves(int UserID)
        {

            string LeaveDay = string.Empty;
            using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
            {
                SQLiteCommand cmd = new SQLiteCommand(" select Medical, Casual from TBL_AllotedLeaves where UserID = '" + UserID + "'", sqlitecon);
                sqlitecon.Open();
                SQLiteDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    LeaveDay = rdr["Medical"].ToString() + "/" + rdr["Casual"].ToString();
                }

            }
            return LeaveDay;


        }
        //public IEnumerable<UserReqestData> GetRequestData
        //{
        //    get
        //    {

        //        List<UserReqestData> ReqDatas = new List<UserReqestData>();

        //        using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
        //        {
        //            SQLiteCommand cmd = new SQLiteCommand("select * from TBL_UserRequest", sqlitecon);
        //            sqlitecon.Open();
        //            SQLiteDataReader rdr = cmd.ExecuteReader();
        //            while (rdr.Read())
        //            {
        //                UserReqestData ReqData = new UserReqestData();

        //                ReqData.Email = rdr["Email"].ToString();
        //                ReqData.Password = rdr["Password"].ToString();
        //                ReqData.Status = rdr["Status"].ToString();
        //                ReqDatas.Add(ReqData);
        //            }

        //        }
        //        return ReqDatas;
        //    }

        //}
        public IEnumerable<HolidayStats> GetHolidayStatus
        {
            get
            {

                List<HolidayStats> HoliDatas = new List<HolidayStats>();

                using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
                {
                    SQLiteCommand cmd = new SQLiteCommand("select * from TBL_HolidayRequest", sqlitecon);
                    sqlitecon.Open();
                    SQLiteDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        HolidayStats HoliData = new HolidayStats();
                        HoliData.ReqID = Convert.ToInt32(rdr["ReqID"]);
                        HoliData.AplliedOn = Convert.ToDateTime(rdr["AppliedOn"]);
                        HoliData.FromDate = Convert.ToDateTime(rdr["FromDate"]);
                        HoliData.ToDate = Convert.ToDateTime(rdr["ToDate"]);
                        HoliData.Status = rdr["Status"].ToString();
                        HoliData.Reason = rdr["Reason"].ToString();
                        HoliData.ApprovedBy = rdr["ApprovedBy"].ToString();
                        HoliData.UserID = Convert.ToInt32(rdr["UserID"]);
                        HoliData.Days = Convert.ToInt32(rdr["Days"]);
                        HoliDatas.Add(HoliData);
                    }

                }
                return HoliDatas;
            }

        }
        public IEnumerable<SalaryData> PaySlips
        {
            get
            {

                List<SalaryData> EmployeeSalarydata = new List<SalaryData>();

                using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
                {
                    SQLiteCommand cmd = new SQLiteCommand("select * from TBL_Payslips", sqlitecon);
                    sqlitecon.Open();
                    SQLiteDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        SalaryData SalDat = new SalaryData();
                        SalDat.UserID = Convert.ToInt32(rdr["UserID"]);
                        SalDat.Salary = rdr["Salary"].ToString();
                        SalDat.MonthnYear = rdr["MonthnYear"].ToString();
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
                  
                        EmployeeSalarydata.Add(SalDat);
                    }

                }
                return EmployeeSalarydata;
            }

        }
        public bool InsertVacancies(string Title, string AboutJob, string Industry, string FunctionalArea, string Role, string Keyskills, string Location, string Salary, string CandidateDetails, string EducationDetails, string LanguageKnown, string Experience)
        {
            using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
            {
                try
                {
                    sqlitecon.Open();
                    SQLiteCommand cmd = new SQLiteCommand("insert or replace into TBL_Jobs(Title,AboutJob,Industry,FunctionalArea,Role,Keyskills,Location,Salary,Cand_Details,Cand_Edu_Details,LanguageKnown,Experience) values('" + Title + "','" + AboutJob + "','" + Industry + "','" + FunctionalArea + "','" + Role + "','" + Keyskills + "','" + Location + "','" + Salary + "','" + CandidateDetails + "','" + EducationDetails + "','" + LanguageKnown + "','" + Experience + "')", sqlitecon);
                    cmd.ExecuteNonQuery();
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
        public IEnumerable<JobDetails> GetVacancies
        {
            get
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

        }
        public bool DelVacancy(int ID)
        {
            using (SQLiteConnection sqlitecon = new SQLiteConnection(strConnection))
            {
                try
                {
                    sqlitecon.Open();
                    SQLiteCommand cmd = new SQLiteCommand("Delete from TBL_Jobs where Id='" + ID + "'", sqlitecon);
                    cmd.ExecuteNonQuery();
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
    }
}