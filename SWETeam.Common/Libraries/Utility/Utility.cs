using System;
using System.Collections.Generic;
using System.Text;

namespace SWETeam.Common.Libraries
{
    public static class Utility
    {
        #region Functions

        public static string RandomNumber(int length)
        {
            StringBuilder sb = new StringBuilder();
            Random random = new Random();

            while (sb.Length < length)
            {
                int num = random.Next(0, 9);
                sb.Append(num + "");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Tạo chuỗi random
        /// </summary>
        public static string MixRandom(int length)
        {
            StringBuilder sb = new StringBuilder();
            var random = new Random();
            string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            while (sb.Length < length)
            {
                sb.Append(chars[random.Next(chars.Length)]);
            }

            return sb.ToString(); ;
        }
        #endregion
    }
}
