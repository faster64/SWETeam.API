using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SWETeam.Common.Exceptions
{
    public class CaughtableException : Exception
    {
        public CaughtableException() : base()
        {
        }

        public CaughtableException(string message) : base(message)
        {
        }

        public CaughtableException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CaughtableException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
