using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionTooManyProps : MapiPermanentException
	{
		internal MapiExceptionTooManyProps(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionTooManyRecips", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionTooManyProps(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
