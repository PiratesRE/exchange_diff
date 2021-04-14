using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionInvalidEntryId : MapiPermanentException
	{
		internal MapiExceptionInvalidEntryId(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionInvalidEntryId", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionInvalidEntryId(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
