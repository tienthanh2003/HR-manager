using Employee_Management.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employee_Management.Repository
{
    public interface ISalaryRepository
    {
        public List<SalaryViewModel> GetAllSalariesWithEmployeeName(int month, int year);
        public void GenerateMonthlySalary();
        public void AddSalary(Salary salary);

        public List<SalaryViewModel> GetSalariesByDate(int month, int quarter, int year);
        public void UpdateSalary(SalaryViewModel salary);
    }
}
