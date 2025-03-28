using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Employee_Management.Models;

namespace Employee_Management.Repository
{
    public interface IUserRepository
    {
        
        public User Login(string username, string password);

        public User GetUserByEmployeeId(int employeeId);

        public void AddUser(User user);

        public User GetUserById(int Id);

        public List<User> GetAll();
    }
}
