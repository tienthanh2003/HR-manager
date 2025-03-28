using Employee_Management.Models;
using Employee_Management.Repository;
using Employee_Management.View;
using Employee_Management.ViewModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Employee_Management.ViewModels
{
    public class EmployeeDashboardViewModel : INotifyPropertyChanged
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IAttendRepository _attendRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IActivityLogRepository _activityLogRepository;
        private readonly IUserRepository _userRepository;
        private DateTime datenow = DateTime.Today;
        private Employee _employee;
        private Department _department;

        // Thuộc tính
        public string FullName => _employee.FullName;
        public string Position => _employee.Position;
        public string Department => _department?.DepartmentName; // Sử dụng thông tin Department đã tải
        public BitmapImage AvatarImage { get; set; }

        private string _checkInTime;
        public string CheckInTime
        {
            get => _checkInTime;
            set
            {
                _checkInTime = value;
                OnPropertyChanged(nameof(CheckInTime));
            }
        }

        private string _checkOutTime;
        public string CheckOutTime
        {
            get => _checkOutTime;
            set
            {
                _checkOutTime = value;
                OnPropertyChanged(nameof(CheckOutTime));
            }
        }

        private List<Notification> _notifications;
        private string _notificationMessage;

        public List<Notification> Notifications
        {
            get => _notifications;
            set
            {
                _notifications = value;
                OnPropertyChanged(nameof(Notifications));
            }
        }


        public string NotificationMessage
        {
            get => _notificationMessage;
            set
            {
                _notificationMessage = value;
                OnPropertyChanged(nameof(NotificationMessage));
            }
        }

        // Lệnh
        public ICommand EditProfileCommand { get; }
        public ICommand RequestLeaveCommand { get; }
        public ICommand LogOutCommand { get; }

        public ICommand CheckInCommand { get; }
        public ICommand CheckOutCommand { get; }

        public EmployeeDashboardViewModel(Employee employee, IEmployeeRepository employeeRepository, IDepartmentRepository departmentRepository)
        {
            _employee = employee;
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
            _attendRepository = new AttendRepository();
            _notificationRepository = new NotificationRepository();
            _activityLogRepository = new ActivityLogRepository();
            _userRepository = new UserRepository();
            // Tải thông tin Department
            LoadNotifications();
            LoadDepartment();
            LoadTodayAttendance();
            // Khởi tạo ảnh đại diện
            AvatarImage = LoadAvatar(_employee.Avatar);

            // Khởi tạo các lệnh
            EditProfileCommand = new RelayCommand(EditProfile);
            RequestLeaveCommand = new RelayCommand(RequestLeave);
            CheckInCommand = new RelayCommand(CheckIn);
            CheckOutCommand = new RelayCommand(CheckOut);
            LogOutCommand = new RelayCommand(LogOut);
        }

        private void LoadNotifications()
        {
            try
            {
                // Lấy danh sách thông báo dành cho nhân viên hiện tại
                Notifications = _notificationRepository.GetNotificationsForUser(_employee.EmployeeId);

                // Hiển thị thông báo mới nhất (nếu có)
                if (Notifications.Any())
                {
                    NotificationMessage = Notifications.First().Content;
                }
                else
                {
                    NotificationMessage = "Không có thông báo mới.";
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi (ví dụ: hiển thị thông báo lỗi)
                MessageBox.Show($"Lỗi khi tải thông báo: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadDepartment()
        {
            if (_employee.DepartmentId.HasValue)
            {
                _department = _departmentRepository.GetDepartmentById(_employee.DepartmentId.Value);
                OnPropertyChanged(nameof(Department)); // Thông báo thay đổi cho thuộc tính Department
            }
        }

        private BitmapImage LoadAvatar(byte[] avatarData)
        {
            if (avatarData == null || avatarData.Length == 0) return null;

            var image = new BitmapImage();
            using (var stream = new System.IO.MemoryStream(avatarData))
            {
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = stream;
                image.EndInit();
            }
            return image;
        }

        private void EditProfile(object parameter)
        {
            // Mở cửa sổ chỉnh sửa thông tin
            var editProfileWindow = new EditProfileWindow(_employee);
            if (editProfileWindow.ShowDialog() == true)
            {
                // Cập nhật thông tin nhân viên sau khi chỉnh sửa
                _employee = _employeeRepository.GetEmployeeById(_employee.EmployeeId);
                LoadDepartment(); // Tải lại thông tin Department nếu có thay đổi
                OnPropertyChanged(nameof(FullName));
                OnPropertyChanged(nameof(Position));
                OnPropertyChanged(nameof(Department));
                AvatarImage = LoadAvatar(_employee.Avatar);
                OnPropertyChanged(nameof(AvatarImage));
            }
        }

        private void RequestLeave(object parameter)
        {
            var leaveRequestWindow = new LeaveRequestWindow(_employee);
            leaveRequestWindow.DataContext = new LeaveRequestViewModel(_employee.EmployeeId); 
            leaveRequestWindow.ShowDialog();

        }

        private void LoadTodayAttendance()
        {
            var todayAttendance = _attendRepository.GetTodayAttendance(_employee.EmployeeId,datenow);
            if (todayAttendance != null)
            {
                CheckInTime = todayAttendance.CheckInTime?.ToString("HH:mm");
                CheckOutTime = todayAttendance.CheckOutTime?.ToString("HH:mm");
            }
        }

        private void CheckIn(object parameter)
        {
                var todayAttendance = _attendRepository.GetTodayAttendance(_employee.EmployeeId, datenow);
            Console.WriteLine($"EmployeeId: {_employee.EmployeeId}");
            MessageBox.Show($"EmployeeId: {_employee.EmployeeId} - Date : {datenow}", "Thông tin EmployeeId", MessageBoxButton.OK, MessageBoxImage.Information);
            var userd = _userRepository.GetUserByEmployeeId(_employee.EmployeeId);
            _activityLogRepository.LogActivity(userd.UserId, "Nhân viên check in",$"Nhân viên {_employee.FullName} check in");
            // Kiểm tra xem todayAttendance có null không
            if (todayAttendance.CheckInTime == null)
                {
                    // Cập nhật Check-in
                    todayAttendance.CheckInTime = TimeOnly.FromDateTime(DateTime.Now); // Sử dụng TimeOnly
                    todayAttendance.AttendStatus = "Attend";
                    _attendRepository.UpdateAttendance(todayAttendance);
                    CheckInTime = todayAttendance.CheckInTime?.ToString("HH:mm");
                    MessageBox.Show("Check-in thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Bạn đã Check-in hôm nay!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }


        private void CheckOut(object parameter)
        {
            try
            {
                var todayAttendance = _attendRepository.GetTodayAttendance(_employee.EmployeeId, datenow);
                if (todayAttendance != null && todayAttendance.CheckInTime != null && todayAttendance.CheckOutTime == null)
                {
                    // Cập nhật Check-out
                    todayAttendance.CheckOutTime = TimeOnly.FromDateTime(DateTime.Now); // Sử dụng TimeOnly

                    // Tính toán OvertimeHours
                    var checkInTime = todayAttendance.CheckInTime.Value;
                    var checkOutTime = todayAttendance.CheckOutTime.Value;

                    // Tính tổng số giờ làm việc
                    var totalHours = (checkOutTime - checkInTime).TotalHours;

                    // Kiểm tra nếu totalHours quá nhỏ (ví dụ: nhỏ hơn 0.1 giờ)
                    if (totalHours < 0.1)
                    {
                        MessageBox.Show("Thời gian làm việc quá ngắn, không tính Overtime.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                        todayAttendance.OvertimeHours = 0; // Đặt OvertimeHours thành 0
                    }
                    else
                    {
                        // Tính OvertimeHours: TotalHours - 8 (giờ làm việc tiêu chuẩn)
                        var overtimeHours = totalHours - 8;

                        // Làm tròn OvertimeHours đến 2 chữ số thập phân
                        todayAttendance.OvertimeHours = (decimal)Math.Round(overtimeHours, 2, MidpointRounding.AwayFromZero);
                    }

                    _attendRepository.UpdateAttendance(todayAttendance);
                    CheckOutTime = todayAttendance.CheckOutTime?.ToString("HH:mm");
                    MessageBox.Show($"Check-out thành công! Overtime: {todayAttendance.OvertimeHours:F2} giờ", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    var userd = _userRepository.GetUserByEmployeeId(_employee.EmployeeId);
                    _activityLogRepository.LogActivity(userd.UserId, "Nhân viên check out", $"Nhân viên {_employee.FullName} check out");
                }
                else if (todayAttendance?.CheckOutTime != null)
                {
                    MessageBox.Show("Bạn đã Check-out hôm nay!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    MessageBox.Show("Vui lòng Check-in trước khi Check-out!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi Check-out: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void LogOut(object parameter)
        {
            try
            {
                // Hiển thị hộp thoại xác nhận
                var result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận đăng xuất",
                                             MessageBoxButton.YesNo, MessageBoxImage.Question);

                // Nếu người dùng chọn Yes
                if (result == MessageBoxResult.Yes)
                {
                    // Xử lý đăng xuất
                    

                    // Đóng cửa sổ hiện tại và mở cửa sổ đăng nhập
                    var loginWindow = new Login();
                    loginWindow.Show();

                    if (parameter is Window window)
                    {
                        window.Close();
                    }
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi : {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}