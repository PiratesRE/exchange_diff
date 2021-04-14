using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionCancelMessage : MapiPermanentException
	{
		internal MapiExceptionCancelMessage(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionCancelMessage", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionCancelMessage(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
