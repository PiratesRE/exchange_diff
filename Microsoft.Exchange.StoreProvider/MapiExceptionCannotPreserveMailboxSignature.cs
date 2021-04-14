using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionCannotPreserveMailboxSignature : MapiPermanentException
	{
		internal MapiExceptionCannotPreserveMailboxSignature(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionCannotPreserveMailboxSignature", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionCannotPreserveMailboxSignature(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
