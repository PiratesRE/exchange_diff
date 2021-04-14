using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionMaxThreadsPerMdbExceeded : MapiExceptionRpcServerTooBusy
	{
		internal MapiExceptionMaxThreadsPerMdbExceeded(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionMaxThreadsPerMdbExceeded", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionMaxThreadsPerMdbExceeded(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
