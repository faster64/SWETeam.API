using System;
using System.Collections.Generic;
using System.Text;
using static SWETeam.Common.Mail.MailEnumeration;

namespace SWETeam.Common.Mail
{
    public class ValidateMailResult
    {
        public bool IsValid
        {
            get
            {
                return ErrorCode == MailErrorCode.None;
            }
        }

        public string FieldError { get; set; }

        public MailErrorCode ErrorCode { get; set; } = MailErrorCode.None;

        public string ErrorMessage { get; set; }
    }
}
