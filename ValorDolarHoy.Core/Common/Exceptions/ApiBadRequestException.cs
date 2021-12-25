using System;

namespace ValorDolarHoy.Core.Common.Exceptions;

public class ApiBadRequestException : Exception
{
    public ApiBadRequestException(string message) : base(message)
    {
    }
}