using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionDeclineCopy : MapiRetryableException
	{
		internal MapiExceptionDeclineCopy(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionDeclineCopy", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionDeclineCopy(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
