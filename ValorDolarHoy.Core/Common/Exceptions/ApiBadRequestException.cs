using System;

#pragma warning disable CS1591

namespace ValorDolarHoy.Core.Common.Exceptions;

public class ApiBadRequestException : Exception
{
    public ApiBadRequestException(string message) : base(message)
    {
    }
}