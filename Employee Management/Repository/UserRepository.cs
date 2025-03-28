using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data_Access.Security;
using Employee_Management.Models;

namespace Employee_Management.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly EmployeeManagementContext _context;
        public UserRepository() {
            _context = new EmployeeManagementContext();
        }
        public User Login(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if(user == null || user.PasswordHash != SecurityHelper.HashPassword(password) ) {
                return null;
            }
            return user;
        }

        public User GetUserByEmployeeId(int employeeId)
        {
            // Tìm người dùng có EmployeeID tương ứng
            return _context.Users.FirstOrDefault(u => u.EmployeeId == employeeId);
        }

        public User GetUserById(int Id)
        {
            // Tìm người dùng có EmployeeID tương ứng
            return _context.Users.FirstOrDefault(u => u.UserId == Id);
        }

        public List<User> GetAll()
        {
            // Tìm người dùng có EmployeeID tương ứng
            return _context.Users.ToList();
        }
        public void AddUser(User user)
        {
            // Kiểm tra xem người dùng đã tồn tại chưa
            var existingUser = _context.Users.FirstOrDefault(u => u.Username == user.Username || u.EmployeeId == user.EmployeeId);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Người dùng đã tồn tại.");
            }

            // Thêm người dùng mới vào cơ sở dữ liệu
            _context.Users.Add(user);
            _context.SaveChanges();
        }
    }
}
