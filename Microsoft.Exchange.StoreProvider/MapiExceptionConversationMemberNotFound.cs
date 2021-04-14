using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionConversationMemberNotFound : MapiPermanentException
	{
		internal MapiExceptionConversationMemberNotFound(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionConversationMemberNotFound", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionConversationMemberNotFound(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
