using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionMaxThreadsPerSCTExceeded : MapiExceptionRpcServerTooBusy
	{
		internal MapiExceptionMaxThreadsPerSCTExceeded(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionMaxThreadsPerSCTExceeded", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionMaxThreadsPerSCTExceeded(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
