using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Employee_Management.Models;
using Employee_Management.Repository;
using Employee_Management.View;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Employee_Management.ViewModels
{
    public class ActivityLogViewModel : INotifyPropertyChanged
    {
        private readonly IActivityLogRepository _activityLogRepository;
        private readonly IUserRepository userRepository;

        // Danh sách lịch sử hoạt động
        private List<ActivityViewModel> _activityLogs;
        public List<ActivityViewModel> ActivityLogs
        {
            get => _activityLogs;
            set
            {
                _activityLogs = value;
                OnPropertyChanged();
            }
        }

        // Danh sách người dùng
        private List<User> _users;
        public List<User> Users
        {
            get => _users;
            set
            {
                _users = value;
                OnPropertyChanged();
            }
        }

        // Người dùng được chọn
        private int _selectedUserId;
        public int SelectedUserId
        {
            get => _selectedUserId;
            set
            {
                _selectedUserId = value;
                OnPropertyChanged();
                Loadlog();
            }
        }

        // Từ khóa tìm kiếm
        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                Loadlog();
            }
        }

        // Command để tải lại dữ liệu
        public ICommand LoadCommand { get; }
        public ICommand BackCommand { get; }

        // Constructor
        public ActivityLogViewModel(IActivityLogRepository activityLogRepository)
        {
            _activityLogRepository = activityLogRepository;
            userRepository = new UserRepository();
            ActivityLogs = new List<ActivityViewModel>();
            Users = new List<User>();
            // Đặt giá trị mặc định cho SelectedUserId là -1 (Tất cả)
            SelectedUserId = -1;
            LoadCommand = new RelayCommand(LoadActivityLogs);
            BackCommand = new RelayCommand(Back);

            LoadUsers();
            Loadlog();
        }

        // Tải danh sách người dùng
        private void LoadUsers()
        {
            var users = userRepository.GetAll();
            users.Insert(0, new User { UserId = -1, Username = "Tất cả" });
            // Giả sử bạn có một repository để lấy danh sách người dùng
           Users = users;

            
        }

        // Tải lịch sử hoạt động
        private void LoadActivityLogs(object parameter)
        {
            Loadlog();
            
        }

        private void Loadlog()
        {
            List<ActivityViewModel> logs = new List<ActivityViewModel>(); // Khởi tạo giá trị mặc định

            // Kiểm tra xem người dùng có chọn "Tất cả" không
            if (SelectedUserId == -1)
            {
                logs = _activityLogRepository.GetAllActivityLogs();


            }
            else
            {
                // Lấy lịch sử hoạt động của người dùng được chọn
                logs = _activityLogRepository.GetActivityByUserId(SelectedUserId);
            }

            // Lọc dữ liệu dựa trên từ khóa tìm kiếm
            if (!string.IsNullOrEmpty(SearchText))
            {
                logs = logs.Where(log =>
                    log.Action.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    log.Details.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    log.Username.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            ActivityLogs = logs;
        }
        // Quay lại cửa sổ trước đó
        private void Back(object parameter)
        {
            
            // Đóng cửa sổ hiện tại
            foreach (Window window in Application.Current.Windows)
            {
                if (window is ActivityLogWindow activity)
                {
                    activity.Close();
                    break;
                }
            }
        }

        // Triển khai INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}