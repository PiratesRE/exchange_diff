using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionMaxAttachmentExceeded : MapiPermanentException
	{
		internal MapiExceptionMaxAttachmentExceeded(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionMaxAttachmentExceeded", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionMaxAttachmentExceeded(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
