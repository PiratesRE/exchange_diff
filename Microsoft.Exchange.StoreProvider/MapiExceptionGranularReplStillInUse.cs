using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionGranularReplStillInUse : MapiRetryableException
	{
		internal MapiExceptionGranularReplStillInUse(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionGranularReplStillInUse", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionGranularReplStillInUse(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
