using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace SWETeam.Common.Auth
{
    public class AuthEnumeration
    {
        /// <summary>
        /// Loại OTP
        /// </summary>
        public enum OtpType
        {
            [Description("None")]
            None = 0,

            [Description("For password")]
            Password = 1,

            [Description("For verify")]
            Verify = 2,

            [Description("Two factor authentication")]
            TFA = 3,
        }

        public enum TwoFactorType
        {
            [Description("None")]
            None = 0,

            [Description("Sử dụng email")]
            Email = 1,

            [Description("Sử dụng số điện thoại")]
            Phone = 2,
        }
    }
}
