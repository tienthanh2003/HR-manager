using Employee_Management.Models;
using Employee_Management.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Employee_Management.Repository;
using System.Windows;
using Employee_Management.View;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

public class EmployeeManagementViewModel : INotifyPropertyChanged
{
    public ObservableCollection<Employee> Employees { get; set; }

    private ObservableCollection<Department> _departments;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IUserRepository _userRepository;
    private readonly IActivityLogRepository _activityLogRepository;
    public ObservableCollection<Department> Departments
    {
        get { return _departments; }
        set
        {
            _departments = value;
            OnPropertyChanged(nameof(Departments));
        }
    }

    private int _selectedDepartmentId;
    public int SelectedDepartmentId
    {
        get { return _selectedDepartmentId; }
        set
        {
            if (_selectedDepartmentId != value)
            {
                _selectedDepartmentId = value;
                OnPropertyChanged(nameof(SelectedDepartmentId));
                FilterEmployees();// Gọi phương thức lọc nhân viên
            }
        }
    }
    public List<string> Genders { get; } = new List<string> { "Male", "Female","All" };
    private string _selectedGender;
    public string SelectedGender
    {
        get => _selectedGender;
        set
        {
            _selectedGender = value;
            OnPropertyChanged(nameof(SelectedGender));
            FilterEmployees();
        }
    }


    private string _searchText;
    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            OnPropertyChanged(nameof(SearchText));
        }
    }

    public List<string> SalaryRanges { get; } = new List<string>
    {
    "Tất cả", "Dưới 10 triệu", "10 - 30 triệu",  "Trên 30 triệu"
    };

    private string _selectedSalaryRange;
    public string SelectedSalaryRange
    {
        get => _selectedSalaryRange;
        set
        {
            _selectedSalaryRange = value;
            OnPropertyChanged(nameof(SelectedSalaryRange));
            FilterEmployees();
        }
    }



    public ICommand SearchCommand { get; }
    public ICommand LoadCommand { get; }
    public ICommand AddCommand { get; }
    public ICommand UpdateCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand BackCommand { get; }

    public ICommand CreateAccountCommand { get; }

    public EmployeeManagementViewModel(IEmployeeRepository employeeRepository , IDepartmentRepository departmentRepository)
    {

        _activityLogRepository = new ActivityLogRepository();
        Employees = new ObservableCollection<Employee>();
        SearchCommand = new RelayCommand(SearchEmployees);
        LoadCommand = new RelayCommand(LoadEmployees);
        AddCommand = new RelayCommand(AddEmployee);
        UpdateCommand = new RelayCommand(UpdateEmployee);
        DeleteCommand = new RelayCommand(DeleteEmployee);
        BackCommand = new RelayCommand(BackToMain);
        CreateAccountCommand = new RelayCommand(CreateAccount);
        _employeeRepository = employeeRepository;
        _departmentRepository = departmentRepository;
        _userRepository = new UserRepository();
        LoadDepartments();
        LoadEmployeesData();
    }


    private void FilterEmployees()
    {
        IEnumerable<Employee> filteredEmployees = _employeeRepository.getAllEmployee();

        // Lọc theo phòng ban (nếu được chọn)
        if (SelectedDepartmentId > 0)
        {
            filteredEmployees = filteredEmployees.Where(e => e.DepartmentId == SelectedDepartmentId);
        }

        // Lọc theo giới tính (nếu không chọn "All")
        if (!string.IsNullOrEmpty(SelectedGender) && SelectedGender != "All")
        {
            filteredEmployees = filteredEmployees.Where(e => e.Gender == SelectedGender);
        }

        // Lọc theo tìm kiếm (nếu có nội dung)
        if (!string.IsNullOrEmpty(SearchText))
        {
            filteredEmployees = filteredEmployees.Where(e => e.FullName.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
        }

        // Lọc theo mức lương
        switch (SelectedSalaryRange)
        {
            case "Dưới 10 triệu":
                filteredEmployees = filteredEmployees.Where(e => e.Salary < 10000000);
                break;
            case "10 - 30 triệu":
                filteredEmployees = filteredEmployees.Where(e => e.Salary >= 10000000 && e.Salary <= 30000000);
                break;
            case "Trên 30 triệu":
                filteredEmployees = filteredEmployees.Where(e => e.Salary > 30000000);
                break;
        }

        // Cập nhật danh sách hiển thị
        Employees = new ObservableCollection<Employee>(filteredEmployees);
        OnPropertyChanged(nameof(Employees));
    }



    private void LoadDepartments()
    {
        try
        {
            var departments = _departmentRepository.getDepartmentActive();

            // Cập nhật ObservableCollection
            Departments = new ObservableCollection<Department>(departments);

            // Thêm một mục "Tất cả phòng ban" (nếu cần)
            Departments.Insert(0, new Department { DepartmentId = 0, DepartmentName = "Tất cả phòng ban" });
        }
        catch (Exception ex)
        {
            // Hiển thị thông báo lỗi cho người dùng
            MessageBox.Show($"Lỗi khi tải danh sách phòng ban: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void SearchEmployees(object parameter)
    {
        // Logic tìm kiếm nhân viên
        FilterEmployees();
    }

    private void LoadEmployeesData()
    {
        try
        {
            // Gọi phương thức GetAllEmployees từ repository
            var employees = _employeeRepository.getAllEmployee();

            // Cập nhật ObservableCollection với dữ liệu nhân viên
            Employees = new ObservableCollection<Employee>(employees);
        }
        catch (Exception ex)
        {
            // Hiển thị thông báo lỗi cho người dùng
            MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void LoadEmployees(object parameter)
    {
       LoadEmployeesData();
    }

    private void CreateAccount(object parameter)
    {
        if (parameter is Employee employee)
        {
            // Kiểm tra xem nhân viên đã có tài khoản hay chưa
            var existingUser = _userRepository.GetUserByEmployeeId(employee.EmployeeId);

            if (existingUser != null)
            {
                MessageBox.Show("Nhân viên này đã có tài khoản!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Tạo cửa sổ nhập thông tin tài khoản
            var createAccountWindow = new CreateAccountWindow(employee);
            var viewModel = new CreateAccountViewModel(_userRepository, employee);
            createAccountWindow.DataContext = viewModel;

            // Mở cửa sổ và chờ kết quả
            if (createAccountWindow.ShowDialog() == true)
            {
                MessageBox.Show("Tạo tài khoản thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }


    private void AddEmployee(object parameter)
    {
        var addEmployee = new AddEmployee();
        addEmployee.ShowDialog();
        CloseWindow(Application.Current.Windows.OfType<EmployeeManagement>().FirstOrDefault());
    }

    private void UpdateEmployee(object parameter)
    {
        if (parameter is Employee selectedEmployee)
        {
            // Mở cửa sổ chỉnh sửa nhân viên
            var editEmployeeWindow = new UpdateEmployees(selectedEmployee);
            bool? result = editEmployeeWindow.ShowDialog();

            // Nếu người dùng lưu thay đổi
            if (result == true)
            {
                try
                {
                    _employeeRepository.UpdateEmployee(selectedEmployee);
                   
                    LoadEmployeesData(); // Refresh danh sách nhân viên
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi cập nhật nhân viên: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        else
        {
            MessageBox.Show("Vui lòng chọn một nhân viên để cập nhật.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }


    private void DeleteEmployee(object parameter)
    {
        if (parameter is Employee selectedEmployee)
        {
            // Hiển thị hộp thoại xác nhận trước khi xóa
            var result = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa nhân viên {selectedEmployee.FullName}?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Gọi phương thức xóa nhân viên từ repository
                    _employeeRepository.DeleteEmployee(selectedEmployee.EmployeeId);
                    _activityLogRepository.LogActivity(1, "Xoá nhân viên", $"Xoá nhân viên {selectedEmployee.FullName} ");
                    // Cập nhật danh sách nhân viên
                    LoadEmployeesData();

                    // Hiển thị thông báo thành công
                    MessageBox.Show("Xóa nhân viên thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    // Hiển thị thông báo lỗi nếu có vấn đề xảy ra
                    MessageBox.Show($"Lỗi khi xóa nhân viên: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        else
        {
            MessageBox.Show("Vui lòng chọn một nhân viên để xóa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void BackToMain(object parameter)
    {
        // Logic quay lại cửa sổ chính
        var mainWindow = new MainWindow();
        mainWindow.Show();

        // Đóng cửa sổ hiện tại
        foreach (Window window in Application.Current.Windows)
        {
            if (window is EmployeeManagement employeeManagement)
            {
                employeeManagement.Close();
                break;
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void CloseWindow(Window window)
    {
        window.Close();
    }
}