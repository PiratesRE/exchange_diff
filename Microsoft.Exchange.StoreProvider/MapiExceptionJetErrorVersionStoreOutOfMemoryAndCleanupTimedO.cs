using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionJetErrorVersionStoreOutOfMemoryAndCleanupTimedOut : MapiRetryableException
	{
		internal MapiExceptionJetErrorVersionStoreOutOfMemoryAndCleanupTimedOut(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionJetErrorVersionStoreOutOfMemoryAndCleanupTimedOut", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionJetErrorVersionStoreOutOfMemoryAndCleanupTimedOut(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
