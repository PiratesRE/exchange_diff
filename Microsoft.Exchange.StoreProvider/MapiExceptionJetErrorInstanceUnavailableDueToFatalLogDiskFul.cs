using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionJetErrorInstanceUnavailableDueToFatalLogDiskFull : MapiPermanentException
	{
		internal MapiExceptionJetErrorInstanceUnavailableDueToFatalLogDiskFull(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionJetErrorInstanceUnavailableDueToFatalLogDiskFull", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionJetErrorInstanceUnavailableDueToFatalLogDiskFull(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
