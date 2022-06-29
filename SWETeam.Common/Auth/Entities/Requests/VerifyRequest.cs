using SWETeam.Common.Libraries;
using System;

namespace SWETeam.Common.Auth
{
    public class VerifyRequest
    {
        public Guid UserId { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string OTP { get; set; }
    }
}
