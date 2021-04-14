using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionServerPaused : MapiRetryableException
	{
		internal MapiExceptionServerPaused(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionServerPaused", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionServerPaused(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
