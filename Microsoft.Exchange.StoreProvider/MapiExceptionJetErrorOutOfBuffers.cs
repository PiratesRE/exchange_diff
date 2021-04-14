using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionJetErrorOutOfBuffers : MapiRetryableException
	{
		internal MapiExceptionJetErrorOutOfBuffers(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionJetErrorOutOfBuffers", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionJetErrorOutOfBuffers(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
