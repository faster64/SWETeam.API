using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWETeam.Common.Excel
{
    public class ExcelService : IExcelService
    {
        private readonly IServiceProvider _provider;
        private readonly IConfiguration _config;

        public ExcelService(IServiceProvider provider)
        {
            _provider = provider;
            _config = provider.GetRequiredService<IConfiguration>();
        }
    }
}
