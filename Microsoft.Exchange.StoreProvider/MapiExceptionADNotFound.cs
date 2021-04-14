using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionADNotFound : MapiRetryableException
	{
		internal MapiExceptionADNotFound(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionADNotFound", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionADNotFound(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
