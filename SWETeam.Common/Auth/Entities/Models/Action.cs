using SWETeam.Common.Entities;

namespace SWETeam.Common.Auth
{
    public class Action : BaseModel
    {
        public long Id { get; set; }

        public string ActionName { get; set; }
    }
}
