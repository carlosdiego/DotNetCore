using System;

namespace Shared.Exceptions
{
    public class CustomException : Exception
    {
        public CustomException(string message)
            : base(message)
        {

        }

        public CustomException(string message, Exception ex)
            : base(message, ex)
        {

        }
    }
}
