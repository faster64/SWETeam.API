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
                if (!Constant.IsDevelopmentENV)
                    return Constant.HAS_ERROR_MESSAGE;
                return _errorMessage;
            }
            set
            {
                _errorMessage = value;
            }
        }

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
    }
}
