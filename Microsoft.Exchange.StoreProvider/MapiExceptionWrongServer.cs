using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionWrongServer : MapiPermanentException
	{
		internal MapiExceptionWrongServer(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionWrongServer", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionWrongServer(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
