using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionLockViolation : MapiRetryableException
	{
		internal MapiExceptionLockViolation(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionLockViolation", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionLockViolation(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
