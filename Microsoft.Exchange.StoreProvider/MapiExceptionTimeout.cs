using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionTimeout : MapiRetryableException
	{
		internal MapiExceptionTimeout(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionTimeout", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionTimeout(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
