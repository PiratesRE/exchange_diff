using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionRpcAuthentication : MapiRetryableException
	{
		internal MapiExceptionRpcAuthentication(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionRpcAuthentication", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionRpcAuthentication(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
