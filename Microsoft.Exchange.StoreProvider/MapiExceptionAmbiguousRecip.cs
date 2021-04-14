using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionAmbiguousRecip : MapiPermanentException
	{
		internal MapiExceptionAmbiguousRecip(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionAmbiguousRecip", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionAmbiguousRecip(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
