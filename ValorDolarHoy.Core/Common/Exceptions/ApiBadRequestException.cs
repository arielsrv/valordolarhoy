using System;
using System.Runtime.Serialization;

namespace ValorDolarHoy.Core.Common.Exceptions;

[Serializable]
public class ApiBadRequestException : Exception
{
    public ApiBadRequestException()
    {
    }

    protected ApiBadRequestException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public ApiBadRequestException(string? message) : base(message)
    {
    }

    public ApiBadRequestException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}