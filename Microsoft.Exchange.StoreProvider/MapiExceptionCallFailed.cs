using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionCallFailed : MapiPermanentException
	{
		internal MapiExceptionCallFailed(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionCallFailed", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionCallFailed(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
