using Employee_Management.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employee_Management.Repository
{
    public interface INotificationRepository
    {

        public List<Notification> GetNotificationsForUser(int employeeId);
        public void AddNotification(Notification notification);
        public void AddnotifiDepartment(NotificationDepartment department);
        public List<Notification> getAll();
    }
}
