using SWETeam.Common.Entities;

namespace SWETeam.Common.Auth
{
    public class Role : BaseModel
    {
        public int Id { get; set; }

        public string RoleName { get; set; }
    }
}
