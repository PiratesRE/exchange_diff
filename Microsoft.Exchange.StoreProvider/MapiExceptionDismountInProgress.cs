using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionDismountInProgress : MapiRetryableException
	{
		internal MapiExceptionDismountInProgress(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionDismountInProgress", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionDismountInProgress(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
