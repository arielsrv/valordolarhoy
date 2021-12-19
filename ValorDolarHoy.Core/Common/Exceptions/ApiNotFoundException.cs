using System;
using System.Runtime.Serialization;

namespace ValorDolarHoy.Core.Common.Exceptions;

[Serializable]
public class ApiNotFoundException : Exception
{
    public ApiNotFoundException()
    {
    }

    protected ApiNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public ApiNotFoundException(string? message) : base(message)
    {
    }

    public ApiNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}