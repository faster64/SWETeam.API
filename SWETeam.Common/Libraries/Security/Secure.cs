using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SWETeam.Common.Libraries
{
    public static class Secure
    {

        /// <summary>
        /// CreatedBy: nnanh4 (01/05/2022)
        /// Message trả về khi detected sql injection
        /// </summary>
        public static string msgDetectedSqlInjection = "Sql injection detected. Please re-check your parameters.";

        /// <summary>
        /// Detect sql injection
        /// CreatedBy: nnanh4 (01/05/2022)
        /// </summary>
        /// <param name="input">input cần check sql injection</param>
        /// <returns>true nếu có sql injection - otherwise false</returns>
        public static bool DetectSqlInjection(string input)
        {
            Regex reg = new Regex(@"\s?or\s*|\s?;\s?|\s?drop\s|\s?grant\s|^'|\s?--|/s?union\s|\s?delete\s|\s?truncate\s|\s?sysobjects\s?|\s?xp_.*?|\s?syslogins\s?|/s?sysremote\s?|\s?sysusers\s?|\s?sysxlogins\s?|\s?sysdatabases\s?|\s?aspnet_.*?|\s?exec\s?",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

            return !string.IsNullOrWhiteSpace(input) && reg.IsMatch(input);
        }

    }
}
