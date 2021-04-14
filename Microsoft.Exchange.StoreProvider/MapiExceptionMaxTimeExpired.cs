using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionMaxTimeExpired : MapiPermanentException
	{
		internal MapiExceptionMaxTimeExpired(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionMaxTimeExpired", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionMaxTimeExpired(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
