
using Employee_Management.Models;
using Employee_Management.Repository;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Employee_Management.ViewModels
{
    public class EditDepartmentViewModel : INotifyPropertyChanged
    {
        private Department _updatedDepartment;
        private readonly IActivityLogRepository activityLogRepository;
        public Department UpdatedDepartment
        {
            get => _updatedDepartment;
            set
            {
                _updatedDepartment = value;
                OnPropertyChanged(nameof(UpdatedDepartment));
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public EditDepartmentViewModel(Department department)
        {
            UpdatedDepartment = new Department
            {
                DepartmentId = department.DepartmentId,
                DepartmentName = department.DepartmentName
            };
            activityLogRepository = new ActivityLogRepository();

            // Khởi tạo các lệnh
            SaveCommand = new RelayCommand(Save);
            CancelCommand = new RelayCommand(Cancel);
        }

        private void Save(object parameter)
        {
            // Kiểm tra tên phòng ban không được trống
            if (string.IsNullOrWhiteSpace(UpdatedDepartment.DepartmentName))
            {
                MessageBox.Show("Vui lòng nhập tên phòng ban!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            activityLogRepository.LogActivity(1, "Sửa thông tin phòng ban", $"Sửa thông tin phòng ban: {UpdatedDepartment.DepartmentName}");
            // Đóng cửa sổ và trả về kết quả DialogResult = true
            if (parameter is Window window)
            {
                window.DialogResult = true;
                window.Close();
            }
        }

        private void Cancel(object parameter)
        {
            // Đóng cửa sổ và trả về kết quả DialogResult = false
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