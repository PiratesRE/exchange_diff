using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionJetErrorWriteConflict : MapiRetryableException
	{
		internal MapiExceptionJetErrorWriteConflict(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionJetErrorWriteConflict", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionJetErrorWriteConflict(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
