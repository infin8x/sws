using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server.Protocol
{
    class ProtocolException : Exception
    {
        public int Status { get; private set; }
        public ProtocolException()
            : base("An error occurred while processing the HTTP protocol.")
        {
            Status = Constants.NotSupportedCode;
        }

        public ProtocolException(String message)
            : base(message)
        {
            Status = Constants.NotSupportedCode;
        }

        public ProtocolException(int status, String message)
            : base(message)
        {
            Status = status;
        }

        public ProtocolException(int status, String message, Exception innerException)
            : base(message, innerException)
        {
            Status = status;
        }

        public ProtocolException(Exception innerException)
            : base("An error occurred while processing the HTTP protocol.", innerException)
        {
            Status = Constants.NotSupportedCode;
        }
    }
}
