using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionTableEmpty : MapiRetryableException
	{
		internal MapiExceptionTableEmpty(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionTableEmpty", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionTableEmpty(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
