using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionLogonFailed : MapiRetryableException
	{
		internal MapiExceptionLogonFailed(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionLogonFailed", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionLogonFailed(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
