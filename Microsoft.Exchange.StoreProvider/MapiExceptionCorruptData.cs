using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionCorruptData : MapiPermanentException
	{
		internal MapiExceptionCorruptData(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionCorruptData", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionCorruptData(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
