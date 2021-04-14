using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionNotEnoughDisk : MapiRetryableException
	{
		internal MapiExceptionNotEnoughDisk(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionNotEnoughDisk", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionNotEnoughDisk(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
