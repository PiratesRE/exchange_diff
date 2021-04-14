using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionStreamSizeError : MapiPermanentException
	{
		internal MapiExceptionStreamSizeError(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionStreamSizeError", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionStreamSizeError(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
