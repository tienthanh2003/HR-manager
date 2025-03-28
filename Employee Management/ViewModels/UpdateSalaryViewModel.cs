using Employee_Management.Models;
using Employee_Management.Repository;
using Employee_Management.ViewModels;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows;

public class UpdateSalaryViewModel : INotifyPropertyChanged
{
    private readonly ISalaryRepository _salaryRepository;
    private SalaryViewModel _selectedSalary;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IActivityLogRepository _activityLogRepository;

    public UpdateSalaryViewModel(SalaryViewModel selectedSalary, ISalaryRepository salaryRepository)
    {
        _salaryRepository = salaryRepository;
        _selectedSalary = selectedSalary;
        _employeeRepository = new EmployeeRepository();
        _activityLogRepository = new ActivityLogRepository();

        // Khởi tạo giá trị từ đối tượng được chọn
        BaseSalary = selectedSalary.BaseSalary ?? 0;
        Allowance = selectedSalary.Allowance ?? 0;
        Bonus = selectedSalary.Bonus ?? 0;
        Deduction = selectedSalary.Deduction ?? 0;
        SelectedSalaryStatus = selectedSalary.SalaryStatus;
        TotalSalary = BaseSalary + Allowance + Bonus - Deduction;

        // Khởi tạo danh sách trạng thái lương
        SalaryStatuses = new List<string> { "Pending", "Complete" };

        // Khởi tạo các lệnh
        UpdateSalaryCommand = new RelayCommand(UpdateSalary);
        CancelCommand = new RelayCommand(Cancel);
    }

    // Các thuộc tính
    public decimal BaseSalary { get; set; }
    public decimal Allowance { get; set; }
    public decimal Bonus { get; set; }
    public decimal Deduction { get; set; }
    public string SelectedSalaryStatus { get; set; }
    public decimal TotalSalary { get; private set; } // Chỉ đọc
    public List<string> SalaryStatuses { get; set; }

    // Các lệnh
    public ICommand UpdateSalaryCommand { get; }
    public ICommand CancelCommand { get; }

    // Xử lý cập nhật lương
    private void UpdateSalary(object parameter)
    {
        try
        {
            // Cập nhật giá trị vào đối tượng được chọn
            _selectedSalary.BaseSalary = BaseSalary;
            _selectedSalary.Allowance = Allowance;
            _selectedSalary.Bonus = Bonus;
            _selectedSalary.Deduction = Deduction;
            _selectedSalary.SalaryStatus = SelectedSalaryStatus;
            _selectedSalary.TotalSalary = BaseSalary + Allowance + Bonus - Deduction;

            // Cập nhật vào cơ sở dữ liệu

            _salaryRepository.UpdateSalary(_selectedSalary);// Lưu thay đổi
            var employee = _employeeRepository.GetEmployeeById(_selectedSalary.EmployeeId);
            _activityLogRepository.LogActivity(1, "Admin đã cập nhật mới lương tháng", $"Admin đã cập nhật lương tháng cho nhân viên {employee.FullName}");
            if (employee != null)
            {
                employee.Salary = BaseSalary; // Cập nhật lương cơ bản trong bảng Employee
                _employeeRepository.UpdateEmployee(employee);
               
            }

            // Đóng cửa sổ
            if (parameter is Window window)
            {
                window.Close();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi khi cập nhật lương: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    // Xử lý hủy
    private void Cancel(object parameter)
    {
        if (parameter is Window window)
        {
            window.Close();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}