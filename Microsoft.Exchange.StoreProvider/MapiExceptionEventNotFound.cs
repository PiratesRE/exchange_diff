using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionEventNotFound : MapiRetryableException
	{
		internal MapiExceptionEventNotFound(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionEventNotFound", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionEventNotFound(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
