using System;
using System.Collections.Generic;

namespace Employee_Management.Models;

public partial class Attendance
{
    public int AttendanceId { get; set; }

    public int? EmployeeId { get; set; }

    public DateTime Date { get; set; }

    public TimeOnly? CheckInTime { get; set; }

    public TimeOnly? CheckOutTime { get; set; }

    public decimal OvertimeHours { get; set; }

    public string AttendStatus { get; set; }

    public virtual Employee? Employee { get; set; }
}
