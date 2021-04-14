using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionNoRecipients : MapiPermanentException
	{
		internal MapiExceptionNoRecipients(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionNoRecipients", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionNoRecipients(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
