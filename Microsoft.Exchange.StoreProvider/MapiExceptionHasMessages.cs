using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionHasMessages : MapiPermanentException
	{
		internal MapiExceptionHasMessages(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionHasMessages", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionHasMessages(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
