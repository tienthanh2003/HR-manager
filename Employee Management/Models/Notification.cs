using System;
using System.Collections.Generic;

namespace Employee_Management.Models;

public partial class Notification
{
    public int NotificationId { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public int? SenderId { get; set; }

    public bool IsBroadcast { get; set; }

    public DateTime SentDate { get; set; }

    public DateTime EndDate { get; set; }

    public virtual ICollection<NotificationDepartment> NotificationDepartments { get; set; } = new List<NotificationDepartment>();

    public virtual User? Sender { get; set; }

    
}
