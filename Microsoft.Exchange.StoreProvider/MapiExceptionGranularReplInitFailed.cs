using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionGranularReplInitFailed : MapiRetryableException
	{
		internal MapiExceptionGranularReplInitFailed(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionGranularReplInitFailed", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionGranularReplInitFailed(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
