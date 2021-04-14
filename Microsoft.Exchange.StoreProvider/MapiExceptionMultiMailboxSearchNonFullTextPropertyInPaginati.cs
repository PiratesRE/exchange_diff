using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionMultiMailboxSearchNonFullTextPropertyInPagination : MapiPermanentException
	{
		internal MapiExceptionMultiMailboxSearchNonFullTextPropertyInPagination(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionMultiMailboxSearchInvalidSortBy", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionMultiMailboxSearchNonFullTextPropertyInPagination(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
