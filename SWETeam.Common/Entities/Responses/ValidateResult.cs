using System;
using System.Collections.Generic;
using System.Text;

namespace SWETeam.Common.Entities
{
    /// <summary>
    /// Kết quả sau khi validate
    /// </summary>
    public class ValidateResult
    {
        /// <summary>
        /// Có valid hay không?
        /// </summary>
        public bool IsValid
        {
            get { return ValidateFields == null || ValidateFields.Count == 0; }
        }

        /// <summary>
        /// List validate field
        /// </summary>
        public List<ValidateField> ValidateFields { get; set; } = new List<ValidateField>();
    }
}
