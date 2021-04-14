using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionJetErrorDatabaseNotFound : MapiPermanentException
	{
		internal MapiExceptionJetErrorDatabaseNotFound(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionJetErrorDatabaseNotFound", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionJetErrorDatabaseNotFound(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
