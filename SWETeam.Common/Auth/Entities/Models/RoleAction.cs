using SWETeam.Common.Entities;

namespace SWETeam.Common.Auth
{
    public class RoleAction : BaseModel
    {
        public long Id { get; set; }

        public int RoleId { get; set; }

        public long ActionId { get; set; }
    }
}
