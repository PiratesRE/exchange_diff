using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionNotEnoughMemory : MapiRetryableException
	{
		internal MapiExceptionNotEnoughMemory(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionNotEnoughMemory", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionNotEnoughMemory(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
