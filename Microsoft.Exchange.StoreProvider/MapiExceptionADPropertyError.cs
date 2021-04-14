using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionADPropertyError : MapiRetryableException
	{
		internal MapiExceptionADPropertyError(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionADPropertyError", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionADPropertyError(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
