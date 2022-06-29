using SWETeam.Common.Entities;

namespace SWETeam.Common.Validations
{
    public interface IValidator
    {
        /// <summary>
        /// Thực hiện validate
        /// </summary>
        ValidateResult Execute(object obj, string tableName = "");
    }
}
