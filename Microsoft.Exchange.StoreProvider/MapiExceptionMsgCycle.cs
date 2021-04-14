using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionMsgCycle : MapiPermanentException
	{
		internal MapiExceptionMsgCycle(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionMsgCycle", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionMsgCycle(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
