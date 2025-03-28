using Employee_Management.Models;
using Employee_Management.Repository;
using Employee_Management.View;
using Employee_Management.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Employee_Management.ViewModel
{
    public class SalaryManagementViewModel : INotifyPropertyChanged
    {
        private readonly ISalaryRepository _salaryRepository;
        private readonly IActivityLogRepository _activityLogRepository;

        // Danh sách lương thưởng
        public ObservableCollection<SalaryViewModel> Salaries { get; set; }

        // Danh sách trạng thái lương
        public ObservableCollection<string> SalaryStatuses { get; set; }

        // Thuộc tính tìm kiếm
        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                Search(_searchText);
            }
        }

        // Thuộc tính trạng thái lương được chọn
        private string _selectedSalaryStatus;
        public string SelectedSalaryStatus
        {
            get => _selectedSalaryStatus;
            set
            {
                _selectedSalaryStatus = value;
                OnPropertyChanged(nameof(SelectedSalaryStatus));
                Search(_selectedSalaryStatus);
            }
        }

        // Các lệnh
        public ICommand SearchCommand { get; }
        public ICommand LoadCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand BackCommand { get; }

        public SalaryManagementViewModel()
        {
            // Khởi tạo repository
            _salaryRepository = new SalaryRespository();

            _activityLogRepository = new ActivityLogRepository();
            // Khởi tạo danh sách lương thưởng và trạng thái
            Salaries = new ObservableCollection<SalaryViewModel>();
            SalaryStatuses = new ObservableCollection<string> { "Pending", "Complete" };

            // Khởi tạo các lệnh
            SearchCommand = new RelayCommand(Search);
            LoadCommand = new RelayCommand(Load);
            UpdateCommand = new RelayCommand(Update);
            BackCommand = new RelayCommand(Back);

            // Tải dữ liệu ban đầu
            LoadSalaries();
        }

        // Phương thức tải dữ liệu lương thưởng
        private void LoadSalaries()
        {
            try
            {
                // Lấy tháng và năm hiện tại
                var currentDate = DateTime.Now;
                int month = currentDate.Month;
                int year = currentDate.Year;

                // Tạo lương tháng nếu cần
                _salaryRepository.GenerateMonthlySalary();
               

                // Lấy danh sách lương thưởng
                var salaries = _salaryRepository.GetAllSalariesWithEmployeeName(month, year);

                // Cập nhật danh sách lương thưởng
                Salaries.Clear();
                foreach (var salaryViewModel in salaries)
                {
                    Salaries.Add(salaryViewModel);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu lương: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Lệnh tìm kiếm
        private void Search(object parameter)
        {
            try
            {
                var filteredSalaries = _salaryRepository.GetAllSalariesWithEmployeeName(DateTime.Now.Month, DateTime.Now.Year);

                if (!string.IsNullOrEmpty(SearchText))
                {
                    filteredSalaries = filteredSalaries
                        .Where(s => s.EmployeeName.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                if (!string.IsNullOrEmpty(SelectedSalaryStatus))
                {
                    filteredSalaries = filteredSalaries
                        .Where(s => s.SalaryStatus == SelectedSalaryStatus)
                        .ToList();
                }

                Salaries.Clear();
                foreach (var salary in filteredSalaries)
                {
                    Salaries.Add(salary);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Lệnh tải lại dữ liệu
        private void Load(object parameter)
        {
            LoadSalaries();
        }

        // Lệnh cập nhật
        private void Update(object parameter)
        {
            try
            {
                if (parameter is SalaryViewModel selectedSalary)
                {
                    var updateSalaryWindow = new UpdateSalaryWindow(selectedSalary);
                    updateSalaryWindow.ShowDialog();

                    // Tải lại dữ liệu sau khi cập nhật
                    LoadSalaries();
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn một bản ghi lương để cập nhật.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật lương: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    if (window is SalaryManagementWindow salaryManagement)
                    {
                        salaryManagement.Close();
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