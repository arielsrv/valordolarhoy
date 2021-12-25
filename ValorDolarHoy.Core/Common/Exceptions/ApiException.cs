using System;

namespace ValorDolarHoy.Core.Common.Exceptions;

public class ApiException : Exception
{
    public ApiException()
    {
    }

    public ApiException(string message) : base(message)
    {
    }
}