using System;
using System.Collections.Generic;
using System.Text;

namespace SWETeam.Common.Entities
{
    public static class Constant
    {
        #region Env

        public const bool IsDevelopmentENV = true;

        #endregion

        #region Messages

        /// <summary>
        /// Msg đã xảy ra lỗi
        /// </summary>
        public const string HAS_ERROR_MESSAGE = "Đã có lỗi xảy ra!";

        /// <summary>
        /// Msg param không được null
        /// </summary>
        public const string PARAMETER_CANNOT_BE_NULL = "Parameter {0} cannot be null";

        /// <summary>
        /// Msg param invalid
        /// </summary>
        public const string PARAMETER_IS_INVALID = "Parameter {0} is invalid";

        /// <summary>
        /// Msg báo transaction là bắt buộc
        /// </summary>
        public const string TRANSACTION_IS_REQUIRED = "Transaction is required";

        /// <summary>
        /// Msg not permission
        /// </summary>
        public const string NOT_PERMISSION_MESSAGE = "Not permission!";

        #endregion
    }

    public static class RegexPattern
    {
        public const string PHONE_PATTERN = @"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}";
        public const string ONLY_NUMBER_PATTERN = @"^[0-9]+$";
    }
}
