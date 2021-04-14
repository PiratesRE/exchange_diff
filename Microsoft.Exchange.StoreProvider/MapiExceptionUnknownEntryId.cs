using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionUnknownEntryId : MapiPermanentException
	{
		internal MapiExceptionUnknownEntryId(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionUnknownEntryId", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionUnknownEntryId(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
