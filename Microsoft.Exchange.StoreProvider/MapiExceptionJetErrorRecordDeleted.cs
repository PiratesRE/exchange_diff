using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionJetErrorRecordDeleted : MapiRetryableException
	{
		internal MapiExceptionJetErrorRecordDeleted(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionJetErrorRecordDeleted", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionJetErrorRecordDeleted(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
