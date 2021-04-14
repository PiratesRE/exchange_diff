using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionCrossPostDenied : MapiPermanentException
	{
		internal MapiExceptionCrossPostDenied(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionCrossPostDenied", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionCrossPostDenied(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
