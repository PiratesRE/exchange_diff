using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionRecursionLimit : MapiPermanentException
	{
		internal MapiExceptionRecursionLimit(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionRecursionLimit", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionRecursionLimit(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
