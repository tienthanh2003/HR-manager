using Employee_Management.Models;
using Employee_Management.Repository;
using Employee_Management.View;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Employee_Management.ViewModels
{
    public class LeaveManagementViewModel : INotifyPropertyChanged
    {
        private readonly ILeaveRepository _leaveRepository;
        private readonly IActivityLogRepository activityLogRepository;

        public ObservableCollection<LeaveViewModel> Leaves { get; set; }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                FilterLeaves();
            }
        }

        private string _selectedStatus;
        public string SelectedStatus
        {
            get => _selectedStatus;
            set
            {
                _selectedStatus = value;
                OnPropertyChanged(nameof(SelectedStatus));
                FilterLeaves();
            }
        }

        public List<string> Statuses { get; } = new List<string> { "All", "Pending", "Approved", "Rejected" };

        public ICommand SearchCommand { get; }
        public ICommand LoadCommand { get; }
        public ICommand BackCommand { get; }
        public ICommand ApproveCommand { get; }
        public ICommand RejectCommand { get; }

        public LeaveManagementViewModel(ILeaveRepository leaveRepository)
        {
            _leaveRepository = leaveRepository;
            Leaves = new ObservableCollection<LeaveViewModel>();
            activityLogRepository = new ActivityLogRepository();

            SearchCommand = new RelayCommand(SearchLeaves);
            LoadCommand = new RelayCommand(LoadLeaves);
            BackCommand = new RelayCommand(BackToMain);
            ApproveCommand = new RelayCommand(ApproveLeave);
            RejectCommand = new RelayCommand(RejectLeave);

            LoadLeaves();
        }

        private void LoadLeaves(object parameter = null)
        {
            var leaves = _leaveRepository.GetAllLeaves();
            Leaves.Clear();
            foreach (var leave in leaves)
            {
                Leaves.Add(leave);
            }
        }

        private void SearchLeaves(object parameter)
        {
            FilterLeaves();
        }

        private void FilterLeaves()
        {
            var leaves = _leaveRepository.GetAllLeaves();

            // Lọc theo tìm kiếm
            if (!string.IsNullOrEmpty(SearchText))
            {
                leaves = leaves.Where(l => l.EmployeeName.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Lọc theo trạng thái
            if (!string.IsNullOrEmpty(SelectedStatus) && SelectedStatus != "All")
            {
                leaves = leaves.Where(l => l.Status == SelectedStatus).ToList();
            }

            // Cập nhật danh sách hiển thị
            Leaves.Clear();
            foreach (var leave in leaves)
            {
                Leaves.Add(leave);
            }
        }

        private void ApproveLeave(object parameter)
        {
            if (parameter is LeaveViewModel leave)
            {
                leave.Status = "Approved";
                _leaveRepository.UpdateLeave(leave);
                activityLogRepository.LogActivity(1, "duyệt đơn xin nghỉ phép",$"admin duyệt đơn xin nghỉ phép loại:{leave.LeaveType}");
                FilterLeaves(); // Cập nhật danh sách
            }
        }

        private void RejectLeave(object parameter)
        {
            if (parameter is LeaveViewModel leave)
            {
                leave.Status = "Rejected";
                _leaveRepository.UpdateLeave(leave);
                activityLogRepository.LogActivity(1, "từ chối đơn xin nghỉ phép", $"admin từ chối xin nghỉ phép loại:{leave.LeaveType}");
                FilterLeaves(); // Cập nhật danh sách
            }
        }

        private void BackToMain(object parameter)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();

            foreach (Window window in Application.Current.Windows)
            {
                if (window is LeaveManagement leaveManagement)
                {
                    leaveManagement.Close();
                    break;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}