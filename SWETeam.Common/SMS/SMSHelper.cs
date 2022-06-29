using System;
using System.Collections.Generic;
using System.Text;

namespace SWETeam.Common.SMS
{
    public static class SMSHelper
    {
        private static IServiceProvider _provider;

        /// <summary>
        /// Inject provider
        /// </summary>
        public static void InjectProvider(IServiceProvider provider)
        {
            _provider = provider;
        }
    }
}
