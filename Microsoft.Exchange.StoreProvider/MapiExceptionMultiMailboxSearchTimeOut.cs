using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionMultiMailboxSearchTimeOut : MapiRetryableException
	{
		internal MapiExceptionMultiMailboxSearchTimeOut(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionMultiMailboxSearchTimeOut", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionMultiMailboxSearchTimeOut(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
