using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionCollision : MapiRetryableException
	{
		internal MapiExceptionCollision(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionCollision", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionCollision(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
