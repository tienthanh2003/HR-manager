using System;
using System.Collections.Generic;

namespace Employee_Management.Models;

public partial class Leaves
{
    public int LeaveId { get; set; }

    public int EmployeeId { get; set; }

    public string LeaveType { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string Status { get; set; } = null!;

    public virtual Employee? Employee { get; set; }
}
