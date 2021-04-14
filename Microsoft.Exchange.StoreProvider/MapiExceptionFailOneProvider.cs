using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionFailOneProvider : MapiPermanentException
	{
		internal MapiExceptionFailOneProvider(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionFailOneProvider", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionFailOneProvider(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
