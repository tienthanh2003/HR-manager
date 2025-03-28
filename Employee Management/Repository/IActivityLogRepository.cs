using Employee_Management.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employee_Management.Repository
{
    public interface IActivityLogRepository
    {
        void LogActivity(int userId, string action, string details);
        public List<ActivityViewModel> GetActivityByUserId(int userId);

        public List<ActivityViewModel> GetAllActivityLogs();



    }
}
