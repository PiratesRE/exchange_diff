using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionNotInitialized : MapiPermanentException
	{
		internal MapiExceptionNotInitialized(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionNotInitialized", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionNotInitialized(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
