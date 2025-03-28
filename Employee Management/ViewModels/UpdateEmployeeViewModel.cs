using Employee_Management.Models;
using Employee_Management.Repository;
using Employee_Management.View;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Employee_Management.ViewModels
{
    public class UpdateEmployeeViewModel : INotifyPropertyChanged
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IActivityLogRepository _activityLogRepository;
        private Employee _employee;
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
        public ICommand UpdateEmployeeCommand { get; }
        public ICommand CancelCommand { get; }

        public UpdateEmployeeViewModel(IDepartmentRepository departmentRepository, IEmployeeRepository employeeRepository, int employeeId)
        {
            _departmentRepository = departmentRepository;
            _employeeRepository = employeeRepository;
            _activityLogRepository = new ActivityLogRepository();
            // Khởi tạo các lệnh
            UploadAvatarCommand = new RelayCommand(UploadAvatar);
            UpdateEmployeeCommand = new RelayCommand(UpdateEmployee, CanUpdateEmployee);
            CancelCommand = new RelayCommand(Cancel);
            LoadDepartments();
            // Tải dữ liệu nhân viên và phòng ban
            LoadEmployee(employeeId);
           
        }

        private void LoadEmployee(int employeeId)
        {
            try
            {
                // Lấy thông tin nhân viên từ repository
                _employee = _employeeRepository.GetEmployeeById(employeeId);

                if (_employee != null)
                {
                    // Gán giá trị từ nhân viên vào các thuộc tính
                    FullName = _employee.FullName;
                    DateOfBirth = _employee.DateOfBirth;
                    Gender = _employee.Gender;
                    Address = _employee.Address;
                    PhoneNumber = _employee.PhoneNumber;
                    Position = _employee.Position;
                    Salary = _employee.Salary ?? 0; // Đảm bảo Salary không bị null
                    SelectedDepartment = Departments?.FirstOrDefault(d => d.DepartmentId == _employee.DepartmentId);
                    Avatar = ConvertByteArrayToBitmapImage(_employee.Avatar); // Chuyển đổi byte[] thành BitmapImage
                    _activityLogRepository.LogActivity(1, "Sửa thông tin nhân viên", $"Sửa thông tin nhân viên {_employee.FullName}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải thông tin nhân viên: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

        private void UpdateEmployee(object parameter)
        {
            try
            {
                // Chuyển đổi ảnh đại diện thành mảng byte
                byte[] avatarBytes = ConvertBitmapImageToByteArray(Avatar);

                // Cập nhật thông tin nhân viên
                _employee.FullName = FullName;
                _employee.DateOfBirth = DateOfBirth.Value;
                _employee.Gender = Gender;
                _employee.Address = Address;
                _employee.PhoneNumber = PhoneNumber;
                _employee.Position = Position;
                _employee.Salary = Salary;
                _employee.DepartmentId = SelectedDepartment.DepartmentId;
                _employee.Avatar = avatarBytes;

                // Gọi repository để cập nhật nhân viên
                _employeeRepository.UpdateEmployee(_employee);

                // Hiển thị thông báo thành công
                MessageBox.Show("Employee updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                // Đóng cửa sổ
                if (parameter is Window window)
                {
                    window.DialogResult = true;
                   
                    window.Close();
                 ;
                }
            }
            catch (Exception ex)
            {
                // Hiển thị thông báo lỗi
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CanUpdateEmployee(object parameter)
        {
            // Kiểm tra các điều kiện để nút "Update" được kích hoạt
            return !string.IsNullOrWhiteSpace(FullName) &&
                   DateOfBirth.HasValue &&
                   !string.IsNullOrWhiteSpace(Gender) &&
                   !string.IsNullOrWhiteSpace(Address) &&
                   !string.IsNullOrWhiteSpace(PhoneNumber) &&
                   !string.IsNullOrWhiteSpace(Position) &&
                   Salary > 0 &&
                   SelectedDepartment != null;
        }

        private void Cancel(object parameter)
        {
            if (parameter is Window window)
            {
                window.DialogResult = false;
                window.Close();
            }
        }

        private BitmapImage ConvertByteArrayToBitmapImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0)
                return null;

            var bitmapImage = new BitmapImage();
            using (var ms = new MemoryStream(imageData))
            {
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = ms;
                bitmapImage.EndInit();
            }
            return bitmapImage;
        }

        private byte[] ConvertBitmapImageToByteArray(BitmapImage bitmapImage)
        {
            if (bitmapImage == null)
                return null;

            byte[] data;
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapImage));

            using (var ms = new MemoryStream())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }

            return data;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}