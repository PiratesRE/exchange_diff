using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionWait : MapiRetryableException
	{
		internal MapiExceptionWait(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionWait", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionWait(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
