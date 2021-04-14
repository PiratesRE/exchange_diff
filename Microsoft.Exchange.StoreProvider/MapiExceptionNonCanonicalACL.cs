using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionNonCanonicalACL : MapiPermanentException
	{
		internal MapiExceptionNonCanonicalACL(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionNonCanonicalACL", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionNonCanonicalACL(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
