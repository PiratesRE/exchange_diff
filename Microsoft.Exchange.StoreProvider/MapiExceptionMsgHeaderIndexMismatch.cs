using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionMsgHeaderIndexMismatch : MapiPermanentException
	{
		internal MapiExceptionMsgHeaderIndexMismatch(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionMsgHeaderIndexMismatch", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionMsgHeaderIndexMismatch(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
