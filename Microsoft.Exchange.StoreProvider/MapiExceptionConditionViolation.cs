using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionConditionViolation : MapiRetryableException
	{
		internal MapiExceptionConditionViolation(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionConditionViolation", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionConditionViolation(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
