using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UAParser;

namespace SWETeam.Common.Device
{
    public class DeviceService : IDeviceService
    {
        #region Declare
        private readonly IServiceProvider _provider;
        private readonly IHttpContextAccessor _accessor;
        #endregion

        #region Constructor
        public DeviceService(IServiceProvider provider)
        {
            _provider = provider;
            _accessor = _provider.GetRequiredService<IHttpContextAccessor>();
        }
        #endregion

        #region Implementation
        /// <summary>
        /// Lấy thông tin từ request
        /// </summary>
        public object GetDeviceInformation()
        {
            try
            {
                IHeaderDictionary headers = _accessor.HttpContext.Request.Headers;
                string ip = headers["X-Forwarded-For"];
                var osDescription = RuntimeInformation.OSDescription;
                var clientInfo = Parser.GetDefault().Parse(headers[HeaderNames.UserAgent].ToString());

                Connection result = new Connection()
                {
                    IP = ip,
                    OS = clientInfo.OS.Family,
                    Browser = clientInfo.UA.Family,
                    Device = clientInfo.Device.Family,
                    OSDescription = osDescription,
                    More = new
                    {
                        OS = clientInfo.OS,
                        Browser = clientInfo.UA,
                        Device = clientInfo.Device,
                        ServerTime = DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy")
                    },
                };

                return JsonConvert.SerializeObject(result, Formatting.Indented);
            }
            catch (Exception)
            {
                return new { ErrorMessage = "Có lỗi xảy ra khi lấy thông tin thiết bị" };
            }
        }

        /// <summary>
        /// Lấy thông tin từ request async
        /// </summary>
        public Task<object> GetDeviceInformationAsync()
        {
            Task<object> task = new Task<object>(() => GetDeviceInformation());
            task.Start();

            return task;
        }
        #endregion
    }
}
