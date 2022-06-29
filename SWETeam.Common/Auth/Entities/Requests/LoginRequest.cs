using SWETeam.Common.Libraries;

namespace SWETeam.Common.Auth
{
    public class LoginRequest
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        public string OTP { get; set; }
    }
}
