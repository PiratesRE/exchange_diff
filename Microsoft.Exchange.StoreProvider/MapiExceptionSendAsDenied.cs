using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionSendAsDenied : MapiPermanentException
	{
		internal MapiExceptionSendAsDenied(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionSendAsDenied", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionSendAsDenied(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
