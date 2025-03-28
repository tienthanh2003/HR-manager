using Employee_Management.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employee_Management.Repository
{
    public interface IAttendRepository
    {
        public void GenerateDailyAttendance();

        public Attendance GetTodayAttendance(int employeeId, DateTime date);
        public void AddAttendance(Attendance attendance);
        public void UpdateAttendance(Attendance attendance);

        public List<AttendanceViewModel> GetAttendancesWithEmployeeName();

        public decimal CalculateTotalOvertime(int employeeId, DateTime startDate, DateTime endDate);
        public int CountAbsentDays(int employeeId, DateTime startDate, DateTime endDate);
        public int CountAttendDays(int employeeId, DateTime startDate, DateTime endDate);
    }
}
