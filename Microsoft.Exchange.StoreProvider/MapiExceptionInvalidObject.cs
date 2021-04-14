using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionInvalidObject : MapiRetryableException
	{
		internal MapiExceptionInvalidObject(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionInvalidObject", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionInvalidObject(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
