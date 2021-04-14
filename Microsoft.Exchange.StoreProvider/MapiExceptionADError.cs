using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionADError : MapiRetryableException
	{
		internal MapiExceptionADError(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionADError", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionADError(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
