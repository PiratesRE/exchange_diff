using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionFailCallback : MapiPermanentException
	{
		internal MapiExceptionFailCallback(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionFailCallback", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionFailCallback(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
