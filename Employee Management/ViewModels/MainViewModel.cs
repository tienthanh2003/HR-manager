using System.Windows;
using System.Windows.Input;
using Employee_Management.View;
using Employee_Management.ViewModels;
using Microsoft.Win32; // Để sử dụng SaveFileDialog và OpenFileDialog
using System.IO; // Để làm việc với file
using Newtonsoft.Json;
using Employee_Management.Models;
using Employee_Management.Repository; // Để xử lý JSON
using System.Collections.ObjectModel;

namespace Employee_Management.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly EmployeeManagementContext _context;

       
       
        private readonly IActivityLogRepository _activityLogRepository;
        // Các lệnh tương ứng với các button
        private readonly INotificationRepository _notificationRepository;

        // Danh sách thông báo
        private ObservableCollection<Notification> _notifications;
        public ObservableCollection<Notification> Notifications
        {
            get => _notifications;
            set
            {
                _notifications = value;
                OnPropertyChanged(nameof(Notifications));
            }
        }
        public ICommand ManageEmployeesCommand { get; }
        public ICommand ManageDepartmentsCommand { get; }
        public ICommand ManageSalariesCommand { get; }
        public ICommand ManageAttendanceCommand { get; }
        public ICommand ReportsCommand { get; }
        public ICommand SendNotificationsCommand { get; }
        public ICommand BackupCommand { get; }
        public ICommand RestoreCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand ManageLeaveCommand { get; }

        public ICommand ViewActivityLogCommand { get; }

        public MainViewModel()
        {
            _activityLogRepository = new ActivityLogRepository();
            _notificationRepository = new NotificationRepository();
            // Khởi tạo các lệnh
            ManageEmployeesCommand = new RelayCommand(ExecuteManageEmployees);
            ManageDepartmentsCommand = new RelayCommand(ExecuteManageDepartments);
            ManageSalariesCommand = new RelayCommand(ExecuteManageSalaries);
            ManageAttendanceCommand = new RelayCommand(ExecuteManageAttendance);
            ReportsCommand = new RelayCommand(ExecuteReports);
            SendNotificationsCommand = new RelayCommand(ExecuteSendNotifications);
            BackupCommand = new RelayCommand(ExecuteBackup);
            RestoreCommand = new RelayCommand(ExecuteRestore); // Khởi tạo lệnh phục hồi
            LogoutCommand = new RelayCommand(ExecuteLogout);
            LogoutCommand = new RelayCommand(ExecuteLogout);
            ManageLeaveCommand = new RelayCommand(ExecuteManageLeave);
            ViewActivityLogCommand = new RelayCommand(OpenActivityLogWindow);
            _context = new EmployeeManagementContext();
            LoadNotifications();
        }
        private void LoadNotifications()
        {
            try
            {
                // Lấy tất cả thông báo từ repository
                var notifications = _notificationRepository.getAll(); // Thay 1 bằng EmployeeId thực tế
                Notifications = new ObservableCollection<Notification>(notifications);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải thông báo: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void ExecuteManageEmployees(object parameter)
        {
            // Xử lý khi nhấn nút "Quản lý nhân viên"
            var employeeManagement = new View.EmployeeManagement();
            employeeManagement.Show();
            CloseWindow(Application.Current.Windows.OfType<MainWindow>().FirstOrDefault());
        }

        private void ExecuteManageDepartments(object parameter)
        {
            // Xử lý khi nhấn nút "Quản lý phòng ban"
            var dpManagementWindow = new View.DPManagement();
            dpManagementWindow.Show();
            CloseWindow(Application.Current.Windows.OfType<MainWindow>().FirstOrDefault());
        }

        private void ExecuteManageSalaries(object parameter)
        {
            // Xử lý khi nhấn nút "Quản lý lương thưởng"
            var salary = new View.SalaryManagementWindow();
            salary.Show();
            CloseWindow(Application.Current.Windows.OfType<MainWindow>().FirstOrDefault());
        }

        private void ExecuteManageAttendance(object parameter)
        {
            // Xử lý khi nhấn nút "Quản lý chấm công"
            var attend = new View.AttendanceManagementWindow();
            attend.Show();
            CloseWindow(Application.Current.Windows.OfType<MainWindow>().FirstOrDefault());
        }

        private void ExecuteManageLeave(object parameter)
        {
            // Xử lý khi nhấn nút "Quản lý nghỉ phép"
            var leave = new View.LeaveManagement();
            leave.Show();
            CloseWindow(Application.Current.Windows.OfType<MainWindow>().FirstOrDefault());
        }

        private void ExecuteReports(object parameter)
        {
            // Xử lý khi nhấn nút "Báo cáo và Thống kê"
            MessageBox.Show("Báo cáo và Thống kê");
            var report = new View.ReportStatisticsView();
            report.Show();
            CloseWindow(Application.Current.Windows.OfType<MainWindow>().FirstOrDefault());
        }

        private void ExecuteSendNotifications(object parameter)
        {
            // Xử lý khi nhấn nút "Gửi thông báo"
            var notification = new View.SendNotificationWindow();
            notification.Show();
            CloseWindow(Application.Current.Windows.OfType<MainWindow>().FirstOrDefault());
        }

        private void ExecuteBackup(object parameter)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                DefaultExt = ".json",
                FileName = "employees_backup.json"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;
                BackupEmployeeData(filePath);
            }
        }

        private void BackupEmployeeData(string filePath)
        {
            try
            {
                var employees = _context.Employees.ToList();
                string jsonData = JsonConvert.SerializeObject(employees, Formatting.Indented);
                File.WriteAllText(filePath, jsonData);
                MessageBox.Show("Sao lưu dữ liệu thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                _activityLogRepository.LogActivity(1, "Sao lưu dữ liệu", $"Admin sao lưu dữ liệu nhân viên ngày{DateTime.Today}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi sao lưu dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteRestore(object parameter)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                DefaultExt = ".json"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                RestoreEmployeeData(filePath);
            }
        }

        private void RestoreEmployeeData(string filePath)
        {
            try
            {
                // Đọc dữ liệu từ file JSON
                string jsonData = File.ReadAllText(filePath);
                var employees = JsonConvert.DeserializeObject<List<Employee>>(jsonData);

                // Xóa dữ liệu cũ trong bảng Employees
                _context.Employees.RemoveRange(_context.Employees);
                _context.SaveChanges();

                // Đặt EmployeeId = 0 để Entity Framework tự động tạo giá trị mới
                foreach (var employee in employees)
                {
                    employee.EmployeeId = 0;
                }

                // Thêm dữ liệu mới vào cơ sở dữ liệu
                _context.Employees.AddRange(employees);

                _context.SaveChanges();

                MessageBox.Show("Phục hồi dữ liệu thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                _activityLogRepository.LogActivity(1, "Phục hồi dữ liệu", $"Admin phục hồi dữ liệu nhân viên ngày{DateTime.Today}");
            }
            catch (Exception ex)
            {
                // Hiển thị thông báo lỗi chi tiết
                MessageBox.Show($"Lỗi khi phục hồi dữ liệu: {ex.InnerException?.Message ?? ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenActivityLogWindow(object parameter)
        {
            var activityLogWindow = new ActivityLogWindow();
            activityLogWindow.Show();
        }

        private void ExecuteLogout(object parameter)
        {
            // Hiển thị hộp thoại xác nhận đăng xuất
            MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận đăng xuất", MessageBoxButton.OKCancel, MessageBoxImage.Question);

            // Nếu người dùng chọn OK
            if (result == MessageBoxResult.OK)
            {
                // Mở lại cửa sổ đăng nhập
                var loginWindow = new Login();
                loginWindow.Show();
                CloseWindow(Application.Current.Windows.OfType<MainWindow>().FirstOrDefault());
            }
        }

        private void CloseWindow(Window window)
        {
            window.Close();
        }
    }
}