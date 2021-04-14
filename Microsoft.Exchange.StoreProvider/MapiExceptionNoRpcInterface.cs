using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionNoRpcInterface : MapiRetryableException
	{
		internal MapiExceptionNoRpcInterface(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionNoRpcInterface", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionNoRpcInterface(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
