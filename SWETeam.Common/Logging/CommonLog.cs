using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SWETeam.Common.Logging
{
    public static class CommonLog
    {
        private static IServiceProvider _provider;
        private static string projectName = "OApi";
        private static string basePath = $"D:";
        private static int currentErrorLogNo = 1;

        /// <summary>
        /// Log error
        /// </summary>
        /// <param name="moreInfo"></param>
        public static void LogError(Exception ex, string moreInfo = "")
        {
            string fileName = $"LOG-ERROR-{DateTime.Now.ToString("dd-MM-yyyy")}.txt";
            string filePath = $"{basePath}/{fileName}";

            using (StreamWriter w = File.AppendText(filePath))
            {
                w.WriteLine("================================================================================");
                w.Write("------- Time: ");
                w.WriteLine(DateTime.Now.ToString("hh:mm:ss dd/MM/yyyy"));

                w.WriteLine("------- StackTrace:");
                w.WriteLine(ex.StackTrace);

                w.WriteLine("------- Exception:");
                w.WriteLine(ex.Message);

                if (!string.IsNullOrWhiteSpace(moreInfo))
                {
                    w.Write("------- More info: ");
                    w.WriteLine(moreInfo);
                }
                w.WriteLine($"TỔNG SỐ LOG HIỆN TẠI: {currentErrorLogNo++}");
                w.WriteLine("================================================================================");
                w.WriteLine();
            }
        }

        /// <summary>
        /// Log info
        /// </summary>
        /// <param name="moreInfo"></param>
        public static void LogInfor(string info)
        {
            string fileName = $"LOG-INFO-{DateTime.Now.ToString("dd-MM-yyyy")}.txt";
            string filePath = $"{basePath}/{fileName}";

            using (StreamWriter w = File.AppendText(filePath))
            {
                w.WriteLine("=========================================================================");
                w.WriteLine(info);
                w.WriteLine("==========================================================================");
                w.WriteLine();
            }
        }

        /// <summary>
        /// Log vào database
        /// </summary>
        public static void WriteToDatabase(Exception ex, string moreInfo = "")
        {
        }

        /// <summary>
        /// Inject provider
        /// </summary>
        public static void InjectProvider(IServiceProvider provider)
        {
            _provider = provider;
        }
    }
}
