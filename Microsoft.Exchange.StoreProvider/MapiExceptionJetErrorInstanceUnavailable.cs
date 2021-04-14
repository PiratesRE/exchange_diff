using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionJetErrorInstanceUnavailable : MapiRetryableException
	{
		internal MapiExceptionJetErrorInstanceUnavailable(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionJetErrorInstanceUnavailable", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionJetErrorInstanceUnavailable(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
