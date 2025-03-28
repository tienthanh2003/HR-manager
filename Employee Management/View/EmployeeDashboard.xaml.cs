using Employee_Management.Models;
using Employee_Management.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Employee_Management.Repository;
namespace Employee_Management.View
{
    /// <summary>
    /// Interaction logic for EmployeeDashboard.xaml
    /// </summary>
    public partial class EmployeeDashboard : Window
    {
        public Employee Employee { get; set; }
        public EmployeeDashboard(Employee employees)
        {
            InitializeComponent();
            Employee = employees;
            DataContext = new EmployeeDashboardViewModel( employees,new EmployeeRepository(),new DepartmentRepository());
        }
    }
}
