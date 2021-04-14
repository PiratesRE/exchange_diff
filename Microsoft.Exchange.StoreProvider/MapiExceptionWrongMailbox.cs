using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionWrongMailbox : MapiPermanentException
	{
		internal MapiExceptionWrongMailbox(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionWrongMailbox", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionWrongMailbox(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
