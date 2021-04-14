using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionWatermarkError : MapiPermanentException
	{
		internal MapiExceptionWatermarkError(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionWatermarkError", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionWatermarkError(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
