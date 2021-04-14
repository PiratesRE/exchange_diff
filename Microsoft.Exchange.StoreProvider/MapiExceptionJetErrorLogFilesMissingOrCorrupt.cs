using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionJetErrorLogFilesMissingOrCorrupt : MapiPermanentException
	{
		internal MapiExceptionJetErrorLogFilesMissingOrCorrupt(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionJetErrorRequiredLogFilesMissing", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionJetErrorLogFilesMissingOrCorrupt(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
