using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWETeam.Common.Excel
{
    public class ExcelHandler : IExcelHandler
    {
        private readonly IServiceProvider _provider;
        private readonly IConfiguration _config;

        public ExcelHandler(IServiceProvider provider)
        {
            _provider = provider;
            _config = provider.GetRequiredService<IConfiguration>();
        }
    }
}
