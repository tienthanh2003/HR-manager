using System;
using System.Collections.Generic;

namespace Employee_Management.Models;

public partial class NotificationDepartment
{
    public int NotificationDepartmentId { get; set; }

    public int? NotificationId { get; set; }

    public int? DepartmentId { get; set; }

    public virtual Department? Department { get; set; }

    public virtual Notification? Notification { get; set; }

    // Danh sách các phòng ban được chọn
    
}
