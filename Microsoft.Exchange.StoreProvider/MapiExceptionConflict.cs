using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionConflict : MapiRetryableException
	{
		internal MapiExceptionConflict(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionConflict", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionConflict(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
