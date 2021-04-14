using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionUnknownLcid : MapiPermanentException
	{
		internal MapiExceptionUnknownLcid(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionUnknownLcid", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionUnknownLcid(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
