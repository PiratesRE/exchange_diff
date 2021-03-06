using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionJetErrorOutOfSessions : MapiRetryableException
	{
		internal MapiExceptionJetErrorOutOfSessions(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionJetErrorOutOfSessions", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionJetErrorOutOfSessions(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
