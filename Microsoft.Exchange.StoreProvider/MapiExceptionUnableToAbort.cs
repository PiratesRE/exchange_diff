using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionUnableToAbort : MapiRetryableException
	{
		internal MapiExceptionUnableToAbort(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionUnableToAbort", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionUnableToAbort(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
