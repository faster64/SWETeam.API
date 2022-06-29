using System;
using System.Collections.Generic;
using System.Text;

namespace SWETeam.Common.Mail
{
    public class MailConstant
    {
        #region Errors
        public const string MAIL_PARAM_NULL = "MailParameter cannot be null";
        public const string TO_MAIL_EMPTY_ERROR = "Địa chỉ mail của người nhận không được để trống";
        public const string TO_MAIL_INVALID_ERROR = "Địa chỉ email không hợp lệ";
        public const string SUBJECT_EMPTY_ERROR = "Subject không được để trống";
        public const string BODY_EMPTY_ERROR = "Nội dung mail không được để trống";
        #endregion
    }
}
