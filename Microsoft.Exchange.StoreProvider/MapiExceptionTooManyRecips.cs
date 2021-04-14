using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionTooManyRecips : MapiPermanentException
	{
		internal MapiExceptionTooManyRecips(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionTooManyRecips", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionTooManyRecips(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
