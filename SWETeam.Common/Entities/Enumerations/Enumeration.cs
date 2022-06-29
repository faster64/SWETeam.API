using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace SWETeam.Common.Entities
{
    public static class Enumeration
    {
        public enum HttpStatusCodeExtension : int
        {
            /// <summary>
            /// Tài khoản chưa xác minh
            /// </summary>
            NotVerified = 801,
        }

        /// <summary>
        /// Entity state
        /// </summary>
        public enum EntityState
        {
            Add = 1,

            Edit = 2,

            Delete = 3
        }


        /// <summary>
        /// Validate code
        /// </summary>
        public enum ValidateCode : int
        {
            Required = 1,

            Duplicate = 2,

            Invalid = 3,
        }

    }
}
