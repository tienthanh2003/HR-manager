using Employee_Management.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employee_Management.Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly EmployeeManagementContext _context;
        
        public EmployeeRepository() {
            _context = new EmployeeManagementContext();
        }
        public List<Employee> getAllEmployee()
        {
            try
            {
                return _context.Employees.ToList();
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<Employee> GetEmployeesByDepartment(int departmentId)
        {
            return _context.Employees
                          .Where(e => e.DepartmentId == departmentId)
                          .ToList();
        }

        public List<Employee> GetEmployeesByGender(string gender)
        {
            return _context.Employees
                          .Where(e => e.Gender == gender )
                          .ToList();
        }

        public List<Employee> GetEmployeesBySalaryRange(decimal minSalary, decimal maxSalary)
        {
            try
            {
                return _context.Employees
                               .Where(e => e.Salary >= minSalary && e.Salary <= maxSalary)
                               .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while filtering employees by salary.", ex);
            }
        }


        public List<Employee> SearchEmployeesByName(string name)
        {
            try
            {
                // Sử dụng LINQ để tìm kiếm nhân viên có tên chứa chuỗi `name`
                return _context.Employees
                               .Where(e => e.FullName.Contains(name) || e.Position.Contains(name))
                               .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void AddEmployee(Employee employee)
        {
            try
            {
                // Kiểm tra xem nhân viên có hợp lệ không
                if (employee == null)
                {
                    throw new ArgumentNullException(nameof(employee), "Employee cannot be null.");
                }

                // Đặt StartDate là ngày hiện tại
                employee.StartDate = DateTime.Now;

                // Thêm nhân viên vào DbSet
                _context.Employees.Add(employee);
                _context.SaveChanges(); // Lưu trước để có EmployeeId

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");

                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }

                throw;
            }
        }

        public Employee GetEmployeeById(int employeeId)
        {
            try
            {
                // Truy vấn nhân viên dựa trên employeeId
                var employee = _context.Employees
                    .FirstOrDefault(e => e.EmployeeId == employeeId);

                return employee; // Trả về nhân viên nếu tìm thấy, ngược lại trả về null
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ và ghi log (nếu cần)
                throw new Exception($"Lỗi khi lấy thông tin nhân viên: {ex.Message}");
            }
        }

        public void UpdateEmployee(Employee employee)
        {
            try
            {
                // Kiểm tra xem nhân viên có hợp lệ không
                if (employee == null)
                {
                    throw new ArgumentNullException(nameof(employee), "Employee cannot be null.");
                }

                // Tìm nhân viên cần cập nhật trong cơ sở dữ liệu
                var existingEmployee = _context.Employees.Find(employee.EmployeeId);

                if (existingEmployee == null)
                {
                    throw new ArgumentException("Employee not found.", nameof(employee));
                }

                // Cập nhật thông tin của nhân viên
                existingEmployee.FullName = employee.FullName;
                existingEmployee.DateOfBirth = employee.DateOfBirth;
                existingEmployee.Gender = employee.Gender;
                existingEmployee.Address = employee.Address;
                existingEmployee.PhoneNumber = employee.PhoneNumber;
                existingEmployee.DepartmentId = employee.DepartmentId;
                existingEmployee.Position = employee.Position;
                existingEmployee.Salary = employee.Salary;
                existingEmployee.StartDate = employee.StartDate;

                // Chỉ cập nhật Avatar nếu giá trị được cung cấp
                if (employee.Avatar != null && employee.Avatar.Length > 0)
                {
                    existingEmployee.Avatar = employee.Avatar;
                }

                // Lưu thay đổi vào cơ sở dữ liệu
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và ném ngoại lệ
                throw new Exception("An error occurred while updating the employee.", ex);
            }
        }


        public void DeleteEmployee(int employeeId)
        {
            try
            {
                // Tìm nhân viên cần xóa dựa trên employeeId
                var employee = _context.Employees.Find(employeeId);

                if (employee == null)
                {
                    throw new ArgumentException("Nhân viên không tồn tại.", nameof(employeeId));
                }

                // Xóa nhân viên khỏi DbSet
                _context.Employees.Remove(employee);

                // Lưu thay đổi vào cơ sở dữ liệu
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và ném ngoại lệ
                throw new Exception("Lỗi khi xóa nhân viên.", ex);
            }
        }

        public Employee GetEmployeeByUserId(int userId)
        {
            using (var context = new EmployeeManagementContext())
            {
                // Truy vấn để lấy Employee dựa trên UserId
                var employee = (from u in context.Users
                                join e in context.Employees on u.EmployeeId equals e.EmployeeId
                                where u.UserId == userId
                                select e).FirstOrDefault();

                return employee;
            }
        }
    }
}
