using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionJetErrorAttachedDatabaseMismatch : MapiPermanentException
	{
		internal MapiExceptionJetErrorAttachedDatabaseMismatch(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionJetErrorAttachedDatabaseMismatch", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionJetErrorAttachedDatabaseMismatch(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
