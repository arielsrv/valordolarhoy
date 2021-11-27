using System;
using System.Globalization;

namespace ValorDolarHoy.Common.Exceptions
{
    public class ApiBadRequestException : Exception
    {
        public ApiBadRequestException()
        {
        }

        public ApiBadRequestException(string message) : base(message)
        {
        }

        public ApiBadRequestException(string message, params object[] args)
            : base(string.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}