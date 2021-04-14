using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionMdbOffline : MapiPermanentException
	{
		internal MapiExceptionMdbOffline(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionMdbOffline", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionMdbOffline(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
