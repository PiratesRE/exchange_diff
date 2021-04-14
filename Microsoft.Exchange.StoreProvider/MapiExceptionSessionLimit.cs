using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionSessionLimit : MapiRetryableException
	{
		internal MapiExceptionSessionLimit(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionSessionLimit", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionSessionLimit(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
