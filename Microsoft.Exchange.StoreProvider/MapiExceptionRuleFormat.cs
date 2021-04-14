using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionRuleFormat : MapiPermanentException
	{
		internal MapiExceptionRuleFormat(string message) : base("MapiExceptionRuleFormat", message)
		{
		}

		internal MapiExceptionRuleFormat(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionRuleFormat", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionRuleFormat(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
