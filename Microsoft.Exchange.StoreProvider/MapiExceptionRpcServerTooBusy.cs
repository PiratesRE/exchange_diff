using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class MapiExceptionRpcServerTooBusy : MapiRetryableException
	{
		internal MapiExceptionRpcServerTooBusy(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionRpcServerTooBusy", message, hr, ec, context, innerException)
		{
		}

		protected MapiExceptionRpcServerTooBusy(string className, string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base(className, message, hr, ec, context, innerException)
		{
		}

		protected MapiExceptionRpcServerTooBusy(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
