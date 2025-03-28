using Employee_Management.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employee_Management.Repository
{
    public interface IEmployeeRepository
    {
        public List<Employee> getAllEmployee();

        public List<Employee> GetEmployeesByDepartment(int departmentId);

        public List<Employee> GetEmployeesByGender(string gender);

        public List<Employee> SearchEmployeesByName(string name);

        public Employee GetEmployeeById(int employeeId);

        public void UpdateEmployee(Employee employee);

        public void DeleteEmployee(int employeeId);

        public Employee GetEmployeeByUserId(int userId);
        public void AddEmployee(Employee employee);
    }
}
