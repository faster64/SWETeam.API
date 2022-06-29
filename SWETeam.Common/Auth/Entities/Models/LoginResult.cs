namespace SWETeam.Common.Auth
{
    internal class LoginResult : User
    {
        public int RoleId { get; set; }

        public string RoleName { get; set; }
    }
}
