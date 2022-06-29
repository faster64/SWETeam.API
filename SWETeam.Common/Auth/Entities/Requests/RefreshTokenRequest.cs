using SWETeam.Common.Entities;
using SWETeam.Common.Libraries;
using System;

namespace SWETeam.Common.Auth
{
    public class RefreshTokenRequest : BaseModel
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public string RefreshToken { get; set; }

        public DateTime ExpriedTime { get; set; }
    }
}
