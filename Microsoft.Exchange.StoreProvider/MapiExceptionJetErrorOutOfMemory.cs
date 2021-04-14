using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionJetErrorOutOfMemory : MapiRetryableException
	{
		internal MapiExceptionJetErrorOutOfMemory(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionJetErrorOutOfMemory", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionJetErrorOutOfMemory(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
