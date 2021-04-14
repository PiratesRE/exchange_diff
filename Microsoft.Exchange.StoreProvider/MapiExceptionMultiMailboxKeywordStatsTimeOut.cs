using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionMultiMailboxKeywordStatsTimeOut : MapiRetryableException
	{
		internal MapiExceptionMultiMailboxKeywordStatsTimeOut(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionMultiMailboxKeywordStatsTimeOut", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionMultiMailboxKeywordStatsTimeOut(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
