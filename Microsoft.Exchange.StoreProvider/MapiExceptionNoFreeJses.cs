using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionNoFreeJses : MapiRetryableException
	{
		internal MapiExceptionNoFreeJses(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionNoFreeJses", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionNoFreeJses(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
