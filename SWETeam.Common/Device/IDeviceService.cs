using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SWETeam.Common.Device
{
    public interface IDeviceService
    {
        /// <summary>
        /// Lấy thông tin từ request
        /// </summary>
        object GetDeviceInformation();

        /// <summary>
        /// Lấy thông tin từ request async
        /// </summary>
        Task<object> GetDeviceInformationAsync();
    }
}
