using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionBadColumn : MapiPermanentException
	{
		internal MapiExceptionBadColumn(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionBadColumn", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionBadColumn(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
