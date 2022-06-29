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
        public void SetError(Exception ex, string moreInfo = "")
        {
            Success = false;

            // Ghi đè error code nếu là unauthorized
            if (_code == HttpStatusCode.Unauthorized)
            {
                _code = HttpStatusCode.Unauthorized;
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

        /// <summary>
        /// Hàm xử lý khi không có quyền
        /// CreatedBy: nvcuong2 (02/05/2022)
        /// </summary>
        /// <param name="customMsg"></param>
        public void HandleAuthorization(string customMsg = "")
        {
            _success = false;
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
