using SWETeam.Common.Entities;
using System;

namespace SWETeam.Common.Auth
{
    public class RoleUser : BaseModel
    {
        public long Id { get; set; }

        public int RoleId { get; set; }

        public Guid UserId { get; set; }
    }
}
