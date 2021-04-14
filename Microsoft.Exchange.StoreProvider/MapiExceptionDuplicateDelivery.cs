using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionDuplicateDelivery : MapiPermanentException
	{
		internal MapiExceptionDuplicateDelivery(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionDuplicateDelivery", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionDuplicateDelivery(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
