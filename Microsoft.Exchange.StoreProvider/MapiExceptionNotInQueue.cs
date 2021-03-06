using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionNotInQueue : MapiPermanentException
	{
		internal MapiExceptionNotInQueue(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionNotInQueue", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionNotInQueue(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
