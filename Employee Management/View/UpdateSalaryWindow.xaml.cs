using Employee_Management.Models;
using Employee_Management.Repository;
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

namespace Employee_Management.View
{
    /// <summary>
    /// Interaction logic for UpdateSalaryWindow.xaml
    /// </summary>
    public partial class UpdateSalaryWindow : Window
    {
        public SalaryViewModel Salaries { get; set; }
        public UpdateSalaryWindow(SalaryViewModel salary)
        {
            InitializeComponent();
            Salaries = salary;
            this.DataContext = new UpdateSalaryViewModel(salary,new SalaryRespository());
        }
    }
}
