using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employee_Management.Models
{
    public class AttendanceViewModel
    {
        public int AttendanceId { get; set; }

        public int? EmployeeId { get; set; }

        public DateTime Date { get; set; }

        public TimeOnly? CheckInTime { get; set; }

        public TimeOnly? CheckOutTime { get; set; }

        public decimal? OvertimeHours { get; set; }

        public string AttendStatus { get; set; }

        public string EmployeeName { get; set; }

        public virtual Employee? Employee { get; set; } = null;
    }
}
