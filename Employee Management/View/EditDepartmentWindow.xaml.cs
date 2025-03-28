using Employee_Management.Models;
using Employee_Management.Repository;
using Employee_Management.ViewModels;
using System.Windows;

namespace Employee_Management.View
{
    public partial class EditDepartmentWindow : Window
    {
        public EditDepartmentWindow(Department department, IDepartmentRepository departmentRepository)
        {
            InitializeComponent();
            this.DataContext = new EditDepartmentViewModel(department);
        }
    }
}
