using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionStringTooLong : MapiPermanentException
	{
		internal MapiExceptionStringTooLong(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionStringTooLong", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionStringTooLong(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
