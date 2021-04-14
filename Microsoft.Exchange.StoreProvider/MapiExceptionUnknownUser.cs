using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionUnknownUser : MapiPermanentException
	{
		internal MapiExceptionUnknownUser(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionUnknownUser", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionUnknownUser(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
