using Employee_Management.Repository;
using Employee_Management.ViewModel;
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
    /// Interaction logic for SalaryManagementWindow.xaml
    /// </summary>
    public partial class SalaryManagementWindow : Window
    {
        public SalaryManagementWindow()
        {
            InitializeComponent();
            this.DataContext = new SalaryManagementViewModel();
        }
    }
}
