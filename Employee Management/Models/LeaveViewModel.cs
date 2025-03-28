using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employee_Management.Models
{
    public class LeaveViewModel
    {
        public int LeaveID { get; set; }
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string LeaveType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public int RemainingLeaveDays { get; set; }
    }
}
