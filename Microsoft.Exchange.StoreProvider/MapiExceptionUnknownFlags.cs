using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionUnknownFlags : MapiPermanentException
	{
		internal MapiExceptionUnknownFlags(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionUnknownFlags", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionUnknownFlags(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
