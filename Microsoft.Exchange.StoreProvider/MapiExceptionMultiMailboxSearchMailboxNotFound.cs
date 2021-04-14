using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionMultiMailboxSearchMailboxNotFound : MapiPermanentException
	{
		internal MapiExceptionMultiMailboxSearchMailboxNotFound(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionMultiMailboxSearchMailboxNotFound", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionMultiMailboxSearchMailboxNotFound(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
