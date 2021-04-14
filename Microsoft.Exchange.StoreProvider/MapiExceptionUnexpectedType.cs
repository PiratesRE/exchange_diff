using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionUnexpectedType : MapiPermanentException
	{
		internal MapiExceptionUnexpectedType(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionUnexpectedType", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionUnexpectedType(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
