using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionUnsupportedProp : MapiPermanentException
	{
		internal MapiExceptionUnsupportedProp(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionUnsupportedProp", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionUnsupportedProp(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
