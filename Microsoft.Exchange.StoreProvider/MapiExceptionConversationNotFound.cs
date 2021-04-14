using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionConversationNotFound : MapiPermanentException
	{
		internal MapiExceptionConversationNotFound(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionConversationNotFound", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionConversationNotFound(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
