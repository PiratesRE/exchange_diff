using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionInvalidMultiMailboxSearchRequest : MapiPermanentException
	{
		internal MapiExceptionInvalidMultiMailboxSearchRequest(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionInvalidMultiMailboxSearchRequest", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionInvalidMultiMailboxSearchRequest(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
