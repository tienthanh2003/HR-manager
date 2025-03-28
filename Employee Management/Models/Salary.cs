using System;
using System.Collections.Generic;

namespace Employee_Management.Models;

public partial class Salary
{
    public int SalaryId { get; set; }

    public int EmployeeId { get; set; }

    public decimal? BaseSalary { get; set; }

    public decimal? Allowance { get; set; }

    public decimal? Bonus { get; set; }

    public decimal? Deduction { get; set; }

    public decimal? TotalSalary { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime PaymentDate { get; set; }

    public string SalaryStatus { get; set; } = "Pending"!;
    public virtual Employee? Employee { get; set; }
}
