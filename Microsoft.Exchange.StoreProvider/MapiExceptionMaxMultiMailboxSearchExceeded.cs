using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionMaxMultiMailboxSearchExceeded : MapiRetryableException
	{
		internal MapiExceptionMaxMultiMailboxSearchExceeded(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionMaxMultiMailboxSearchExceeded", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionMaxMultiMailboxSearchExceeded(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
