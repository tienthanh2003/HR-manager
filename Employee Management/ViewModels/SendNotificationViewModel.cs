using Employee_Management.Models;
using Employee_Management.Repository;
using Employee_Management.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Employee_Management.ViewModels
{
    public class SendNotificationViewModel : INotifyPropertyChanged
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IDepartmentRepository departmentRepository;
        private readonly IActivityLogRepository activityLogRepository;

        // Thuộc tính bind với giao diện
        private string _title;
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }
        private DateTime _endDate;
        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                _endDate = value;
                OnPropertyChanged(nameof(EndDate));
            }
        }

        private string _content;
        public string Content
        {
            get { return _content; }
            set
            {
                _content = value;
                OnPropertyChanged(nameof(Content));
            }
        }

        private bool _isBroadcast = true;
        public bool IsBroadcast
        {
            get { return _isBroadcast; }
            set
            {
                _isBroadcast = value;
                OnPropertyChanged(nameof(IsBroadcast));
                OnPropertyChanged(nameof(DepartmentPanelVisibility)); // Cập nhật Visibility khi IsBroadcast thay đổi
            }
        }

        private int _selectedDepartmentId;
        public int SelectedDepartmentId
        {
            get { return _selectedDepartmentId; }
            set
            {
                _selectedDepartmentId = value;
                OnPropertyChanged(nameof(SelectedDepartmentId));
            }
        }

        // Danh sách phòng ban
        public List<Department> Departments { get; set; }

        // Thuộc tính để binding Visibility của StackPanel chọn phòng ban
        public Visibility DepartmentPanelVisibility
        {
            get { return IsBroadcast ? Visibility.Collapsed : Visibility.Visible; }
        }

        // Command để gửi thông báo
        public ICommand SendNotificationCommand { get; }

        public ICommand CancelCommand { get; }



        public SendNotificationViewModel(INotificationRepository notificationRepository)
        {
            activityLogRepository = new ActivityLogRepository();
            _notificationRepository = notificationRepository;
            departmentRepository = new DepartmentRepository();
            // Lấy danh sách phòng ban có trạng thái "Active"
            Departments = departmentRepository.getDepartmentActive();
            EndDate = DateTime.Now.AddDays(7);

            // Khởi tạo command
            SendNotificationCommand = new RelayCommand(SendNotification);
            CancelCommand = new RelayCommand(ExecuteCancelCommand);
        }

        private void SendNotification(object parameter)
        {
            try
            {
                if(IsBroadcast)
                {
                    var notification = new Notification
                    {
                        Title = Title,
                        Content = Content,
                        SenderId = 1, // Thay bằng ID người gửi thực tế
                        IsBroadcast = IsBroadcast,
                        SentDate = DateTime.Now,
                        EndDate = EndDate, // Sử dụng giá trị từ DatePicker
                    };
                    // Gửi thông báo thông qua repository
                    _notificationRepository.AddNotification(notification);
                    activityLogRepository.LogActivity(1, "Thông báo mới", "Có thông báo mới tới toàn bộ công ty");

                }
                else
                {
                    var notification = new Notification
                    {
                        Title = Title,
                        Content = Content,
                        SenderId = 1, // Thay bằng ID người gửi thực tế
                        IsBroadcast = IsBroadcast,
                        SentDate = DateTime.Now,
                        EndDate = EndDate, // Sử dụng giá trị từ DatePicker
                    };
                    _notificationRepository.AddNotification(notification);
                    var nofidaprtment = new NotificationDepartment
                    {
                        NotificationId = notification.NotificationId,
                        DepartmentId = SelectedDepartmentId

                    };
                    _notificationRepository.AddnotifiDepartment(nofidaprtment);
                    if (nofidaprtment.DepartmentId.HasValue)
                    {
                        var department = departmentRepository.GetDepartmentById(nofidaprtment.DepartmentId.Value);
                        activityLogRepository.LogActivity(1, "Thông báo mới", $"Có thông báo mới tới phòng {department.DepartmentName} ");
                        // Tiếp tục xử lý với department
                    }
                    else
                    {
                        // Xử lý trường hợp DepartmentId là null
                        MessageBox.Show("DepartmentId không được để trống.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    
                }


                // Hiển thị thông báo thành công
                MessageBox.Show("Thông báo đã được gửi thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                var main = new MainWindow();
                main.Show();
                // Đóng cửa sổ sau khi gửi
                (parameter as Window)?.Close();
            }
            catch (Exception ex)
            {
                // Hiển thị thông báo lỗi
                MessageBox.Show($"Lỗi khi gửi thông báo: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteCancelCommand(object parameter)
        {
            if (parameter is Window window)
            {
               

                // Mở lại cửa sổ chính (MainWindow)
                var mainWindow = new MainWindow();
                mainWindow.Show();
                // Đóng cửa sổ hiện tại
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