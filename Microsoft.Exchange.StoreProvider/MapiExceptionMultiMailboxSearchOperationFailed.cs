using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionMultiMailboxSearchOperationFailed : MapiRetryableException
	{
		internal MapiExceptionMultiMailboxSearchOperationFailed(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionMultiMailboxSearchOperationFailed", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionMultiMailboxSearchOperationFailed(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
