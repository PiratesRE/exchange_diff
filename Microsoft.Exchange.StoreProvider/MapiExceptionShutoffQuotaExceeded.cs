using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionShutoffQuotaExceeded : MapiPermanentException
	{
		internal MapiExceptionShutoffQuotaExceeded(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionShutoffQuotaExceeded", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionShutoffQuotaExceeded(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
