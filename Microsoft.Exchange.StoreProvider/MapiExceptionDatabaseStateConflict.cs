using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionDatabaseStateConflict : MapiPermanentException
	{
		internal MapiExceptionDatabaseStateConflict(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionDatabaseStateConflict", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionDatabaseStateConflict(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
