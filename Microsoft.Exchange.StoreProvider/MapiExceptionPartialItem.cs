using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionPartialItem : MapiPermanentException
	{
		internal MapiExceptionPartialItem(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionPartialItem", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionPartialItem(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
