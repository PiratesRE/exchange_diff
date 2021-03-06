using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionCIStopping : MapiRetryableException
	{
		internal MapiExceptionCIStopping(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionCIStopping", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionCIStopping(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
