using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Employee_Management.Models;
using Employee_Management.Repository;
using Employee_Management.ViewModels;

namespace Employee_Management.ViewModel
{
    public class LeaveRequestViewModel : INotifyPropertyChanged
    {
        private readonly LeaveRepository _leaveRepository;
        private readonly IActivityLogRepository _activityLogRepository;
        private readonly IUserRepository userRepository;
        private readonly IEmployeeRepository employeeRepository;
        // Danh sách các loại nghỉ phép
        public ObservableCollection<string> LeaveTypes { get; set; }

        private int _employeeId;
        public int EmployeeID
        {
            get => _employeeId;
            set
            {
                if (_employeeId != value)
                {
                    _employeeId = value;
                    OnPropertyChanged(nameof(EmployeeID));
                }
            }
        }

        // Thuộc tính để lưu loại nghỉ phép được chọn
        private string _selectedLeaveType;
        public string SelectedLeaveType
        {
            get => _selectedLeaveType;
            set
            {
                if (_selectedLeaveType != value)
                {
                    _selectedLeaveType = value;
                    OnPropertyChanged(nameof(SelectedLeaveType));
                }
            }
        }

        // Thuộc tính để lưu ngày bắt đầu
        private DateTime? _startDate;
        public DateTime? StartDate
        {
            get => _startDate;
            set
            {
                if (_startDate != value)
                {
                    _startDate = value;
                    OnPropertyChanged(nameof(StartDate));
                }
            }
        }

        // Thuộc tính để lưu ngày kết thúc
        private DateTime? _endDate;
        public DateTime? EndDate
        {
            get => _endDate;
            set
            {
                if (_endDate != value)
                {
                    _endDate = value;
                    OnPropertyChanged(nameof(EndDate));
                }
            }
        }

        // Lệnh để gửi đơn nghỉ phép
        public ICommand SubmitLeaveRequestCommand { get; }

        // Lệnh để hủy và đóng cửa sổ
        public ICommand CancelCommand { get; }

        public LeaveRequestViewModel(int employeeId)
        {
            // Khởi tạo repository
            _leaveRepository = new LeaveRepository();
            _employeeId = employeeId; // Gán EmployeeID
            _activityLogRepository = new ActivityLogRepository();
            employeeRepository =new EmployeeRepository();
            userRepository = new UserRepository();
            // Khởi tạo danh sách các loại nghỉ phép
            LeaveTypes = new ObservableCollection<string>
            {
                "Vacation",
                "Sick Leave",
                "Leave without pay"
            };

            // Khởi tạo lệnh
            SubmitLeaveRequestCommand = new RelayCommand(SubmitLeaveRequest);
            CancelCommand = new RelayCommand(Cancel);
        }

        private void SubmitLeaveRequest(object parameter)
        {
            // Kiểm tra dữ liệu hợp lệ
            if (string.IsNullOrEmpty(SelectedLeaveType) || StartDate == null || EndDate == null)
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Tính số ngày nghỉ phép
            int leaveDays = (EndDate.Value - StartDate.Value).Days + 1;

            // Lấy thông tin nhân viên
            var employee = employeeRepository.GetEmployeeById(_employeeId);
            if (employee == null)
            {
                MessageBox.Show("Không tìm thấy thông tin nhân viên.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Kiểm tra nếu số ngày nghỉ phép vượt quá số ngày còn lại
            if (leaveDays > employee.RemainingLeaveDays)
            {
                MessageBox.Show($"Không thể gửi đơn nghỉ phép vì số ngày nghỉ ({leaveDays} ngày) vượt quá số ngày còn lại ({employee.RemainingLeaveDays} ngày).", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // Dừng thực thi phương thức
            }

            // Tạo đối tượng LeaveRequest từ dữ liệu nhập
            var leaveRequest = new Leaves
            {
                LeaveType = SelectedLeaveType,
                StartDate = StartDate.Value,
                EndDate = EndDate.Value,
                EmployeeId = _employeeId, // Thiết lập EmployeeID
                Status = "Pending" // Trạng thái mặc định
            };

            var user1 = userRepository.GetUserByEmployeeId(leaveRequest.EmployeeId);
            try
            {
                // Thêm đơn nghỉ phép vào cơ sở dữ liệu
                _leaveRepository.AddLeaveRequest(leaveRequest);

                // Hiển thị thông báo thành công
                MessageBox.Show($"Đơn nghỉ phép đã được gửi:\nLoại: {leaveRequest.LeaveType}\nTừ: {leaveRequest.StartDate.ToShortDateString()}\nĐến: {leaveRequest.EndDate.ToShortDateString()}",
                               "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                // Ghi log hoạt động
                _activityLogRepository.LogActivity(user1.UserId, "Nhân viên gửi đơn xin nghỉ phép", $"Nhân viên {employee.FullName} xin nghỉ phép");

                // Đóng cửa sổ sau khi gửi thành công
                (parameter as Window)?.Close();
            }
            catch (Exception ex)
            {
                // Hiển thị thông báo lỗi
                string errorMessage = $"Lỗi khi gửi đơn nghỉ phép: {ex.Message}";
                if (ex.InnerException != null)
                {
                    errorMessage += $"\nChi tiết: {ex.InnerException.Message}";
                }
                MessageBox.Show(errorMessage, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel(object parameter)
        {
            // Đóng cửa sổ khi nhấn nút "Huỷ"
            (parameter as Window)?.Close();
        }

        // Triển khai INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}