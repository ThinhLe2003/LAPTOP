using System.Security.Cryptography;
using System.Text;

namespace LAPTOP.Helpers
{
    public static class HashHelper
    {
        public static string ToMD5(string input)
        {
            using (var md5 = MD5.Create())
            {
                byte[] bytes=Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(bytes);

                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}
