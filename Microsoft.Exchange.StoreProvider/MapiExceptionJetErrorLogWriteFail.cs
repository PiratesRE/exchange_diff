using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionJetErrorLogWriteFail : MapiRetryableException
	{
		internal MapiExceptionJetErrorLogWriteFail(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionJetErrorLogWriteFail", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionJetErrorLogWriteFail(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
