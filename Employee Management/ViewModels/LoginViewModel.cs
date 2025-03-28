using Employee_Management.Repository;
using Employee_Management.ViewModels;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Employee_Management.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private string _userName;
        private string _password;
        private readonly IUserRepository _authService;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IAttendRepository _attendRepository;
        private bool _isAttendanceGenerated = false;
        private readonly IActivityLogRepository _activityLogRepository;

        public LoginViewModel()
        {
            _authService = new UserRepository(); // Khởi tạo dịch vụ xác thực
            LoginCommand = new RelayCommand(ExecuteLogin, CanExecuteLogin);
            _attendRepository = new AttendRepository();
            _employeeRepository = new EmployeeRepository();
            _activityLogRepository = new ActivityLogRepository();
        }

        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoginCommand { get; }

        private bool CanExecuteLogin(object parameter)
        {
            // Chỉ cho phép đăng nhập nếu cả UserName và Password không trống
            return !string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password);
        }

        private void ExecuteLogin(object parameter)
        {
            try
            {
                var user = _authService.Login(UserName, Password);
                if (user != null)
                {
                    // Đăng nhập thành công
                    ShowSuccessMessage("Đăng nhập thành công!");
                    var employee = _employeeRepository.GetEmployeeByUserId(user.UserId);
                   
                    _attendRepository.GenerateDailyAttendance();
                   
                    // Mở cửa sổ tương ứng dựa trên vai trò của người dùng
                    if (user.Role == "Admin")
                    {
                        OpenWindow(new View.MainWindow());
                        _activityLogRepository.LogActivity(user.UserId, "Đăng nhập", "Đăng nhập thành công vào hệ thống.");
                    }
                    else
                    {
                        var employeeDashboard = new View.EmployeeDashboard(employee);
                        
                        employeeDashboard.Show();
                        _activityLogRepository.LogActivity(user.UserId, "Đăng nhập", $"Nhân viên {employee.FullName}  đăng nhập thành công vào hệ thống.");
                    }

                    // Đóng cửa sổ đăng nhập
                    CloseLoginWindow();
                }
                else
                {
                    // Đăng nhập thất bại
                    ShowErrorMessage("Sai tên đăng nhập hoặc mật khẩu!");

                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                ShowErrorMessage($"Đã xảy ra lỗi: {ex.Message}");
            }
        }

        private void ShowSuccessMessage(string message)
        {
            MessageBox.Show(message, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void OpenWindow(Window window)
        {
            window.Show();
        }

        private void CloseLoginWindow()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window is Login loginWindow)
                {
                    loginWindow.Close();
                    break;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}