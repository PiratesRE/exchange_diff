using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionCancel : MapiRetryableException
	{
		internal MapiExceptionCancel(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionCancel", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionCancel(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
