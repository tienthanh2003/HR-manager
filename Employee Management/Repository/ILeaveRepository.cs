using Employee_Management.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employee_Management.Repository
{
    public interface ILeaveRepository
    {
        public List<LeaveViewModel> GetAllLeaves();

        public void UpdateLeave(LeaveViewModel leave);

        public void AddLeaveRequest(Leaves leave);
    }
}
