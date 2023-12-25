using System;

#pragma warning disable CS1591

namespace ValorDolarHoy.Core.Common.Exceptions;

public class ApiNotFoundException : Exception
{
    /// <inheritdoc />
    public ApiNotFoundException(string message) : base(message)
    {
    }
}