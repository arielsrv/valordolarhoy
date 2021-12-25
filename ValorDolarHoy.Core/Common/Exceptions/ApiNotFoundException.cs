using System;

namespace ValorDolarHoy.Core.Common.Exceptions;

public class ApiNotFoundException : Exception
{
    public ApiNotFoundException(string message) : base(message)
    {
    }
}