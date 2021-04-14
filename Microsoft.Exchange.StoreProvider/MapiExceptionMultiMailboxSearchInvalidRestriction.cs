using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionMultiMailboxSearchInvalidRestriction : MapiPermanentException
	{
		internal MapiExceptionMultiMailboxSearchInvalidRestriction(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionMultiMailboxSearchInvalidRestriction", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionMultiMailboxSearchInvalidRestriction(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
