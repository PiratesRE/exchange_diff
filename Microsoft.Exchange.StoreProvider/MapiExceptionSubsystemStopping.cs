using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionSubsystemStopping : MapiRetryableException
	{
		internal MapiExceptionSubsystemStopping(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionSubsystemStopping", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionSubsystemStopping(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
