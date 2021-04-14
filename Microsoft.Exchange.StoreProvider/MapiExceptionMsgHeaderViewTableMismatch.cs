using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionMsgHeaderViewTableMismatch : MapiPermanentException
	{
		internal MapiExceptionMsgHeaderViewTableMismatch(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionMsgHeaderViewTableMismatch", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionMsgHeaderViewTableMismatch(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
