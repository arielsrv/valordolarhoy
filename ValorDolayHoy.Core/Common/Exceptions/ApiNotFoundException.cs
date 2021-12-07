using System;

namespace ValorDolarHoy.Common.Exceptions
{
    public class ApiNotFoundException : Exception
    {
        public ApiNotFoundException(string message) : base(message)
        {
        }
    }
}