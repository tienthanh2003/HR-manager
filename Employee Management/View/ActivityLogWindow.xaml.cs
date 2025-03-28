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
    /// Interaction logic for ActivityLogWindow.xaml
    /// </summary>
    public partial class ActivityLogWindow : Window
    {
        public ActivityLogWindow()
        {
            InitializeComponent();
            this.DataContext = new ActivityLogViewModel(new ActivityLogRepository());
        }
    }
}
