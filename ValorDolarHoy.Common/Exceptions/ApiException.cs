using System;

namespace ValorDolarHoy.Common.Exceptions
{
    public class ApiException : Exception
    {
        public ApiException(string message) : base(message)
        {
        }
    }
}