using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionISIntegQueueFull : MapiRetryableException
	{
		internal MapiExceptionISIntegQueueFull(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionISIntegQueueFull", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionISIntegQueueFull(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
