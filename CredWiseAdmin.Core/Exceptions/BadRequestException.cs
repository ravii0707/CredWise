using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CredWiseAdmin.Core.Exceptions
{
    public class BadRequestException : CustomException
    {
        public BadRequestException(string message)
            : base(message, 400, "Bad Request") { }
    }
}
