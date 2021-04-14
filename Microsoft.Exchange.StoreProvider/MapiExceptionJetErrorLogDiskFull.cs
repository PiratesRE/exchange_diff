using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionJetErrorLogDiskFull : MapiPermanentException
	{
		internal MapiExceptionJetErrorLogDiskFull(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionJetErrorLogDiskFull", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionJetErrorLogDiskFull(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
