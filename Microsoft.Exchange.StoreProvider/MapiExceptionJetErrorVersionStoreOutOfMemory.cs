using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionJetErrorVersionStoreOutOfMemory : MapiRetryableException
	{
		internal MapiExceptionJetErrorVersionStoreOutOfMemory(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionJetErrorVersionStoreOutOfMemory", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionJetErrorVersionStoreOutOfMemory(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
