using System;
using System.Globalization;

namespace ValorDolarHoy.Common.Exceptions
{
    public class ApiNotFoundException : Exception
    {
        public ApiNotFoundException()
        {
        }

        public ApiNotFoundException(string message) : base(message)
        {
        }

        public ApiNotFoundException(string message, params object[] args)
            : base(string.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}