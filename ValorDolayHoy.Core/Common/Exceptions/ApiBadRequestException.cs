using System;

namespace ValorDolarHoy.Common.Exceptions
{
    public class ApiBadRequestException : Exception
    {
        public ApiBadRequestException(string message) : base(message)
        {
        }
    }
}