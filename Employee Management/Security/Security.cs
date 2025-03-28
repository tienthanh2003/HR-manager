using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Data_Access.Security
{
    public static class SecurityHelper
    {
        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // SQL Server HASHBYTES('SHA2_256', 'password') dùng UTF-16LE
                byte[] data = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
     
                return BitConverter.ToString(data).Replace("-", ""); ;
            }
        }
    }

}
