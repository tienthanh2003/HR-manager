using ClosedXML.Excel;
using Employee_Management.Models;
using Employee_Management.Repository;
using Employee_Management.View;
using iText.Kernel.Pdf;
using iText.Layout.Properties;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

using iText.Layout;
using iText.Layout.Element;


namespace Employee_Management.ViewModels
{
    public class SalaryReportViewModel : INotifyPropertyChanged
    {
        private readonly ISalaryRepository _salaryRepository;
        private readonly IActivityLogRepository activityLogRepository;

        // Danh sách lương thưởng
        public ObservableCollection<SalaryViewModel> Salaries { get; set; }

        // Danh sách tháng, quý, năm
        public ObservableCollection<int> Months { get; } = new ObservableCollection<int>(Enumerable.Range(1, 12).ToList());
        public ObservableCollection<int> Quarters { get; } = new ObservableCollection<int>(Enumerable.Range(1, 4).ToList());
        public ObservableCollection<int> Years { get; } = new ObservableCollection<int>(Enumerable.Range(2020, 11).ToList()); // Từ năm 2020 đến 2030

        // Thuộc tính tháng, quý, năm được chọn
        private int _selectedMonth;
        public int SelectedMonth
        {
            get => _selectedMonth;
            set
            {
                _selectedMonth = value;
                OnPropertyChanged(nameof(SelectedMonth));
                LoadSalaries();
            }
        }

        private int _selectedQuarter;
        public int SelectedQuarter
        {
            get => _selectedQuarter;
            set
            {
                _selectedQuarter = value;
                OnPropertyChanged(nameof(SelectedQuarter));
                LoadSalaries();
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
                LoadSalaries();
            }
        }

        // Các lệnh
        public ICommand LoadCommand { get; }
        public ICommand ExportExcelCommand { get; }
        public ICommand ExportPdfCommand { get; }
        public ICommand BackCommand { get; }

        public SalaryReportViewModel()
        {
            // Khởi tạo repository
            _salaryRepository = new SalaryRespository();
            activityLogRepository = new ActivityLogRepository();
            // Khởi tạo danh sách lương thưởng
            Salaries = new ObservableCollection<SalaryViewModel>();

            // Khởi tạo các lệnh
            LoadCommand = new RelayCommand(LoadSalaries);
            ExportExcelCommand = new RelayCommand(ExportToExcel);
            ExportPdfCommand = new RelayCommand(ExportToPdf);
            BackCommand = new RelayCommand(Back);
            _salaryRepository = new SalaryRespository();

            // Khởi tạo danh sách lương thưởng
            Salaries = new ObservableCollection<SalaryViewModel>();

            // Khởi tạo danh sách tháng, quý, năm
            Months = new ObservableCollection<int>(Enumerable.Range(0, 13).ToList()); // 0 là "Tất cả", 1-12 là các tháng
            Quarters = new ObservableCollection<int>(Enumerable.Range(0, 5).ToList()); // 0 là "Tất cả", 1-4 là các quý
                                                                                       // Lấy năm hiện tại
            int currentYear = DateTime.Now.Year;

            // Tạo danh sách năm từ (năm hiện tại - 10) đến năm hiện tại
            Years = new ObservableCollection<int>(Enumerable.Range(currentYear - 10, 11).ToList());

            // Thiết lập giá trị mặc định cho tháng, quý, năm
            SelectedMonth = 0; // Mặc định là "Tất cả"
            SelectedQuarter = 0; // Mặc định là "Tất cả"
            SelectedYear = DateTime.Now.Year;

            // Tải dữ liệu ban đầu
            LoadSalaries();

            // Thiết lập giá trị mặc định cho tháng, quý, năm
            SelectedMonth = DateTime.Now.Month;
            SelectedQuarter = (DateTime.Now.Month - 1) / 3 + 1; // Tính quý từ tháng
            SelectedYear = DateTime.Now.Year;

            // Tải dữ liệu ban đầu
            LoadSalaries();
        }

        // Phương thức tải dữ liệu lương thưởng
        private void LoadSalaries(object parameter = null)
        {
            try
            {
                // Kiểm tra nếu cả tháng và quý đều không được chọn (giá trị 0)
                if (SelectedMonth == 0 && SelectedQuarter == 0)
                {
                    // Lấy danh sách lương thưởng chỉ theo năm
                    var salaries = _salaryRepository.GetSalariesByDate(0, 0, SelectedYear);

                    // Cập nhật danh sách lương thưởng
                    Salaries.Clear();
                    foreach (var salaryViewModel in salaries)
                    {
                        Salaries.Add(salaryViewModel);
                    }
                }
                else
                {
                    // Lấy danh sách lương thưởng dựa trên tháng, quý, năm được chọn
                    var salaries = _salaryRepository.GetSalariesByDate(SelectedMonth, SelectedQuarter, SelectedYear);

                    // Cập nhật danh sách lương thưởng
                    Salaries.Clear();
                    foreach (var salaryViewModel in salaries)
                    {
                        Salaries.Add(salaryViewModel);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu lương: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Phương thức xuất Excel
        private void ExportToExcel(object parameter)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel Files (*.xlsx)|*.xlsx",
                    DefaultExt = "xlsx",
                    FileName = "SalaryReport.xlsx"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    string filePath = saveFileDialog.FileName;

                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Salaries");
                        worksheet.Cell(1, 1).Value = "Tên nhân viên";
                        worksheet.Cell(1, 2).Value = "Lương cơ bản";
                        worksheet.Cell(1, 3).Value = "Phụ cấp";
                        worksheet.Cell(1, 4).Value = "Thưởng";
                        worksheet.Cell(1, 5).Value = "Khấu trừ";
                        worksheet.Cell(1, 6).Value = "Tổng lương";
                        worksheet.Cell(1, 7).Value = "Ngày bắt đầu";
                        worksheet.Cell(1, 8).Value = "Ngày thanh toán";
                        worksheet.Cell(1, 9).Value = "Trạng thái";

                        int row = 2;
                        foreach (var salary in Salaries)
                        {
                            worksheet.Cell(row, 1).Value = salary.EmployeeName;
                            worksheet.Cell(row, 2).Value = salary.BaseSalary;
                            worksheet.Cell(row, 3).Value = salary.Allowance;
                            worksheet.Cell(row, 4).Value = salary.Bonus;
                            worksheet.Cell(row, 5).Value = salary.Deduction;
                            worksheet.Cell(row, 6).Value = salary.TotalSalary;
                            worksheet.Cell(row, 7).Value = salary.StartDate.ToString("dd/MM/yyyy");
                            worksheet.Cell(row, 8).Value = salary.PaymentDate.ToString("dd/MM/yyyy");
                            worksheet.Cell(row, 9).Value = salary.SalaryStatus;
                            row++;
                        }

                        workbook.SaveAs(filePath);
                    }

                    MessageBox.Show("Xuất Excel thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    activityLogRepository.LogActivity(1, "Admin xuất dữ liệu báo cáo lương", "admin xuất dữ liệu Excel báo cáo lương");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất Excel: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Phương thức xuất PDF
        private void ExportToPdf(object parameter)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "PDF Files (*.pdf)|*.pdf",
                    DefaultExt = "pdf",
                    FileName = "SalaryReport.pdf"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    string filePath = saveFileDialog.FileName;

                    using (FileStream stream = new FileStream(filePath, FileMode.Create))
                    {
                        PdfWriter writer = new PdfWriter(stream);
                        PdfDocument pdfDoc = new PdfDocument(writer);
                        var document = new Document(pdfDoc);

                        // Thêm tiêu đề
                        document.Add(new Paragraph("Báo cáo lương")
                             .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                            .SetFontSize(16));
                        document.Add(new iText.Layout.Element.Paragraph("\n"));

                        // Tạo bảng dữ liệu
                        var table = new Table(UnitValue.CreatePercentArray(9)).UseAllAvailableWidth();
                        table.AddHeaderCell("Name");
                        table.AddHeaderCell("Base Salary");
                        table.AddHeaderCell("Allowance");
                        table.AddHeaderCell("Bonus");
                        table.AddHeaderCell("Deduction");
                        table.AddHeaderCell("Total");
                        table.AddHeaderCell("Start date");
                        table.AddHeaderCell("Payment date");
                        table.AddHeaderCell("Status");

                        // Đổ dữ liệu vào bảng
                        foreach (var salary in Salaries)
                        {
                            table.AddCell(salary.EmployeeName);
                            table.AddCell(salary.BaseSalary?.ToString("C") ?? "N/A");
                            table.AddCell(salary.Allowance?.ToString("C") ?? "N/A");
                            table.AddCell(salary.Bonus?.ToString("C") ?? "N/A");
                            table.AddCell(salary.Deduction?.ToString("C") ?? "N/A");
                            table.AddCell(salary.TotalSalary?.ToString("C") ?? "N/A");
                            table.AddCell(salary.StartDate.ToString("dd/MM/yyyy"));
                            table.AddCell(salary.PaymentDate.ToString("dd/MM/yyyy"));
                            table.AddCell(salary.SalaryStatus);
                        }

                        // Thêm bảng vào tài liệu
                        document.Add(table);

                        // Đóng tài liệu
                        document.Close();
                    }

                    MessageBox.Show("Xuất PDF thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    activityLogRepository.LogActivity(1, "Admin xuất dữ liệu báo cáo lương", "admin xuất dữ liệu PDF báo cáo lương");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất PDF: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Lệnh quay lại
        private void Back(object parameter)
        {
            try
            {
                var mainWindow = new MainWindow();
                mainWindow.Show();

                foreach (Window window in Application.Current.Windows)
                {
                    if (window is SalaryReportWindow salaryReportWindow)
                    {
                        salaryReportWindow.Close();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi quay lại: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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
