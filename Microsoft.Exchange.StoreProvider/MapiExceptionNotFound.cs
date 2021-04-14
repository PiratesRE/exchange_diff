using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionNotFound : MapiPermanentException
	{
		internal MapiExceptionNotFound(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionNotFound", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionNotFound(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
