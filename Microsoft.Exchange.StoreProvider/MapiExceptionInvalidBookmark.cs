using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionInvalidBookmark : MapiPermanentException
	{
		internal MapiExceptionInvalidBookmark(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionInvalidBookmark", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionInvalidBookmark(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
