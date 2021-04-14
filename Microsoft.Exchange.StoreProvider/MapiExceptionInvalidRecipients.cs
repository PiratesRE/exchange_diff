using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionInvalidRecipients : MapiPermanentException
	{
		internal MapiExceptionInvalidRecipients(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionInvalidRecipients", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionInvalidRecipients(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
