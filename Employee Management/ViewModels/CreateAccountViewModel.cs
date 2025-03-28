using Data_Access.Security;
using Employee_Management.Models;
using Employee_Management.Repository;
using Employee_Management.View;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Employee_Management.ViewModels
{
    public class CreateAccountViewModel : INotifyPropertyChanged
    {
        private readonly IUserRepository _userRepository;
        private readonly Employee _employee;
        private readonly IActivityLogRepository activityLogRepository;
        public string username;

        public string Username
        {
            get => username;
            set
            {
                username = value;
                OnPropertyChanged(nameof(Username));
            }
        }
        public string password;
        public string Password
        {
            get => password;
            set
            {
                password = value;
                OnPropertyChanged(nameof(Password));
            }
        }
        public ICommand CreateAccountCommand { get; }
        public ICommand CancelCommand { get; }

        public CreateAccountViewModel(IUserRepository userRepository, Employee employee)
        {
            _userRepository = userRepository;
            _employee = employee;

            // Khởi tạo các command
            CreateAccountCommand = new RelayCommand(CreateAccount);
            CancelCommand = new RelayCommand(Cancel);
            activityLogRepository = new ActivityLogRepository();
        }

        private void CreateAccount(object parameter)
        {
            try
            {
                // Kiểm tra xem nhân viên đã có tài khoản chưa
                var existingUser = _userRepository.GetUserByEmployeeId(_employee.EmployeeId);
                if (existingUser != null)
                {
                    MessageBox.Show("Nhân viên đã có tài khoản.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // Tạo tài khoản mới
                var user = new User
                {
                    Username = Username,
                    PasswordHash = SecurityHelper.HashPassword(Password), // Băm mật khẩu
                    Role = "Employee", // Vai trò mặc định
                    EmployeeId = _employee.EmployeeId
                };

                // Lưu tài khoản vào cơ sở dữ liệu
                _userRepository.AddUser(user);

                // Hiển thị thông báo thành công
                MessageBox.Show("Tạo tài khoản thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                activityLogRepository.LogActivity(1, "Tạo tài khoản", $"Tạo tài khoản cho nhân viên : {_employee.FullName}");

                // Đóng cửa sổ
                if (parameter is Window window)
                {
                    window.DialogResult = true;
                    window.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tạo tài khoản: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel(object parameter)
        {
            if (parameter is Window window)
            {
                window.DialogResult = false; // Đặt DialogResult là false
                window.Close(); // Đón
            }
        }

        

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}