using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionNoMessages : MapiPermanentException
	{
		internal MapiExceptionNoMessages(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionNoMessages", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionNoMessages(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
