using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;
using Newtonsoft.Json; // Thêm namespace này

namespace Employee_Management.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public string FullName { get; set; } = null!;

    public DateTime DateOfBirth { get; set; }

    public string Gender { get; set; } = null!;

    public string? Address { get; set; }

    public string? PhoneNumber { get; set; }

    public int? DepartmentId { get; set; }

    public int RemainingLeaveDays { get; set; }

    public string? Position { get; set; }

    public decimal? Salary { get; set; }

    public DateTime StartDate { get; set; }

    [JsonIgnore]
    public byte[]? Avatar { get; set; }

    [JsonIgnore] // Bỏ qua thuộc tính này khi serialize
    public BitmapImage AvatarImage
    {
        get
        {
            if (Avatar == null || Avatar.Length == 0)
                return null;

            var bitmapImage = new BitmapImage();
            using (var ms = new MemoryStream(Avatar))
            {
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = ms;
                bitmapImage.EndInit();
            }
            return bitmapImage;
        }
    }

    [JsonIgnore] // Bỏ qua thuộc tính này khi serialize
    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();

    [JsonIgnore] // Bỏ qua thuộc tính này khi serialize
    public virtual Department? Department { get; set; }

    [JsonIgnore] // Bỏ qua thuộc tính này khi serialize
    public virtual ICollection<Leaves> Leaves { get; set; } = new List<Leaves>();

    [JsonIgnore] // Bỏ qua thuộc tính này khi serialize
    public virtual ICollection<Salary> Salaries { get; set; } = new List<Salary>();

    [JsonIgnore] // Bỏ qua thuộc tính này khi serialize
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}