using Employee_Management.Models;
using Employee_Management.Repository;
using Employee_Management.View;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Employee_Management.ViewModels
{
    public class AddEmployeeViewModel : INotifyPropertyChanged
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ISalaryRepository salaryRepository;
        private readonly IActivityLogRepository _activityLogRepository;
        private string _fullName;
        private DateTime? _dateOfBirth;
        private string _gender;
        private string _address;
        private string _phoneNumber;
        private string _position;
        private decimal _salary;
        private Department _selectedDepartment;
        private BitmapImage _avatar;
        private ObservableCollection<Department> _departments;

        public ObservableCollection<Department> Departments
        {
            get { return _departments; }
            set
            {
                _departments = value;
                OnPropertyChanged(nameof(Departments));
            }
        }

        public string FullName
        {
            get => _fullName;
            set
            {
                _fullName = value;
                OnPropertyChanged(nameof(FullName));
            }
        }

        public DateTime? DateOfBirth
        {
            get => _dateOfBirth;
            set
            {
                _dateOfBirth = value;
                OnPropertyChanged(nameof(DateOfBirth));
            }
        }

        public string Gender
        {
            get => _gender;
            set
            {
                _gender = value;
                OnPropertyChanged(nameof(Gender));
            }
        }

        public string Address
        {
            get => _address;
            set
            {
                _address = value;
                OnPropertyChanged(nameof(Address));
            }
        }

        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                _phoneNumber = value;
                OnPropertyChanged(nameof(PhoneNumber));
            }
        }

        public string Position
        {
            get => _position;
            set
            {
                _position = value;
                OnPropertyChanged(nameof(Position));
            }
        }

        public decimal Salary
        {
            get => _salary;
            set
            {
                _salary = value;
                OnPropertyChanged(nameof(Salary));
            }
        }

        public Department SelectedDepartment
        {
            get => _selectedDepartment;
            set
            {
                _selectedDepartment = value;
                OnPropertyChanged(nameof(SelectedDepartment));
            }
        }

        public BitmapImage Avatar
        {
            get => _avatar;
            set
            {
                _avatar = value;
                OnPropertyChanged(nameof(Avatar));
            }
        }

        public List<string> Genders { get; } = new List<string> { "Male", "Female" }; // Danh sách giới tính

        public ICommand UploadAvatarCommand { get; }
        public ICommand AddEmployeeCommand { get; }
        public ICommand CancelCommand { get; }

        public AddEmployeeViewModel(IDepartmentRepository departmentRepository, IEmployeeRepository employeeRepository)
        {
            _departmentRepository = departmentRepository;
            _employeeRepository = employeeRepository;
            salaryRepository = new SalaryRespository();
            UploadAvatarCommand = new RelayCommand(UploadAvatar);
            AddEmployeeCommand = new RelayCommand(AddEmployee, CanAddEmployee);
            CancelCommand = new RelayCommand(Cancel);
            _activityLogRepository = new ActivityLogRepository();
            LoadDepartments();
        }

        private void LoadDepartments()
        {
            try
            {
                var departments = _departmentRepository.getDepartmentActive();

                // Cập nhật ObservableCollection
                Departments = new ObservableCollection<Department>(departments);

            }
            catch (Exception ex)
            {
                // Hiển thị thông báo lỗi cho người dùng
                MessageBox.Show($"Lỗi khi tải danh sách phòng ban: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UploadAvatar(object parameter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                Avatar = new BitmapImage(new Uri(openFileDialog.FileName));
            }
        }

        private void AddEmployee(object parameter)
        {
            try
            {
                if (!CanAddEmployee(parameter))
                {
                    MessageBox.Show("Please enter valid employee information.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                byte[] avatarBytes = null;
                if (Avatar != null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(Avatar));
                        encoder.Save(ms);
                        avatarBytes = ms.ToArray();
                    }
                }

                var employee = new Employee
                {
                    FullName = FullName,
                    DateOfBirth = DateOfBirth.Value,
                    Gender = Gender,
                    Address = Address,
                    PhoneNumber = PhoneNumber,
                    Position = Position,
                    Salary = Salary,
                    DepartmentId = SelectedDepartment.DepartmentId,
                    StartDate = DateTime.Now,
                    Avatar = avatarBytes,
                    RemainingLeaveDays = 5
                };

                _employeeRepository.AddEmployee(employee);
                 

                DateTime nextMonthFirstDay = new DateTime(employee.StartDate.Year, employee.StartDate.Month, 1).AddMonths(1);

                var salary = new Salary
                {
                    EmployeeId = employee.EmployeeId,
                    BaseSalary = employee.Salary,
                    Allowance = 0,
                    Bonus = 0,
                    Deduction = 0,
                    StartDate = employee.StartDate,
                    PaymentDate = nextMonthFirstDay
                };

                salaryRepository.AddSalary(salary);

                MessageBox.Show("Employee added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                _activityLogRepository.LogActivity(1, "Thêm nhân viên", $"Thêm nhân viên: {employee.FullName}");

                if (parameter is Window window)
                {
                    var employeemanager = new EmployeeManagement();
                    employeemanager.Show();
                    window.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}\nInner Exception: {ex.InnerException?.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanAddEmployee(object parameter)
        {
            // Kiểm tra số điện thoại hợp lệ (chỉ chứa số và ít nhất 10 chữ số)
            bool isPhoneNumberValid = !string.IsNullOrWhiteSpace(PhoneNumber) &&
                                      PhoneNumber.Length >= 10 &&
                                      PhoneNumber.All(char.IsDigit);

            // Kiểm tra tuổi (phải đủ 18 tuổi)
            bool isAgeValid = DateOfBirth.HasValue &&
                              (DateTime.Today.Year - DateOfBirth.Value.Year > 18 ||
                              (DateTime.Today.Year - DateOfBirth.Value.Year == 18 &&
                               DateTime.Today >= DateOfBirth.Value.AddYears(18)));

            // Kiểm tra các điều kiện khác
            return !string.IsNullOrWhiteSpace(FullName) &&
                   isAgeValid &&
                   !string.IsNullOrWhiteSpace(Gender) &&
                   !string.IsNullOrWhiteSpace(Address) &&
                   isPhoneNumberValid &&
                   !string.IsNullOrWhiteSpace(Position) &&
                   Salary > 0 &&
                   SelectedDepartment != null;
        }



        private void Cancel(object parameter)
        {
           
            if (parameter is Window window)
            {
                var employeemanager = new EmployeeManagement();
                employeemanager.Show();
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