using System;
using System.Collections.Generic;

namespace Employee_Management.Models;

public partial class Department
{
    public int DepartmentId { get; set; }

    public string DepartmentName { get; set; } = null!;

    public string Status { get; set; } = null!;

    public int NumberOfEmployees { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<NotificationDepartment> NotificationDepartments { get; set; } = new List<NotificationDepartment>();
}
