using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionFxUnexpectedMarker : MapiPermanentException
	{
		internal MapiExceptionFxUnexpectedMarker(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionFxUnexpectedMarker", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionFxUnexpectedMarker(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
