using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SWETeam.Common.Exceptions
{
    public class PermissionException : CaughtableException
    {
        public PermissionException() : base()
        {
        }

        public PermissionException(string message) : base(message)
        {
        }

        public PermissionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PermissionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
