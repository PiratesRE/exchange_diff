using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionRpcBufferTooSmall : MapiPermanentException
	{
		internal MapiExceptionRpcBufferTooSmall(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionRpcBufferTooSmall", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionRpcBufferTooSmall(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
