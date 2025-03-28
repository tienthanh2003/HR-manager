using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Employee_Management.Models;

namespace Employee_Management.Repository
{
    public class LeaveRepository : ILeaveRepository
    {
        private readonly EmployeeManagementContext _context;
        public LeaveRepository() {
            _context = new EmployeeManagementContext();
        }
        public List<LeaveViewModel> GetAllLeaves()
        {
            // Lấy tháng và năm hiện tại
            var currentDate = DateTime.Now;
            var currentMonth = currentDate.Month;
            var currentYear = currentDate.Year;

            var leaves = (from l in _context.Leaves
                          join e in _context.Employees on l.EmployeeId equals e.EmployeeId
                          where l.EmployeeId != null // Loại bỏ các EmployeeID null
                          where l.StartDate.Month == currentMonth && l.StartDate.Year == currentYear // Chỉ lấy đơn trong tháng hiện tại
                          select new LeaveViewModel
                          {
                              LeaveID = l.LeaveId,
                              EmployeeID = l.EmployeeId,
                              EmployeeName = e.FullName,
                              LeaveType = l.LeaveType,
                              StartDate = l.StartDate,
                              EndDate = l.EndDate,
                              RemainingLeaveDays = e.RemainingLeaveDays,
                              Status = l.Status
                          })
                          .OrderByDescending(l => l.StartDate) // Sắp xếp theo StartDate giảm dần (từ mới đến cũ)
                          .ToList();

            return leaves;
        }
        public void UpdateLeave(LeaveViewModel leave)
        {
            // Tìm đơn nghỉ phép hiện có
            var existingLeave = _context.Leaves.Find(leave.LeaveID);
            if (existingLeave != null)
            {
                // Lưu trạng thái cũ
                var oldStatus = existingLeave.Status;

                // Cập nhật trạng thái mới
                existingLeave.Status = leave.Status;

                // Tính số ngày nghỉ phép
                int leaveDays = (existingLeave.EndDate - existingLeave.StartDate).Days + 1 ;

                // Tìm nhân viên tương ứng
                var employee = _context.Employees.Find(existingLeave.EmployeeId);
                if (employee != null)
                {
                  
                    // Nếu chuyển từ "Approved" sang "Rejected", hoàn lại số ngày phép
                    if (oldStatus == "Approved" && leave.Status == "Rejected")
                    {
                        employee.RemainingLeaveDays += leaveDays;
                    }
                    // Nếu chuyển từ "Rejected" hoặc "Pending" sang "Approved", trừ số ngày phép
                    else if ((oldStatus == "Rejected" || oldStatus == "Pending") && leave.Status == "Approved")
                    {
                        if (leaveDays > employee.RemainingLeaveDays)
                        {
                            MessageBox.Show("Không thể duyệt đơn nghỉ phép vì số ngày nghỉ vượt quá số ngày còn lại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return; // Dừng thực thi phương thức
                        }
                        
                        employee.RemainingLeaveDays -= leaveDays;

                        // Đảm bảo số ngày phép không âm
                        if (employee.RemainingLeaveDays < 0)
                        {
                            employee.RemainingLeaveDays = 0; // Hoặc xử lý theo logic của bạn
                        }
                    }
                }

                // Lưu thay đổi vào cơ sở dữ liệu
                _context.SaveChanges();
            }
        }

        public void AddLeaveRequest(Leaves leave)
        {
            try
            {
             
                // Thêm đơn nghỉ phép vào cơ sở dữ liệu
                _context.Leaves.Add(leave);
                _context.SaveChanges();

                Console.WriteLine("Đơn nghỉ phép đã được thêm thành công.");
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                Console.WriteLine($"Lỗi khi thêm đơn nghỉ phép: {ex.Message}");
                throw; // Ném lại ngoại lệ để xử lý ở lớp gọi
            }
        }
    }
 }
