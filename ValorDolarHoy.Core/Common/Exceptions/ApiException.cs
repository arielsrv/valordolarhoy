using System;

#pragma warning disable CS1591

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