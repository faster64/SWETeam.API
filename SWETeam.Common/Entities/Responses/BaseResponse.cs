using SWETeam.Common.Exceptions;
using SWETeam.Common.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace SWETeam.Common.Entities
{
    public class BaseResponse
    {
        protected HttpStatusCode _code = HttpStatusCode.OK;
        protected bool _success = true;
        protected string _errorMessage;

        /// <summary>
        /// Http status code trả về
        /// </summary>
        public HttpStatusCode Code
        {
            get
            {
                return _code;
            }

            set
            {
                _code = value;
                if (!_code.ToString().StartsWith("2") && _success)
                {
                    _success = false;
                }
            }
        }

        /// <summary>
        /// Thành công hay không?
        /// </summary>
        public bool Success
        {
            get
            {
                return _success;
            }
            set
            {
                _success = value;
                if (_success == false && _code == HttpStatusCode.OK)
                {
                    _code = HttpStatusCode.InternalServerError;
                }
            }
        }

        /// <summary>
        /// Message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Error message
        /// </summary>
        public string ErrorMessage
        {
            get
            {
                //if (_success == false && !Constant.IsDevelopmentENV)
                //    return Constant.HAS_ERROR_MESSAGE;
                return _errorMessage;
            }
            set
            {
                _errorMessage = value;
            }
        }

        /// <summary>
        /// Có quyền hay không
        /// </summary>
        public bool HasPermission { get; set; } = true;

        /// <summary>
        /// Time hệ thống
        /// </summary>
        public DateTime ServerTime
        {
            get
            {
                return DateTime.Now;
            }
        }

        /// <summary>
        /// Hàm set lỗi khi có exception
        /// </summary>
        public virtual void SetError(Exception ex, string moreInfo = "")
        {
            Success = false;

            if (ex is CaughtableException)
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
