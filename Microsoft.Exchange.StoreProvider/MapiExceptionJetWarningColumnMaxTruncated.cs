using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionJetWarningColumnMaxTruncated : MapiPermanentException
	{
		internal MapiExceptionJetWarningColumnMaxTruncated(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionJetWarningColumnMaxTruncated", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionJetWarningColumnMaxTruncated(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
