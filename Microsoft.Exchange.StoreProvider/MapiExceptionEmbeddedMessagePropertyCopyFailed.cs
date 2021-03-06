using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionEmbeddedMessagePropertyCopyFailed : MapiPermanentException
	{
		internal MapiExceptionEmbeddedMessagePropertyCopyFailed(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionEmbeddedMessagePropertyCopyFailed", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionEmbeddedMessagePropertyCopyFailed(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
