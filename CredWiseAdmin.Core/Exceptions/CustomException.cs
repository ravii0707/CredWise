using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CredWiseAdmin.Core.Exceptions
{
    public abstract class CustomException : Exception
    {
        public int StatusCode { get; }
        public string ErrorType { get; }

        protected CustomException(string message, int statusCode, string errorType)
            : base(message)
        {
            StatusCode = statusCode;
            ErrorType = errorType;
        }
    }
}
