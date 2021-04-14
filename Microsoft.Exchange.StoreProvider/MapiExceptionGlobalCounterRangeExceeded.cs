using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionGlobalCounterRangeExceeded : MapiRetryableException
	{
		internal MapiExceptionGlobalCounterRangeExceeded(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionGlobalCounterRangeExceeded", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionGlobalCounterRangeExceeded(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
