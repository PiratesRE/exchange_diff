using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionJetErrorFileIOBeyondEOF : MapiRetryableException
	{
		internal MapiExceptionJetErrorFileIOBeyondEOF(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionJetErrorFileIOBeyondEOF", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionJetErrorFileIOBeyondEOF(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
