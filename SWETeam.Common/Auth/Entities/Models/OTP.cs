using System;
using static SWETeam.Common.Auth.AuthEnumeration;

namespace SWETeam.Common.Auth
{
    public class OTP
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string Otp { get; set; }

        public bool IsUsed { get; set; }

        public DateTime ExpriedDate { get; set; }

        public DateTime ProvidedDate { get; set; }

        public OtpType Type { get; set; } = OtpType.None;
    }
}
