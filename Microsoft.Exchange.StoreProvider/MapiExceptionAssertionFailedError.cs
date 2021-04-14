using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionAssertionFailedError : MapiPermanentException
	{
		internal MapiExceptionAssertionFailedError(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionAssertionFailedError", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionAssertionFailedError(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
