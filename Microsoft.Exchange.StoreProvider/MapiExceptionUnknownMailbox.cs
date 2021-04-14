using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionUnknownMailbox : MapiPermanentException
	{
		internal MapiExceptionUnknownMailbox(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionUnknownMailbox", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionUnknownMailbox(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
