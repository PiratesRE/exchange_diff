using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionMultiMailboxSearchFailed : MapiRetryableException
	{
		internal MapiExceptionMultiMailboxSearchFailed(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionMultiMailboxSearchFailed", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionMultiMailboxSearchFailed(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
