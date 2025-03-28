using Employee_Management.Models;
using Employee_Management.Repository;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using System.IO;
using System.Linq;
using Employee_Management.View;
using System.Reflection.Metadata;
using System.Windows.Documents;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using ClosedXML.Excel;

namespace Employee_Management.ViewModels
{
    public class EmployeeReportViewModel : INotifyPropertyChanged
    {
        // Danh sách nhân viên
        private ObservableCollection<Employee> _employees;
        private readonly IActivityLogRepository _activityLogRepository;
        public ObservableCollection<Employee> Employees
        {
            get => _employees;
            set
            {
                _employees = value;
                OnPropertyChanged(nameof(Employees));
            }
        }

        // Danh sách phòng ban
        private ObservableCollection<Department> _departments;
        public ObservableCollection<Department> Departments
        {
            get => _departments;
            set
            {
                _departments = value;
                OnPropertyChanged(nameof(Departments));
            }
        }

        // Các thuộc tính lọc
        private int _selectedDepartmentId;
        public int SelectedDepartmentId
        {
            get => _selectedDepartmentId;
            set
            {
                _selectedDepartmentId = value;
                OnPropertyChanged(nameof(SelectedDepartmentId));
                FilterEmployees();
            }
        }

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

        // Danh sách giới tính
        public List<string> Genders { get; } = new List<string> { "Tất cả", "Male", "Female" };

        // Danh sách mức lương
        public List<string> SalaryRanges { get; } = new List<string>
        {
            "Tất cả", "Dưới 10 triệu", "10 - 30 triệu", "Trên 30 triệu"
        };

        // Repository
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IDepartmentRepository _departmentRepository;

        // Các lệnh
        public ICommand SearchCommand { get; }
        public ICommand LoadCommand { get; }
        public ICommand ExportExcelCommand { get; }
        public ICommand ExportPdfCommand { get; }
        public ICommand BackCommand { get; }

        public EmployeeReportViewModel(IEmployeeRepository employeeRepository, IDepartmentRepository departmentRepository)
        {
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
            _activityLogRepository = new ActivityLogRepository();
            // Khởi tạo các lệnh
            SearchCommand = new RelayCommand(SearchEmployees);
            LoadCommand = new RelayCommand(LoadEmployees);
            ExportExcelCommand = new RelayCommand(ExportToExcel);
            ExportPdfCommand = new RelayCommand(ExportToPdf);
            BackCommand = new RelayCommand(BackToMain);

            // Tải dữ liệu ban đầu
            LoadDepartments();
            LoadEmployeesData();
        }

        // Tải danh sách phòng ban
        private void LoadDepartments()
        {
            try
            {
                var departments = _departmentRepository.getDepartmentActive();
                Departments = new ObservableCollection<Department>(departments);
                Departments.Insert(0, new Department { DepartmentId = 0, DepartmentName = "Tất cả phòng ban" });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách phòng ban: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Tải danh sách nhân viên
        private void LoadEmployeesData()
        {
            try
            {
                var employees = _employeeRepository.getAllEmployee();
                Employees = new ObservableCollection<Employee>(employees);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Lọc nhân viên
        private void FilterEmployees()
        {
            try
            {
                var filteredEmployees = _employeeRepository.getAllEmployee();

                // Lọc theo phòng ban
                if (SelectedDepartmentId > 0)
                {
                    filteredEmployees = filteredEmployees.Where(e => e.DepartmentId == SelectedDepartmentId).ToList();
                }

                // Lọc theo giới tính
                if (!string.IsNullOrEmpty(SelectedGender) && SelectedGender != "Tất cả")
                {
                    filteredEmployees = filteredEmployees.Where(e => e.Gender == SelectedGender).ToList();
                }

                // Lọc theo mức lương
                switch (SelectedSalaryRange)
                {
                    case "Dưới 10 triệu":
                        filteredEmployees = filteredEmployees.Where(e => e.Salary < 10000000).ToList();
                        break;
                    case "10 - 30 triệu":
                        filteredEmployees = filteredEmployees.Where(e => e.Salary >= 10000000 && e.Salary <= 30000000).ToList();
                        break;
                    case "Trên 30 triệu":
                        filteredEmployees = filteredEmployees.Where(e => e.Salary > 30000000).ToList();
                        break;
                }

                // Lọc theo tìm kiếm
                if (!string.IsNullOrEmpty(SearchText))
                {
                    filteredEmployees = filteredEmployees.Where(e => e.FullName.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                Employees = new ObservableCollection<Employee>(filteredEmployees);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lọc dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Tìm kiếm nhân viên
        private void SearchEmployees(object parameter)
        {
            FilterEmployees();
        }

        // Tải lại dữ liệu
        private void LoadEmployees(object parameter)
        {
            LoadEmployeesData();
        }

        // Xuất dữ liệu ra Excel
        private void ExportToExcel(object parameter)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel Files (*.xlsx)|*.xlsx",
                    DefaultExt = "xlsx",
                    FileName = "EmployeeReport.xlsx"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    string filePath = saveFileDialog.FileName;

                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Employees");
                        worksheet.Cell(1, 1).Value = "Tên";
                        worksheet.Cell(1, 2).Value = "Ngày sinh";
                        worksheet.Cell(1, 3).Value = "Giới tính";
                        worksheet.Cell(1, 4).Value = "Địa chỉ";
                        worksheet.Cell(1, 5).Value = "SĐT";
                        worksheet.Cell(1, 6).Value = "Chức vụ";
                        worksheet.Cell(1, 7).Value = "Lương Tháng";
                        worksheet.Cell(1, 8).Value = "Số ngày phép";
                        worksheet.Cell(1, 9).Value = "Ngày bắt đầu";

                        for (int i = 0; i < Employees.Count; i++)
                        {
                            var employee = Employees[i];
                            worksheet.Cell(i + 2, 1).Value = employee.FullName;
                            worksheet.Cell(i + 2, 2).Value = employee.DateOfBirth.ToString("dd/MM/yyyy");
                            worksheet.Cell(i + 2, 3).Value = employee.Gender;
                            worksheet.Cell(i + 2, 4).Value = employee.Address;
                            worksheet.Cell(i + 2, 5).Value = employee.PhoneNumber;
                            worksheet.Cell(i + 2, 6).Value = employee.Position;
                            worksheet.Cell(i + 2, 7).Value = employee.Salary;
                            worksheet.Cell(i + 2, 8).Value = employee.RemainingLeaveDays;
                            worksheet.Cell(i + 2, 9).Value = employee.StartDate.ToString("dd/MM/yyyy");
                        }

                        workbook.SaveAs(filePath);
                    }

                    MessageBox.Show("Xuất Excel thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    _activityLogRepository.LogActivity(1, "Xuất báo cáo", "Admin xuất báo cáo Excel nhân viên");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất Excel: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Xuất dữ liệu ra PDF
        private void ExportToPdf(object parameter)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "PDF Files (*.pdf)|*.pdf",
                    DefaultExt = "pdf",
                    FileName = "EmployeeReport.pdf"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    string filePath = saveFileDialog.FileName;

                    // Tạo file PDF
                    using (FileStream stream = new FileStream(filePath, FileMode.Create))
                    {
                        PdfWriter writer = new PdfWriter(stream);
                        PdfDocument pdfDoc = new PdfDocument(writer);
                        var document = new iText.Layout.Document(pdfDoc);

                        // Thêm tiêu đề
                        document.Add(new iText.Layout.Element.Paragraph("Báo cáo nhân viên")
                            .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                            .SetFontSize(16));
                        document.Add(new iText.Layout.Element.Paragraph("\n"));

                        // Tạo bảng dữ liệu
                        var table = new iText.Layout.Element.Table(UnitValue.CreatePercentArray(9)).UseAllAvailableWidth();
                        table.AddHeaderCell("Name");
                        table.AddHeaderCell("DOB");
                        table.AddHeaderCell("Gender");
                        table.AddHeaderCell("Address");
                        table.AddHeaderCell("Phone");
                        table.AddHeaderCell("Position");
                        table.AddHeaderCell("Salary");
                        table.AddHeaderCell("Number of leaveday");
                        table.AddHeaderCell("Start Date");

                        // Đổ dữ liệu vào bảng
                        foreach (var employee in Employees)
                        {
                            table.AddCell(employee.FullName ?? "N/A");
                            table.AddCell(employee.DateOfBirth.ToString("dd/MM/yyyy") ?? "N/A");
                            table.AddCell(employee.Gender ?? "N/A");
                            table.AddCell(employee.Address ?? "N/A");
                            table.AddCell(employee.PhoneNumber ?? "N/A");
                            table.AddCell(employee.Position ?? "N/A");
                            table.AddCell(employee.Salary?.ToString("C") ?? "N/A");
                            table.AddCell(employee.RemainingLeaveDays.ToString() ?? "N/A");
                            table.AddCell(employee.StartDate.ToString("dd/MM/yyyy") ?? "N/A");
                        }

                        // Thêm bảng vào tài liệu
                        document.Add(table);

                        // Đóng tài liệu
                        document.Close();
                    }

                    MessageBox.Show("Xuất PDF thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    _activityLogRepository.LogActivity(1, "Xuất báo cáo", "Admin xuất báo cáo PDF nhân viên");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất PDF: {ex.ToString()}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Quay lại cửa sổ chính
        private void BackToMain(object parameter)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();

            foreach (Window window in Application.Current.Windows)
            {
                if (window is EmployeeReportView employeeReportView)
                {
                    employeeReportView.Close();
                    break;
                }
            }
        }

        // Triển khai INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}