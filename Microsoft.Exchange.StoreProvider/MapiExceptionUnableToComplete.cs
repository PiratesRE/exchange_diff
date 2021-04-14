using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionUnableToComplete : MapiPermanentException
	{
		internal MapiExceptionUnableToComplete(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionUnableToComplete", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionUnableToComplete(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
