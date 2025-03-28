using Employee_Management.View;
using Employee_Management.ViewModels;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Employee_Management.ViewModel
{
    public class ReportStatisticsViewModel : INotifyPropertyChanged
    {
        // Command để mở cửa sổ báo cáo thống kê nhân viên
        public ICommand EmployeeReportCommand { get; }

        // Command để mở cửa sổ báo cáo thống kê lương
        public ICommand SalaryReportCommand { get; }

        public ReportStatisticsViewModel()
        {
            // Khởi tạo các lệnh
            EmployeeReportCommand = new RelayCommand(OpenEmployeeReport);
            SalaryReportCommand = new RelayCommand(OpenSalaryReport);
        }

        // Phương thức mở cửa sổ báo cáo thống kê nhân viên
        private void OpenEmployeeReport(object parameter)
        {
            var employ = new View.EmployeeReportView();
            employ.Show();
            CloseWindow(Application.Current.Windows.OfType<ReportStatisticsView>().FirstOrDefault());
        }

        // Phương thức mở cửa sổ báo cáo thống kê lương
        private void OpenSalaryReport(object parameter)
        {
            var salary = new View.SalaryReportWindow();
            salary.Show();
            CloseWindow(Application.Current.Windows.OfType<ReportStatisticsView>().FirstOrDefault());
        }

        // Triển khai INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void CloseWindow(Window window)
        {
            window.Close();
        }
    }
}