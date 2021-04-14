using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionCanNotComplete : MapiRetryableException
	{
		internal MapiExceptionCanNotComplete(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionCanNotComplete", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionCanNotComplete(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
