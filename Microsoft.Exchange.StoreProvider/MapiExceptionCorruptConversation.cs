using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionCorruptConversation : MapiPermanentException
	{
		internal MapiExceptionCorruptConversation(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionCorruptConversation", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionCorruptConversation(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
