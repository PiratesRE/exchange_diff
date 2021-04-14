using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionJetErrorDiskIO : MapiRetryableException
	{
		internal MapiExceptionJetErrorDiskIO(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionJetErrorDiskIO", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionJetErrorDiskIO(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
