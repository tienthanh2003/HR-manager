using Employee_Management.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employee_Management.Repository
{
    public interface IDepartmentRepository
    {
        public List<Department> getAllDepartment();

        public List<Department> getDepartmentActive();
        public List<Department> searchByName(string name);

        public void Add(Department department);

        public void Delete(int departmentId);

        public void Update(Department department);

        public Department GetDepartmentById(int departmentId);
    }
}
