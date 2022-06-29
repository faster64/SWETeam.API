﻿using SWETeam.Common.Entities;
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
            this.Success = false;
            if (_code == HttpStatusCode.OK)
            {
                _code = HttpStatusCode.InternalServerError;
            }

            // Nếu là môi trường dev thì cho phép show error
            if (Constant.IsDevelopmentENV)
            {
                ErrorMessage = ex.Message;
            }
            else
            {
                ErrorMessage = Constant.HAS_ERROR_MESSAGE;
                CommonLog.LogError(ex, moreInfo);
            }
        }
    }
}
