using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionRuleSendAsDenied : MapiPermanentException
	{
		internal MapiExceptionRuleSendAsDenied(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionRuleSendAsDenied", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionRuleSendAsDenied(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
