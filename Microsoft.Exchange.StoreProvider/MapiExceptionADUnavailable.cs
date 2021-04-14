using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionADUnavailable : MapiRetryableException
	{
		internal MapiExceptionADUnavailable(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionADUnavailable", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionADUnavailable(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
