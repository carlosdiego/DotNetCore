using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Exceptions
{
    public class CustomValidationFailedException : Exception
    {
        public CustomValidationFailedException(Dictionary<string, IEnumerable<string>> errors)
            : base("Invalid data")
            => Errors = errors;

        public Dictionary<string, IEnumerable<string>> Errors { get; }
    }
}
