using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionNotPrivateMDB : MapiPermanentException
	{
		internal MapiExceptionNotPrivateMDB(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionNotPrivateMDB", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionNotPrivateMDB(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
