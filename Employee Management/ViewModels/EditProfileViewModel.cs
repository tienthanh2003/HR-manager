using Employee_Management.Models;
using Employee_Management.Repository;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Employee_Management.ViewModels
{
    public class EditProfileViewModel : INotifyPropertyChanged
    {
        private readonly IEmployeeRepository _employeeRepository;
        private Employee _employee;
        private readonly IActivityLogRepository _activityLogRepository;
        private readonly IUserRepository _userRepository;

        // Thuộc tính
        public string FullName
        {
            get => _employee.FullName;
            set
            {
                _employee.FullName = value;
                OnPropertyChanged(nameof(FullName));
            }
        }

        public DateTime DateOfBirth
        {
            get => _employee.DateOfBirth;
            set
            {
                _employee.DateOfBirth = value;
                OnPropertyChanged(nameof(DateOfBirth));
            }
        }

        public string Gender
        {
            get => _employee.Gender;
            set
            {
                _employee.Gender = value;
                OnPropertyChanged(nameof(Gender));
            }
        }

        public string Address
        {
            get => _employee.Address;
            set
            {
                _employee.Address = value;
                OnPropertyChanged(nameof(Address));
            }
        }

        public string PhoneNumber
        {
            get => _employee.PhoneNumber;
            set
            {
                _employee.PhoneNumber = value;
                OnPropertyChanged(nameof(PhoneNumber));
            }
        }

        // Danh sách giới tính
        public List<string> Genders { get; } = new List<string> { "Male", "Female" };

        // Lệnh
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public EditProfileViewModel(IEmployeeRepository employeeRepository, Employee employee)
        {
            _employeeRepository = employeeRepository;
            _employee = employee;
            _activityLogRepository = new ActivityLogRepository();   
            _userRepository = new UserRepository();
            SaveCommand = new RelayCommand(SaveProfile);
            CancelCommand = new RelayCommand(Cancel);
        }

        private void SaveProfile(object parameter)
        {
            try
            {
                // Cập nhật thông tin nhân viên
                _employeeRepository.UpdateEmployee(_employee);
                var user = _userRepository.GetUserByEmployeeId(_employee.EmployeeId);
                _activityLogRepository.LogActivity(user.UserId, $"Nhân viên {_employee.FullName} cập nhật thông tin ", $"Nhân viên {_employee.FullName} cập nhật thông tin");
                // Hiển thị thông báo thành công
                MessageBox.Show("Cập nhật thành công!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // Đóng cửa sổ
                if (parameter is Window window)
                {
                    window.DialogResult = true;
                    window.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating profile: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel(object parameter)
        {
            if (parameter is Window window)
            {
                window.DialogResult = false;
                window.Close();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}