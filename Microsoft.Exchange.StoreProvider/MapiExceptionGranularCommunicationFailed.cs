using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionGranularCommunicationFailed : MapiRetryableException
	{
		internal MapiExceptionGranularCommunicationFailed(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionGranularCommunicationFailed", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionGranularCommunicationFailed(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
