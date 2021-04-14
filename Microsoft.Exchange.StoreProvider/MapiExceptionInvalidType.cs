using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionInvalidType : MapiPermanentException
	{
		internal MapiExceptionInvalidType(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionInvalidType", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionInvalidType(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
