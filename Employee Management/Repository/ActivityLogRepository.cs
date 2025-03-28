using Employee_Management.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employee_Management.Repository
{
    public class ActivityLogRepository : IActivityLogRepository
    {
        private readonly EmployeeManagementContext _context;

        public ActivityLogRepository()
        {
            _context = new EmployeeManagementContext();
        }

        // Ghi lại hành động của người dùng
        public void LogActivity(int userId, string action, string details)
        {
            var log = new ActivityLog
            {
                UserId = userId,
                Action = action,
                ActionDate = DateTime.Now,
                Details = details
            };

            _context.ActivityLogs.Add(log);
            _context.SaveChanges();
        }

        // Lấy lịch sử hoạt động của một người dùng
        public List<ActivityViewModel> GetActivityByUserId(int userId)
        {
            return _context.ActivityLogs
                          .Join(
                              _context.Users, // Bảng Users
                              log => log.UserId, // Khóa ngoại từ ActivityLogs
                              user => user.UserId, // Khóa chính từ Users
                              (log, user) => new ActivityViewModel // Tạo đối tượng ViewModel
                              {
                                  LogId = log.LogId,
                                  UserId = log.UserId,
                                  Username = user.Username, // Lấy Username từ bảng Users
                                  Action = log.Action,
                                  Details = log.Details,
                                  ActionDate = log.ActionDate,
                                  Date = log.ActionDate
                              })
                          .Where(log => log.UserId == userId) // Lọc theo UserId
                          .OrderByDescending(log => log.ActionDate) // Sắp xếp theo thời gian
                          .ToList();
        }

        public List<ActivityViewModel> GetAllActivityLogs()
        {
            return _context.ActivityLogs
                          .Join(
                              _context.Users, // Bảng Users
                              log => log.UserId, // Khóa ngoại từ ActivityLogs
                              user => user.UserId, // Khóa chính từ Users
                              (log, user) => new ActivityViewModel // Tạo đối tượng ViewModel
                              {
                                  LogId = log.LogId,
                                  UserId = log.UserId,
                                  Username = user.Username, // Lấy Username từ bảng Users
                                  Action = log.Action,
                                  Details = log.Details,
                                  ActionDate = log.ActionDate,
                                  Date = log.ActionDate
                              })
                          .OrderByDescending(log => log.ActionDate) // Sắp xếp theo thời gian
                          .ToList();
        }
    }
}
