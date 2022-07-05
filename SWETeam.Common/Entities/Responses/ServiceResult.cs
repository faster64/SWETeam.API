using SWETeam.Common.Logging;
using System;
using System.Collections.Generic;
using System.Net;

namespace SWETeam.Common.Entities
{
    public class ServiceResult : BaseResponse
    {
        /// <summary>
        /// Data trả về cho client
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// Có quyền hay không
        /// </summary>
        public bool HasPermission { get; set; } = true;

        /// <summary>
        /// Tổng số bản ghi
        /// </summary>
        public long Total { get; set; }

        /// <summary>
        /// Thông tin validate
        /// </summary>
        public List<ValidateField> ValidateInfo { get; set; } = new List<ValidateField>();

        /// <summary>
        /// Hàm set lỗi khi có exception
        /// </summary>
        public override void SetError(Exception ex, string moreInfo = "")
        {
            Success = false;

            // Ghi đè error code nếu là unauthorized
            if (_code == HttpStatusCode.Unauthorized)
            {
                _code = HttpStatusCode.Unauthorized;
            }

            if (!Constant.IsDevelopmentENV)
            {
                CommonLog.LogError(ex, moreInfo);
            }
            else
            {
                ErrorMessage = ex.Message;
            }
        }

        /// <summary>
        /// Hàm xử lý khi không có quyền
        /// </summary>
        public void HandleAuthorization(string customMsg = "")
        {
            Success = false;
            HasPermission = false;

            if (!string.IsNullOrEmpty(customMsg))
            {
                ErrorMessage = customMsg;
            }
            else
            {
                ErrorMessage = Constant.NOT_PERMISSION_MESSAGE;
            }
        }
    }
}
