using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionProtocolDisabled : MapiPermanentException
	{
		internal MapiExceptionProtocolDisabled(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionProtocolDisabled", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionProtocolDisabled(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
