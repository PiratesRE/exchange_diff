using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionJetDatabaseCorruption : MapiPermanentException
	{
		internal MapiExceptionJetDatabaseCorruption(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionJetDatabaseCorruption", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionJetDatabaseCorruption(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
