using Employee_Management.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employee_Management.Repository
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly EmployeeManagementContext _context;
        public NotificationRepository() {
            _context = new EmployeeManagementContext();
        }

        public List<Notification> GetNotificationsForUser(int employeeId)
        {
            // Lấy DepartmentID của nhân viên
            var employee = _context.Employees.Find(employeeId);
            if (employee == null || !employee.DepartmentId.HasValue)
            {
                return new List<Notification>(); // Trả về danh sách rỗng nếu không tìm thấy nhân viên hoặc không có DepartmentID
            }

            var departmentId = employee.DepartmentId.Value;

            // Lấy thông báo dành cho phòng ban của nhân viên và thông báo broadcast
            var notifications = _context.Notifications
                .Where(n => (n.IsBroadcast || n.NotificationDepartments.Any(nd => nd.DepartmentId == departmentId))
                            && n.EndDate >= DateTime.Now) // So sánh với thời gian hiện tại
                .OrderByDescending(n => n.SentDate) // Sắp xếp theo ngày gửi giảm dần
                .ToList();

            return notifications;
        }

        public List<Notification> getAll()
        {
            var noti = _context.Notifications.Where(n => n.EndDate >= DateTime.Now).ToList();
            return noti;
        }

        public void AddNotification(Notification notification)
        {
            try
            {
                // Thêm thông báo vào bảng Notifications
                _context.Notifications.Add(notification);
                _context.SaveChanges();
                
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi thêm thông báo: " + ex.Message);
            }
        }

        public void AddnotifiDepartment(NotificationDepartment department)
        {
            try
            {
                _context.NotificationDepartments.Add(department);
                _context.SaveChanges();

            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
