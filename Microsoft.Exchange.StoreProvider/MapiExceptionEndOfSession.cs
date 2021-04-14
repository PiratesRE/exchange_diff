using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionEndOfSession : MapiRetryableException
	{
		internal MapiExceptionEndOfSession(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionEndOfSession", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionEndOfSession(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
