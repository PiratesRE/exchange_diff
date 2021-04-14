using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionJetErrorTableLocked : MapiRetryableException
	{
		internal MapiExceptionJetErrorTableLocked(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionJetErrorTableLocked", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionJetErrorTableLocked(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
