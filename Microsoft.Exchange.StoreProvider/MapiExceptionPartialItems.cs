using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionPartialItems : MapiPermanentException
	{
		internal MapiExceptionPartialItems(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionPartialItems", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionPartialItems(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
