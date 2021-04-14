using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionNotAuthorized : MapiPermanentException
	{
		internal MapiExceptionNotAuthorized(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionNotAuthorized", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionNotAuthorized(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
