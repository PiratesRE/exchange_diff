using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionVersionStoreBusy : MapiRetryableException
	{
		internal MapiExceptionVersionStoreBusy(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionVersionStoreBusy", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionVersionStoreBusy(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
