using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionRpcOutOfMemory : MapiRetryableException
	{
		internal MapiExceptionRpcOutOfMemory(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionRpcOutOfMemory", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionRpcOutOfMemory(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
