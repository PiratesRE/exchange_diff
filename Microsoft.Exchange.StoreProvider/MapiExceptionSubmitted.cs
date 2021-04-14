using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionSubmitted : MapiPermanentException
	{
		internal MapiExceptionSubmitted(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionSubmitted", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionSubmitted(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
