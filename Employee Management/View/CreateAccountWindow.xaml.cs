using Employee_Management.Models;
using Employee_Management.Repository;
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

namespace Employee_Management.View
{
    /// <summary>
    /// Interaction logic for CreateAccountWindow.xaml
    /// </summary>
    public partial class CreateAccountWindow : Window
    {
        public Employee Employee { get; set; }
        public CreateAccountWindow(Employee employee)
        {
            InitializeComponent();
            Employee = employee;
            this.DataContext = new CreateAccountViewModel(new UserRepository(),employee);
        }

        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is CreateAccountViewModel viewModel)
            {
                // Cập nhật giá trị Password trong ViewModel
                viewModel.Password = txtPassword.Password;
            }
        }
    }
}
