using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionMountInProgress : MapiRetryableException
	{
		internal MapiExceptionMountInProgress(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionMountInProgress", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionMountInProgress(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
