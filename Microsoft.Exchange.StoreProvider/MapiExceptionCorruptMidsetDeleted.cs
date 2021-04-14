using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionCorruptMidsetDeleted : MapiPermanentException
	{
		internal MapiExceptionCorruptMidsetDeleted(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionCorruptMidsetDeleted", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionCorruptMidsetDeleted(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
