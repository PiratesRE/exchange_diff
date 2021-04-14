using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionTooBig : MapiPermanentException
	{
		internal MapiExceptionTooBig(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionTooBig", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionTooBig(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
