using System;

namespace JurassicCoffee.Core
{
    public class CompilerException : Exception
    {
        public CompilerException(string message)
            : base(message)
        {}

        public CompilerException(string message, Exception innerException) : base(message, innerException)
        {}
    }
}
