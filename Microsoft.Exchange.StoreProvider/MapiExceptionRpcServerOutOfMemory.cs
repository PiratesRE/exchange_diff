using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionRpcServerOutOfMemory : MapiRetryableException
	{
		internal MapiExceptionRpcServerOutOfMemory(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionRpcServerOutOfMemory", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionRpcServerOutOfMemory(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
