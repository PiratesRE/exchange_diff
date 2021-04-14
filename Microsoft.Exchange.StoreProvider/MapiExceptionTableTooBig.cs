using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionTableTooBig : MapiPermanentException
	{
		internal MapiExceptionTableTooBig(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionTableTooBig", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionTableTooBig(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
