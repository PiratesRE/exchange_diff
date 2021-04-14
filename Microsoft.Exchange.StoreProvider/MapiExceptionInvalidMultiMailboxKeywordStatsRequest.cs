using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionInvalidMultiMailboxKeywordStatsRequest : MapiPermanentException
	{
		internal MapiExceptionInvalidMultiMailboxKeywordStatsRequest(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionInvalidMultiMailboxKeywordStatsRequest", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionInvalidMultiMailboxKeywordStatsRequest(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
