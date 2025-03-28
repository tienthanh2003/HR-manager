using System.Windows.Input;

using Employee_Management.Models;
using System.Windows;
using System.ComponentModel;
using Employee_Management.Repository;

namespace Employee_Management.ViewModels
{
    public class AddDepartmentViewModel : INotifyPropertyChanged
    {
        private string _departmentName;
        private readonly IActivityLogRepository _activityLogRepository;
        public string DepartmentName
        {
            get => _departmentName;
            set
            {
                _departmentName = value;
                OnPropertyChanged(nameof(DepartmentName));
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public AddDepartmentViewModel()
        {
            _activityLogRepository = new ActivityLogRepository();
            SaveCommand = new RelayCommand(SaveDepartment, CanSaveDepartment);
            CancelCommand = new RelayCommand(Cancel);
        }

        private void SaveDepartment(object parameter)
        {
            // Kiểm tra tên phòng ban không được trống
            if (string.IsNullOrWhiteSpace(DepartmentName))
            {
                MessageBox.Show("Vui lòng nhập tên phòng ban!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _activityLogRepository.LogActivity(-1, "Thêm phòng ban", $"Thêm phòng ban: {DepartmentName}");

            // Đóng cửa sổ và trả về kết quả DialogResult = true
            if (parameter is Window window)
            {
                window.DialogResult = true;
                window.Close();
            }
        }

        private bool CanSaveDepartment(object parameter)
        {
            // Chỉ cho phép lưu khi tên phòng ban không trống
            return !string.IsNullOrWhiteSpace(DepartmentName);
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