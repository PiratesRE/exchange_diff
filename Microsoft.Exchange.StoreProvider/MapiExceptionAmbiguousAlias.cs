using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionAmbiguousAlias : MapiPermanentException
	{
		internal MapiExceptionAmbiguousAlias(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionAmbiguousAlias", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionAmbiguousAlias(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
