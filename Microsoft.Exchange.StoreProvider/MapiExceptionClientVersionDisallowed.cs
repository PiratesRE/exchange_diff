using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionClientVersionDisallowed : MapiPermanentException
	{
		internal MapiExceptionClientVersionDisallowed(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionClientVersionDisallowed", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionClientVersionDisallowed(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
