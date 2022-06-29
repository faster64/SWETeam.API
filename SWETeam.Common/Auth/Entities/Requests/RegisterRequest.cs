using SWETeam.Common.Auth;

namespace SWETeam.Common.Auth
{
    public class RegisterRequest : User
    {
        public string ConfirmPassword { get; set; }
    }
}
