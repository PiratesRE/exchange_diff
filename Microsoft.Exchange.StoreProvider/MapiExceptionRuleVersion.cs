using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionRuleVersion : MapiPermanentException
	{
		internal MapiExceptionRuleVersion(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionRuleVersion", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionRuleVersion(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
