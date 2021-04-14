using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionRpcOutOfResources : MapiRetryableException
	{
		internal MapiExceptionRpcOutOfResources(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionRpcOutOfResources", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionRpcOutOfResources(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
