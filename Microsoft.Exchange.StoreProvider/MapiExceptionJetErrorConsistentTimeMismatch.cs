using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionJetErrorConsistentTimeMismatch : MapiPermanentException
	{
		internal MapiExceptionJetErrorConsistentTimeMismatch(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionJetErrorConsistentTimeMismatch", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionJetErrorConsistentTimeMismatch(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
