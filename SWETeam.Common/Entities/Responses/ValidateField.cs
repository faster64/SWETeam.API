using System;
using System.Collections.Generic;
using System.Text;
using static SWETeam.Common.Entities.Enumeration;

namespace SWETeam.Common.Entities
{
    public class ValidateField
    {
        /// <summary>
        /// Field bị lỗi
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Mã lỗi
        /// </summary>
        public ValidateCode Code { get; set; }

        /// <summary>
        /// Message lỗi
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
