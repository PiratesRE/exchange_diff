using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionRequiresRefResolve : MapiPermanentException
	{
		internal MapiExceptionRequiresRefResolve(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionRequiresRefResolve", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionRequiresRefResolve(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
