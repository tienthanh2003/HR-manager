using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employee_Management.Models
{
    public class ActivityViewModel
    {
        public int LogId { get; set; } // Khóa chính
        public int UserId { get; set; }

        public string Username { get; set; } // ID của người dùng	        public string Username { get; set; } // Tên người dùng
        public string Action { get; set; } // Hành động (ví dụ: "Đăng nhập")
        public string Details { get; set; } // Chi tiết hành động
        public DateTime ActionDate { get; set; } // Thời gian thực hiện
        public DateTime Date { get; set; } // Ngày của hoạt động

    }
}
