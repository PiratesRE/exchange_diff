using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionUnknownCpid : MapiPermanentException
	{
		internal MapiExceptionUnknownCpid(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionUnknownCpid", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionUnknownCpid(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
