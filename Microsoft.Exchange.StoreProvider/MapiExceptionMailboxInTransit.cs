using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionMailboxInTransit : MapiRetryableException
	{
		public MapiExceptionMailboxInTransit(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionMailboxInTransit", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionMailboxInTransit(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
