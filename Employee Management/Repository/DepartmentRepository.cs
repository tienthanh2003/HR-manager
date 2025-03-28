using Employee_Management.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employee_Management.Repository
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly EmployeeManagementContext _context;
        public DepartmentRepository() {
            _context = new EmployeeManagementContext();
        }
        public List<Department> getAllDepartment()
        {
            try
            {
                return _context.Departments.ToList();
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<Department> getDepartmentActive()
        {
            try
            {
                return _context.Departments.Where(d => d.Status == "Active").ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public List<Department> searchByName(string name)
        {
            try
            {
                // Kiểm tra nếu tên tìm kiếm trống hoặc null
                if (string.IsNullOrWhiteSpace(name))
                {
                    // Trả về toàn bộ danh sách phòng ban nếu không có từ khóa tìm kiếm
                    return getAllDepartment();
                }

                // Chuẩn hóa từ khóa tìm kiếm và tìm kiếm không phân biệt chữ hoa/chữ thường
                string searchTerm = name.Trim().ToLower(); // Loại bỏ khoảng trắng thừa và chuyển về chữ thường
                return _context.Departments
                    .Where(d => d.DepartmentName.ToLower().Contains(searchTerm))
                    .ToList();
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và ném ngoại lệ
                throw new Exception($"Lỗi khi tìm kiếm phòng ban: {ex.Message}", ex);
            }
        }

        public Department GetDepartmentById(int departmentId)
        {
            try
            {
                var department = _context.Departments
                    .FirstOrDefault(d => d.DepartmentId == departmentId);

                if (department == null)
                {
                    throw new KeyNotFoundException($"Không tìm thấy phòng ban với ID: {departmentId}");
                }

                return department;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thông tin phòng ban: {ex.Message}", ex);
            }
        }

        public void Add(Department department)
        {
            try
            {
                // Kiểm tra xem phòng ban đã tồn tại chưa
                var existingDepartment = _context.Departments
                    .FirstOrDefault(d => d.DepartmentName.ToLower() == department.DepartmentName.ToLower());

                if (existingDepartment != null)
                {
                    throw new InvalidOperationException("Phòng ban đã tồn tại.");
                }

                // Thêm phòng ban mới vào cơ sở dữ liệu
                _context.Departments.Add(department);
                _context.SaveChanges();

                Console.WriteLine("Phòng ban đã được thêm thành công.");
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                Console.WriteLine($"Lỗi khi thêm phòng ban: {ex.Message}");
                throw; // Ném lại ngoại lệ để xử lý ở lớp gọi
            }
        }

        public void Delete(int departmentId)
        {
            try
            {
                // Tìm phòng ban cần cập nhật trạng thái
                var departmentToUpdate = _context.Departments.FirstOrDefault(d => d.DepartmentId == departmentId);
                if (departmentToUpdate != null)
                {
                    // Cập nhật trạng thái thành "Inactive"
                    departmentToUpdate.Status = "Inactive";

                    // Lưu thay đổi vào cơ sở dữ liệu
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật trạng thái phòng ban: {ex.Message}");
            }
        }

        public void Update(Department department)
        {
            try
            {
                var departmentToUpdate = _context.Departments.FirstOrDefault(d => d.DepartmentId == department.DepartmentId);
                if (departmentToUpdate != null)
                {
                    departmentToUpdate.DepartmentName = department.DepartmentName;
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật phòng ban: {ex.Message}");
            }
        }

    }
}
 

