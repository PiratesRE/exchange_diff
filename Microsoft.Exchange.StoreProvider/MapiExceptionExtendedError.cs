using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionExtendedError : MapiPermanentException
	{
		internal MapiExceptionExtendedError(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base(" MapiExceptionExtendedError", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionExtendedError(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
