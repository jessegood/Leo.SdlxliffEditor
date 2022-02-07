using System;
using System.Runtime.Serialization;

namespace Leo.SdlxliffEditor.Exceptions;

[Serializable]
internal class MissingTagException : Exception
{
    public MissingTagException()
    {
    }

    public MissingTagException(string message) : base(message)
    {
    }

    public MissingTagException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected MissingTagException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}