using Employee_Management.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Employee_Management.Repository
{
    public class AttendRepository : IAttendRepository
    {
        private readonly EmployeeManagementContext _context;
        private readonly IActivityLogRepository _activityLogRepository;
        public AttendRepository()
        {
            _context = new EmployeeManagementContext();
            _activityLogRepository = new ActivityLogRepository();
        }

        // Phương thức tự động tạo dữ liệu Attendance mỗi ngày
        public void GenerateDailyAttendance()
        {
            bool isNewAttendanceAdded = false;
            // Lấy ngày hiện tại
            var currentDate = DateTime.Today;

                // Lấy danh sách nhân viên
                var employees = _context.Employees.ToList();

                // Duyệt qua từng nhân viên
                foreach (var employee in employees)
                {
                    // Kiểm tra xem đã có bản ghi Attendance cho nhân viên này trong ngày hôm nay chưa
                    var existingAttendance = _context.Attendances
                        .FirstOrDefault(a => a.EmployeeId == employee.EmployeeId && a.Date == currentDate);

                    // Nếu chưa có, tạo bản ghi mới
                    if (existingAttendance == null)
                    {
                        var newAttendance = new Attendance
                        {
                            EmployeeId = employee.EmployeeId,
                            Date = currentDate,
                            CheckInTime = null, // Chưa check-in
                            OvertimeHours = 0, // Mặc định là 0
                            AttendStatus = "Absent" // Mặc định là "Absent"
                        };

                        // Thêm vào cơ sở dữ liệu
                        _context.Attendances.Add(newAttendance);
                        isNewAttendanceAdded = true;
                    }
                    // Nếu đã tồn tại, bỏ qua (không làm gì cả)
                }

            // Lưu thay đổi vào cơ sở dữ liệu
            _context.SaveChanges();
            if (isNewAttendanceAdded)
            {
                _activityLogRepository.LogActivity(1, "Tạo dữ liệu chấm công", "Hệ thống đã tạo dữ liệu chấm công cho ngày mới");
            }

        }

        public Attendance GetTodayAttendance(int employeeId, DateTime date)
        {
            return _context.Attendances .FirstOrDefault(a => a.EmployeeId == employeeId && a.Date == date);
               
        }

        public void AddAttendance(Attendance attendance)
        {
            _context.Attendances.Add(attendance);
            _context.SaveChanges();
        }

        public void UpdateAttendance(Attendance attendance)
        {
            DateTime today = DateTime.Now.Date;

            // Tìm bản ghi điểm danh theo EmployeeId và ngày
            var existingAttendance = _context.Attendances
                .FirstOrDefault(a => a.EmployeeId == attendance.EmployeeId && a.Date== today);

            if (existingAttendance != null)
            {
               
                existingAttendance.CheckInTime = attendance.CheckInTime;
                existingAttendance.CheckOutTime = attendance.CheckOutTime;
                existingAttendance.OvertimeHours = attendance.OvertimeHours;
                
                _context.SaveChanges();
            }
        }

        public int CountAttendDays(int employeeId, DateTime startDate, DateTime endDate)
        {
            return _context.Attendances
                .Count(a => a.EmployeeId == employeeId &&
                            a.AttendStatus == "Attend" &&
                            a.Date >= startDate &&
                            a.Date <= endDate);
        }

        public decimal CalculateTotalOvertime(int employeeId, DateTime startDate, DateTime endDate)
        {
            return _context.Attendances
                .Where(a => a.EmployeeId == employeeId &&
                            a.Date >= startDate &&
                            a.Date <= endDate)
                .Sum(a => a.OvertimeHours);
        }

        public int CountAbsentDays(int employeeId, DateTime startDate, DateTime endDate)
        {
            return _context.Attendances
                .Count(a => a.EmployeeId == employeeId &&
                            a.AttendStatus == "Absent" &&
                            a.Date >= startDate &&
                            a.Date <= endDate);
        }

        public List<AttendanceViewModel> GetAttendancesWithEmployeeName()
        {
            return _context.Attendances
                .Join(_context.Employees,
                    a => a.EmployeeId,
                    e => e.EmployeeId,
                    (a, e) => new AttendanceViewModel
                    {
                        EmployeeName = e.FullName,
                        Date = a.Date,
                        CheckInTime = a.CheckInTime,
                        CheckOutTime = a.CheckOutTime,
                        OvertimeHours = a.OvertimeHours,
                        AttendStatus = a.AttendStatus
                    })
                .ToList();
        }


    }
}