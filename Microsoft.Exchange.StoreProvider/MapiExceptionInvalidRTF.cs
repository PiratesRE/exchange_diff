using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionInvalidRTF : MapiPermanentException
	{
		internal MapiExceptionInvalidRTF(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionInvalidRTF", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionInvalidRTF(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
