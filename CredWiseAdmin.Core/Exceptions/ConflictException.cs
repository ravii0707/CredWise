using System;
using System.Runtime.Serialization;

namespace CredWiseAdmin.Core.Exceptions
{
    [Serializable]
    public class ConflictException : Exception
    {
        public ConflictException() { }
        public ConflictException(string message) : base(message) { }
        public ConflictException(string message, Exception inner) : base(message, inner) { }
        protected ConflictException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }



    [Serializable]
    public class ValidationException : Exception
    {
        public ValidationException() { }
        public ValidationException(string message) : base(message) { }
        public ValidationException(string message, Exception inner) : base(message, inner) { }
        protected ValidationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
