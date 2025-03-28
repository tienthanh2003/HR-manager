using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employee_Management.Models
{
    public class SalaryViewModel
    {
        public int SalaryId { get; set; }

        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; }

        public decimal? BaseSalary { get; set; }

        public decimal? Allowance { get; set; }

        public decimal? Bonus { get; set; }

        public decimal? Deduction { get; set; }

        public decimal? TotalSalary { get; set; }

        public int AbsentDays { get; set; } // Số ngày vắng (không phải nghỉ phép)
        public int WorkDays { get; set; } // Số ngày công (đi làm đủ)
        public decimal OvertimeHours { get; set; } // Số giờ làm thêm
        public DateTime StartDate { get; set; }
        public DateTime PaymentDate { get; set; }
        public int RemainingLeaveDays { get; set; }

        public string SalaryStatus { get; set; } = "Pending"!;
        
    }
}
