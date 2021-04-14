using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionUnexpectedMailboxState : MapiPermanentException
	{
		internal MapiExceptionUnexpectedMailboxState(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionUnexpectedMailboxState", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionUnexpectedMailboxState(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
