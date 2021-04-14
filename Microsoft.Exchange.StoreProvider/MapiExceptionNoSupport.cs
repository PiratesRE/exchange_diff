using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionNoSupport : MapiPermanentException
	{
		internal MapiExceptionNoSupport(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionNoSupport", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionNoSupport(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
