using Employee_Management.Models;
using Employee_Management.View;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Employee_Management.Repository
{
    public class SalaryRespository : ISalaryRepository
    {
        private readonly EmployeeManagementContext context;
        private readonly IActivityLogRepository _activityLogRepository;
        private readonly IAttendRepository attendRepository;
        public SalaryRespository()
        {
            context = new EmployeeManagementContext();
            _activityLogRepository = new ActivityLogRepository();
            attendRepository = new AttendRepository();
        }

        public List<SalaryViewModel> GetAllSalariesWithEmployeeName(int month, int year)
        {
            try
            {
                var salaries = (from s in context.Salaries
                                join e in context.Employees on s.EmployeeId equals e.EmployeeId
                                where s.StartDate.Month == month && s.StartDate.Year == year
                                where s.EmployeeId != null
                                select new SalaryViewModel
                                {
                                    SalaryId = s.SalaryId,
                                    EmployeeId = s.EmployeeId,
                                    EmployeeName = e.FullName,
                                    BaseSalary = s.BaseSalary,
                                    Allowance = s.Allowance,
                                    Bonus = s.Bonus,
                                    Deduction = s.Deduction,
                                    TotalSalary = s.TotalSalary,
                                    StartDate = s.StartDate,
                                    PaymentDate = s.PaymentDate,
                                    RemainingLeaveDays = e.RemainingLeaveDays,
                                    SalaryStatus = s.SalaryStatus,
                                    WorkDays = attendRepository.CountAttendDays(s.EmployeeId, s.StartDate, s.PaymentDate), // Truyền employeeId
                                    AbsentDays = attendRepository.CountAbsentDays(s.EmployeeId, s.StartDate, s.PaymentDate), // Truyền employeeId
                                    OvertimeHours = attendRepository.CalculateTotalOvertime(s.EmployeeId, s.StartDate, s.PaymentDate) // Truyền employeeId
                                }).ToList();

                return salaries;
            }
            catch (Exception ex)
            {
                // Xử lý lỗi (ví dụ: ghi log hoặc hiển thị thông báo)
                Console.WriteLine($"Lỗi khi truy xuất dữ liệu lương thưởng: {ex.Message}");
                return new List<SalaryViewModel>(); // Trả về danh sách rỗng nếu có lỗi
            }
        }

        public void GenerateMonthlySalary()
        {
            try
            {
                bool Isadd = false;
                // Lấy danh sách nhân viên
                var employees = context.Employees.ToList();

                // Tính toán StartDate là ngày 2 của tháng hiện tại
                DateTime startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 2);

                // Tính toán PaymentDate là ngày 1 của tháng sau
                DateTime paymentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1);

                // Duyệt qua từng nhân viên
                foreach (var employee in employees)
                {
                    // Kiểm tra xem đã có bản ghi lương cho nhân viên này trong tháng hiện tại chưa
                    var existingSalary = context.Salaries
                        .FirstOrDefault(s => s.EmployeeId == employee.EmployeeId &&
                                             s.StartDate.Month == startDate.Month &&
                                             s.StartDate.Year == startDate.Year);

                    // Nếu chưa có, tạo bản ghi lương mới
                    if (existingSalary == null)
                    {
                        // Tính toán tổng lương
                        decimal totalSalary = (employee.Salary ?? 0) + 0 + 0 - 0; // BaseSalary + Allowance + Bonus - Deduction

                        var newSalary = new Salary
                        {
                            EmployeeId = employee.EmployeeId,
                            BaseSalary = employee.Salary ?? 0, // Lấy lương cơ bản từ thông tin nhân viên
                            Allowance = 0, // Phụ cấp mặc định là 0
                            Bonus = 0, // Thưởng mặc định là 0
                            Deduction = 0, // Khấu trừ mặc định là 0
                            TotalSalary = totalSalary, // Tổng lương đã tính toán
                            StartDate = startDate, // Ngày 2 của tháng hiện tại
                            PaymentDate = paymentDate, // Ngày 1 của tháng sau
                            SalaryStatus = "Pending" // Trạng thái lương mặc định là "Pending"
                        };

                        // Thêm vào cơ sở dữ liệu
                        context.Salaries.Add(newSalary);
                        Isadd = true;
                        if(Isadd)
                        {
                            _activityLogRepository.LogActivity(1, "Tạo dữ liệu tính lương tháng mới", "Hệ thống đã tạo dữ liệu tính lương tháng mới");
                        }
                        
                    }
                    
                }

                // Lưu thay đổi vào cơ sở dữ liệu
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi tạo lương tháng mới: {ex.Message}");
            }
        }

        public void AddSalary(Salary salary)
        {
            try
            {
                if (salary == null)
                {
                    throw new ArgumentNullException(nameof(salary), "Salary cannot be null.");
                }

                // Tính toán TotalSalary
                salary.TotalSalary = salary.BaseSalary + salary.Allowance + salary.Bonus - salary.Deduction;

                context.Salaries.Add(salary);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while adding salary: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                throw;
            }
        }

        public void UpdateSalary(SalaryViewModel salary)
        {
            try
            {
                // Kiểm tra xem đối tượng salary có tồn tại trong cơ sở dữ liệu không
                var existingSalary = context.Salaries.Find(salary.SalaryId);
                if (existingSalary == null)
                {
                    throw new ArgumentException("Salary not found.");
                }

                // Log giá trị trước khi cập nhật
                MessageBox.Show($"Existing BaseSalary before update: {existingSalary.BaseSalary}");
                

                // Cập nhật thông tin lương
                existingSalary.BaseSalary = salary.BaseSalary;
                existingSalary.Allowance = salary.Allowance;
                existingSalary.Bonus = salary.Bonus;
                existingSalary.Deduction = salary.Deduction;
                existingSalary.SalaryStatus = salary.SalaryStatus;
                existingSalary.TotalSalary = salary.TotalSalary;

                // Log giá trị sau khi cập nhật
                MessageBox.Show($"New BaseSalary: {salary.BaseSalary}");

                context.Salaries.Update(existingSalary);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                // Xử lý lỗi (ví dụ: ghi log, hiển thị thông báo lỗi)
                MessageBox.Show($"Error updating salary: {ex.Message}");
                throw new Exception("An error occurred while updating the salary.", ex);
            }
        }

        public List<SalaryViewModel> GetSalariesByDate(int month, int quarter, int year)
        {
            try
            {
                var salaries = (from s in context.Salaries
                                join e in context.Employees on s.EmployeeId equals e.EmployeeId
                                where s.EmployeeId != null
                                where s.StartDate.Year == year
                                where (month == 0 || s.StartDate.Month == month)
                                where (quarter == 0 || (s.StartDate.Month >= (quarter - 1) * 3 + 1 && s.StartDate.Month <= quarter * 3))
                                orderby e.FullName ascending, s.StartDate descending // Sắp xếp theo tên nhân viên, rồi theo ngày giảm dần
                                select new SalaryViewModel
                                {
                                    SalaryId = s.SalaryId,
                                    EmployeeId = s.EmployeeId,
                                    EmployeeName = e.FullName,
                                    BaseSalary = s.BaseSalary,
                                    Allowance = s.Allowance,
                                    Bonus = s.Bonus,
                                    Deduction = s.Deduction,
                                    TotalSalary = s.TotalSalary,
                                    StartDate = s.StartDate,
                                    PaymentDate = s.PaymentDate,
                                    SalaryStatus = s.SalaryStatus
                                }).ToList();

                return salaries;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi truy xuất dữ liệu lương thưởng: {ex.Message}");
                return new List<SalaryViewModel>();
            }
        }

    }
}
