using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionMultiMailboxSearchNonFullTextSortBy : MapiPermanentException
	{
		internal MapiExceptionMultiMailboxSearchNonFullTextSortBy(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionMultiMailboxSearchNonFullTextSortBy", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionMultiMailboxSearchNonFullTextSortBy(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
