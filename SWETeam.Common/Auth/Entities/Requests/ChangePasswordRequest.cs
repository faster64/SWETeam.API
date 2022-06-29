using SWETeam.Common.Libraries;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWETeam.Common.Auth
{
    public class ChangePasswordRequest
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string OldPassword { get; set; }

        [Required]
        [Password]
        public string NewPassword { get; set; }

        [Required]
        [Password]
        public string ConfirmNewPassword { get; set; }

        public bool IsUseOTP { get; set; }

        public string OTP { get; set; }
    }
}
