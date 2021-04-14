using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionMultiMailboxSearchNonFullTextSearch : MapiPermanentException
	{
		internal MapiExceptionMultiMailboxSearchNonFullTextSearch(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionMultiMailboxSearchNonFullTextSearch", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionMultiMailboxSearchNonFullTextSearch(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
