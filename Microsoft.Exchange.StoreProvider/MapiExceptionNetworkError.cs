using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionNetworkError : MapiRetryableException
	{
		internal MapiExceptionNetworkError(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionNetworkError", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionNetworkError(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
