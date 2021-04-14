using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionUnexpectedId : MapiPermanentException
	{
		internal MapiExceptionUnexpectedId(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionUnexpectedId", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionUnexpectedId(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
