using SWETeam.Common.Entities;
using System;
using static SWETeam.Common.Auth.AuthEnumeration;

namespace SWETeam.Common.Auth
{
    /// <summary>
    /// Lớp user đăng nhập 2 lớp
    /// </summary>
    public class UserTwoFactor : BaseModel
    {
        public Guid UserId { get; set; }

        /// <summary>
        /// Kiểu xác thực
        /// </summary>
        public TwoFactorType TwoFactorType { get; set; } = TwoFactorType.None;

        /// <summary>
        /// True nếu đang bật 2FA, otherwise False
        /// </summary>
        public bool TwoFactorEnabled { get; set; }
    }
}
