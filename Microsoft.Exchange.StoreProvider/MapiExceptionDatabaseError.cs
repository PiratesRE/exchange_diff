using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionDatabaseError : MapiPermanentException
	{
		internal MapiExceptionDatabaseError(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionDatabaseError", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionDatabaseError(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
