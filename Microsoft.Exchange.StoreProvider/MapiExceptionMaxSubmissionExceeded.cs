using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionMaxSubmissionExceeded : MapiPermanentException
	{
		internal MapiExceptionMaxSubmissionExceeded(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionMaxSubmissionExceeded", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionMaxSubmissionExceeded(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
