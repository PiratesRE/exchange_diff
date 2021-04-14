using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionJetErrorTooManyOpenTablesAndCleanupTimedOut : MapiRetryableException
	{
		internal MapiExceptionJetErrorTooManyOpenTablesAndCleanupTimedOut(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionJetErrorTooManyOpenTablesAndCleanupTimedOut", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionJetErrorTooManyOpenTablesAndCleanupTimedOut(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
