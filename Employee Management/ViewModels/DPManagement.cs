using Employee_Management.Models;
using Employee_Management.Repository;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows;
using System.Linq;
using Employee_Management.View;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Employee_Management.ViewModels
{
    public class DPManagementViewModel : INotifyPropertyChanged
    {
        private string _searchText;
        private ObservableCollection<Department> _departments;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IActivityLogRepository activityLogRepository;
        
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Department> Departments
        {
            get => _departments;
            set
            {
                _departments = value;
                OnPropertyChanged(nameof(Departments));
            }
        }

        public ICommand SearchCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand LoadCommand { get; }
        public ICommand BackCommand { get; }

        public ICommand DeleteCommand { get; }
        public ICommand UpdateCommand { get; }

        public DPManagementViewModel(IDepartmentRepository departmentRepository)
        {
            // Khởi tạo repository
            _departmentRepository = departmentRepository;
            activityLogRepository = new ActivityLogRepository();

            // Load dữ liệu ban đầu
            LoadDepartments();

            // Khởi tạo các lệnh
            SearchCommand = new RelayCommand(ExecuteSearch);
            AddCommand = new RelayCommand(ExecuteAdd);
            LoadCommand = new RelayCommand(ExecuteLoad);
            BackCommand = new RelayCommand(ExecuteBack);
            DeleteCommand = new RelayCommand(ExecuteDelete);
            UpdateCommand = new RelayCommand(ExecuteUpdate);
        }

        private void LoadDepartments()
        {
            try
            {
                // Lấy danh sách phòng ban từ repository
                var departments = _departmentRepository.getAllDepartment();

                // Gán danh sách vào ObservableCollection để hiển thị trên giao diện
                Departments = new ObservableCollection<Department>(departments);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteSearch(object parameter)
        {
            try
            {
                // Logic tìm kiếm phòng ban
               
                
                    // Lọc danh sách phòng ban theo tên từ repository
                    var filteredDepartments = _departmentRepository.searchByName(SearchText);

                    // Kiểm tra kết quả tìm kiếm
                    if (filteredDepartments == null || !filteredDepartments.Any())
                    {
                        // Hiển thị thông báo nếu không có dữ liệu
                        MessageBox.Show("Không có dữ liệu tương ứng!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        // Cập nhật danh sách hiển thị
                        Departments = new ObservableCollection<Department>(filteredDepartments);
                        
                    }
                
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                MessageBox.Show($"Lỗi khi tìm kiếm phòng ban: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteAdd(object parameter)
        {
            // Mở cửa sổ nhập tên phòng ban mới
            var addDepartmentWindow = new AddDepartmentWindow();
            bool? result = addDepartmentWindow.ShowDialog();

            // Kiểm tra kết quả từ cửa sổ
            if (result == true)
            {
                var viewModel = addDepartmentWindow.DataContext as AddDepartmentViewModel;
                if (viewModel != null && !string.IsNullOrWhiteSpace(viewModel.DepartmentName))
                {
                    // Tạo phòng ban mới
                    var newDepartment = new Department
                    {
                        DepartmentName = viewModel.DepartmentName,
                        Status = "Active"
                    };

                    // Thêm vào danh sách hiển thị
                    Departments.Add(newDepartment);

                    // TODO: Thêm logic lưu vào cơ sở dữ liệu
                    try
                    {
                        _departmentRepository.Add(newDepartment);
                        MessageBox.Show("Thêm phòng ban thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi thêm phòng ban: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void ExecuteDelete(object parameter)
        {
            // Logic xóa phòng ban (cập nhật trạng thái thành Inactive)
            if (parameter is Department selectedDepartment)
            {
                // Hiển thị hộp thoại xác nhận
                MessageBoxResult result = MessageBox.Show(
                    "Bạn có chắc chắn muốn vô hiệu hóa phòng ban này?",
                    "Xác nhận vô hiệu hóa",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning
                );

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        // Gọi phương thức Delete từ repository để cập nhật trạng thái
                        _departmentRepository.Delete(selectedDepartment.DepartmentId);

                        // Cập nhật trạng thái trong danh sách hiển thị
                        selectedDepartment.Status = "Inactive";

                        // Thông báo thành công
                        MessageBox.Show("Phòng ban đã được vô hiệu hóa thành công.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        activityLogRepository.LogActivity(1, "Vô hiệu hoá phòng ban", $"Vô hiệu phòng ban: {selectedDepartment.DepartmentName}");
                    }
                    catch (Exception ex)
                    {
                        // Xử lý lỗi
                        MessageBox.Show($"Lỗi khi vô hiệu hóa phòng ban: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private async void ExecuteUpdate(object parameter)
        {
            if (parameter is Department selectedDepartment)
            {
                // Mở cửa sổ chỉnh sửa tên phòng ban
                var editWindow = new EditDepartmentWindow(selectedDepartment, _departmentRepository);
                bool? result = editWindow.ShowDialog();

                if (result == true)
                {
                    // Lấy ViewModel từ DataContext của cửa sổ
                    var viewModel = editWindow.DataContext as EditDepartmentViewModel;

                    // Kiểm tra xem ViewModel và UpdatedDepartment có tồn tại không
                    if (viewModel != null && viewModel.UpdatedDepartment != null)
                    {
                        var updatedDepartment = viewModel.UpdatedDepartment;

                        // Cập nhật tên phòng ban trong danh sách
                        var departmentToUpdate = Departments.FirstOrDefault(d => d.DepartmentId == updatedDepartment.DepartmentId);
                        if (departmentToUpdate != null)
                        {
                            departmentToUpdate.DepartmentName = updatedDepartment.DepartmentName;

                            // Cập nhật vào cơ sở dữ liệu
                            _departmentRepository.Update(departmentToUpdate);
                        }
                    }
                }
            }
        }

        private void ExecuteLoad(object parameter)
        {
            // Logic tải lại danh sách phòng ban
            LoadDepartments();
        }

        private void ExecuteBack(object parameter)
        {
            // Logic quay lại cửa sổ chính
            var mainWindow = new MainWindow();
            mainWindow.Show();

            // Đóng cửa sổ hiện tại
            foreach (Window window in Application.Current.Windows)
            {
                if (window is DPManagement dpManagementWindow)
                {
                    dpManagementWindow.Close();
                    break;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}