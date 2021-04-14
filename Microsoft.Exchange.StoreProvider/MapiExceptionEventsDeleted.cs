using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionEventsDeleted : MapiPermanentException
	{
		internal MapiExceptionEventsDeleted(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionEventsDeleted", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionEventsDeleted(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
