using SWETeam.Common.Entities;
using SWETeam.Common.Logging;
using System;
using System.Collections.Generic;
using System.Net;

namespace SWETeam.Common.Auth
{
    public class AuthResult : BaseResponse
    {

        /// <summary>
        /// Access token
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Refresh Token
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// OTP
        /// </summary>
        public string OTP { get; set; }


        /// <summary>
        /// Thông tin validate
        /// </summary>
        public List<ValidateField> ValidateInfo { get; set; } = new List<ValidateField>();

        /// <summary>
        /// Hàm set lỗi khi có exception
        /// </summary>
        public void SetError(Exception ex, string moreInfo = "")
        {
            Success = false;
            ErrorMessage = ex.Message;

            if (true || !Constant.IsDevelopmentENV)
            {
                CommonLog.LogError(ex, moreInfo);
            }
        }
    }
}
