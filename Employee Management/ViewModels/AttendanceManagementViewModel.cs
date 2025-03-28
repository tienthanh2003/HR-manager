using Employee_Management.Models;
using Employee_Management.Repository;
using Employee_Management.View;
using Employee_Management.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

public class AttendanceManagementViewModel : INotifyPropertyChanged
{
    private DateTime? _filterDate;
    private string _selectedAttendanceStatus;
    private readonly AttendRepository _attendRepository;
    private List<AttendanceViewModel> _attendances;

    // Danh sách tháng (1-12)
    public ObservableCollection<int> Months { get; set; } = new ObservableCollection<int>(Enumerable.Range(1, 12));

    // Danh sách năm (ví dụ: từ 2020 đến 2030)
    public ObservableCollection<int> Years { get; set; } = new ObservableCollection<int>(Enumerable.Range(2020, 11));

    // Thuộc tính để lưu tháng và năm được chọn
    private int _selectedMonth;
    public int SelectedMonth
    {
        get => _selectedMonth;
        set
        {
            _selectedMonth = value;
            OnPropertyChanged(nameof(SelectedMonth));
        }
    }

    private int _selectedYear;
    public int SelectedYear
    {
        get => _selectedYear;
        set
        {
            _selectedYear = value;
            OnPropertyChanged(nameof(SelectedYear));
        }
    }

    public List<AttendanceViewModel> Attendances
    {
        get => _attendances;
        set
        {
            _attendances = value;
            OnPropertyChanged(nameof(Attendances));
        }
    }

    public DateTime? FilterDate
    {
        get => _filterDate;
        set
        {
            _filterDate = value;
            OnPropertyChanged(nameof(FilterDate));
            FilterAttendance(); // Tự động lọc dữ liệu khi FilterDate thay đổi
        }
    }

    public string SelectedAttendanceStatus
    {
        get => _selectedAttendanceStatus;
        set
        {
            _selectedAttendanceStatus = value;
            OnPropertyChanged(nameof(SelectedAttendanceStatus));
            FilterAttendance(); // Tự động lọc dữ liệu khi SelectedAttendanceStatus thay đổi
        }
    }

    public List<string> AttendanceStatuses { get; } = new List<string> { "Tất cả", "Absent", "Attend" };

    public ICommand FilterCommand { get; }
    public ICommand LoadCommand { get; }
    public ICommand BackCommand { get; }
    public ICommand ViewReportCommand { get; } // Lệnh để xem báo cáo hàng tháng

    public AttendanceManagementViewModel(AttendRepository attendRepository)
    {
        _attendRepository = attendRepository;
        Months = new ObservableCollection<int>(Enumerable.Range(0, 13).ToList()); // 0 là "Tất cả", 1-12 là các tháng
       // 0 là "Tất cả", 1-4 là các quý

        // Đặt giá trị mặc định cho FilterDate là ngày hiện tại
        FilterDate = DateTime.Now.Date;

        int currentYear = DateTime.Now.Year;

        // Tạo danh sách năm từ (năm hiện tại - 10) đến năm hiện tại
        Years = new ObservableCollection<int>(Enumerable.Range(currentYear - 10, 11).ToList());

        // Đặt giá trị mặc định cho tháng và năm
        SelectedMonth = DateTime.Now.Month;
        SelectedYear = DateTime.Now.Year;

        FilterCommand = new RelayCommand(searchattend);
        LoadCommand = new RelayCommand(_ => LoadAttendance()); // Sử dụng lambda expression
        BackCommand = new RelayCommand(Back);
        ViewReportCommand = new RelayCommand(ViewReport); // Khởi tạo lệnh xem báo cáo

        LoadAttendance();
        FilterAttendance();
    }

    private void LoadAttendance()
    {
        try
        {
            if (_attendRepository == null)
            {
                throw new InvalidOperationException("Repository chưa được khởi tạo.");
            }

            Attendances = _attendRepository.GetAttendancesWithEmployeeName();
        }
        catch (Exception ex)
        {
            // Xử lý lỗi (ví dụ: hiển thị thông báo lỗi)
            MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void searchattend(object parameter)
    {
        FilterAttendance();
    }

    private void FilterAttendance()
    {
        try
        {
            _attendances = _attendRepository.GetAttendancesWithEmployeeName();
            if (_attendances == null)
            {
                throw new InvalidOperationException("Danh sách chấm công chưa được tải.");
            }

            var filtered = _attendances;

            if (FilterDate.HasValue)
            {
                filtered = filtered.Where(a => a.Date == FilterDate.Value).ToList();
            }

            if (!string.IsNullOrEmpty(SelectedAttendanceStatus) && SelectedAttendanceStatus != "Tất cả")
            {
                filtered = filtered.Where(a => a.AttendStatus == SelectedAttendanceStatus).ToList();
            }

            Attendances = filtered;
        }
        catch (Exception ex)
        {
            // Xử lý lỗi (ví dụ: hiển thị thông báo lỗi)
            MessageBox.Show($"Lỗi khi lọc dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ViewReport(object parameter)
    {
        try
        {
            // Lấy tháng và năm được chọn
            int month = SelectedMonth;
            int year = SelectedYear;

            // Gọi phương thức để lấy báo cáo chấm công hàng tháng
            var report = GetMonthlyAttendanceReport(month, year);

            // Cập nhật dữ liệu vào Attendances
            Attendances = report;
        }
        catch (Exception ex)
        {
            // Xử lý lỗi (ví dụ: hiển thị thông báo lỗi)
            MessageBox.Show($"Lỗi khi tạo báo cáo: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private List<AttendanceViewModel> GetMonthlyAttendanceReport(int month, int year)
    {
        // Lấy tất cả dữ liệu chấm công từ repository
        var allAttendances = _attendRepository.GetAttendancesWithEmployeeName();

        // Nếu tháng là 0 (tức là "Tất cả"), lọc theo năm
        if (month == 0)
        {
            return allAttendances
                .Where(a => a.Date.Year == year) // Chỉ lấy dữ liệu trong năm được chọn
                .ToList();
        }

        // Nếu tháng khác 0, lọc theo tháng và năm
        return allAttendances
            .Where(a => a.Date.Month == month && a.Date.Year == year)
            .ToList();
    }

    private void Back(object parameter)
    {
        var mainWindow = new MainWindow();
        mainWindow.Show();

        foreach (Window window in Application.Current.Windows)
        {
            if (window is AttendanceManagementWindow attendManagement)
            {
                attendManagement.Close();
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