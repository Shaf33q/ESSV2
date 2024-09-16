using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ESSV2.Models
{
    public static class Utilities
    {

        public static string IsActive(this HtmlHelper html,
                                         string control,
                                         string action)
        {
            var routeData = html.ViewContext.RouteData;

            var routeAction = (string)routeData.Values["action"];
            var routeControl = (string)routeData.Values["controller"];

            // both must match
            var returnActive = control == routeControl &&
                               action == routeAction;

            return returnActive ? "active" : "";
        }
    }
    public class AllCompData
    {
        public ComputerForm CompData { get; set; }
        public IEnumerable<ComputerDat> CompDatae { get; set; }

    }
    public class ComputerForm
    {
        [Required(ErrorMessage = "Mac Address is Required")]
        public string IP { get; set; }
        [Required(ErrorMessage = "Device Name is Required")]
        public string Device { get; set; }
    }
    public class ListAllEmployees
    {
        public IEnumerable<SelectListItem> Employeename { get; set; }
        public int UserId { get; set; }
    }
    public class ComputerDat
    {
        public string IP { get; set; }
        public string Device { get; set; }
        public int ID { get; set; }
    }
    public class BarChartData
    {
        public string Month { get; set; }
        public string Count { get; set; }
       
    }
    public class AllotedLeaves
    {
        public int UserID { get; set; }
        public string Extra { get; set; }
        public string Medical { get; set; }
        public string RemMedical { get; set; }
        public string Casual { get; set; }
        public string RemCasual { get; set; }
        public string LOP { get; set; }
        
    }
    public class ProfileData
    {
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Addr1 { get; set; }
        public string Addr2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PinCode { get; set; }
        public string Mobile { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime? DOB { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string BldGrp { get; set; }
        public string Designation { get; set; }
        public string Experience { get; set; }
        public string JoinDate { get; set; }
        public string BankName { get; set; }
        public string BankAccNo { get; set; }
        public string PAN { get; set; }
        public string Salary { get; set; }
        
    }
    public class SalaryData
    {
        public int UserID { get; set; }
        public string UserImg { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public string BankName { get; set; }
        public string BankAccNo { get; set; }
        public string PAN { get; set; }
        public string Salary { get; set; }
        public string WorkingDays { get; set; }
        public string LOPAmount { get; set; }
        public int LOPDays { get; set; }
        public string Bonus { get; set; }
        public string Basic { get; set; }
        public string HRA { get; set; }
        public string PFNumber { get; set; }
        public string PFAmount { get; set; }
        public string ESI { get; set; }
        public string ProfTax { get; set; }
        public string Total { get; set; }
        public string PerDay { get; set; }
        public string MonthnYear { get; set; }
        
    }
    public class AllAttendanceData
    {
        public HolidayForm AllHoliday { get; set; }
        public IEnumerable<AttendanceData> AttendanceData { get; set; }
        public IEnumerable<HolidayStats> HolidayStatus { get; set; }

    }
    public class UserAccounts
    {
        [Key]
        public int UserID { get; set; }

        [Required(ErrorMessage = "Email is Required")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required(ErrorMessage = "Username is Required")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Password is Required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required(ErrorMessage = "ConfirmPassword is Required")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Password Mismatch")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage = "Role is Required")]
        public string Role { get; set; }



    }
    public class AttendanceData
    {
        public string LogID { get; set; }
        public int UserID { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime ClockDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:HH:mm:ss tt}")]
        public DateTime Clockin { get; set; }
        [DisplayFormat(DataFormatString = "{0:HH:mm:ss tt}")]
        public DateTime? Clockout { get; set; }


    }
    public class HolidayForm
    {
        [Required(ErrorMessage = "From Date is Required")]
        public DateTime FromDate { get; set; }
        [Required(ErrorMessage = "To Date is Required")]
        public DateTime ToDate { get; set; }
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "Reason is Required")]
        public string Reason { get; set; }


    }
    public class HolidayStats
    {
        public int ReqID { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime AplliedOn { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime FromDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime ToDate { get; set; }
        public string Status { get; set; }
        [DataType(DataType.MultilineText)]
        public string Reason { get; set; }
        public string ApprovedBy { get; set; }
        public int UserID { get; set; }
        public int Days { get; set; }
    }
    public class AllTaskData
    {
        public TaskData MyTask { get; set; }
        public IEnumerable<TaskData> TaskData { get; set; }

    }
    public class TaskData
    {
        public int TaskID { get; set; }
        public string TaskName { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime Deadline { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime DateAlloted { get; set; }
        public string AllotedBy { get; set; }
        public int AllotedTo { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; }
        public string AllotedUser { get; set; }

    }
    public class GetTaskData
    {
        public int TaskID { get; set; }
        public string TaskName { get; set; }       
        public string Deadline { get; set; }       
        public string DateAlloted { get; set; }
        public string AllotedBy { get; set; }
        public int AllotedTo { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; }
        public string Resolution { get; set; }
        
    }
    public class AttCalData
    {
        public int Day { get; set; }
        public int Week { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd}")]
        public DateTime ClockDate { get; set; }
        public string Status { get; set; }



    }
    public class SelectedDayData
    {
        public string ClockIn { get; set; }
        public string ClockOut { get; set; }
        public string Clocktime { get; set; }

    }

    public class Holidays
    {
        public int HolID { get; set; }
        public string Leave_Date { get; set; }
        public string Leave_Info { get; set; }
    }
    public class JobDetails
    {
        public int Id { get; set; }
        public string JobTitle { get; set; }
        public string AboutJob { get; set; }
        public string JobLocation { get; set; }
        public string JobSalary { get; set; }
        public string Industry { get; set; }
        public string FunctionalArea { get; set; }
        public string Role { get; set; }
        public string JobKeyskills { get; set; }
        public string DesiredCandidateDetails { get; set; }
        public string CandidateEducationDetails { get; set; }
        public string CandidateLanguageKnown { get; set; }
        public string CandidateExperience { get; set; }
    }

}