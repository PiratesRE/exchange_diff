using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionMessageTooBig : MapiPermanentException
	{
		internal MapiExceptionMessageTooBig(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionMessageTooBig", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionMessageTooBig(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
