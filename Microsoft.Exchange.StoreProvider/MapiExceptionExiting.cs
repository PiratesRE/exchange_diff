using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionExiting : MapiRetryableException
	{
		internal MapiExceptionExiting(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionExiting", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionExiting(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
