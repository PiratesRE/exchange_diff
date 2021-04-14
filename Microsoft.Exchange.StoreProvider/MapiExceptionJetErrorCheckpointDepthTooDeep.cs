using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionJetErrorCheckpointDepthTooDeep : MapiRetryableException
	{
		internal MapiExceptionJetErrorCheckpointDepthTooDeep(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionJetErrorCheckpointDepthTooDeep", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionJetErrorCheckpointDepthTooDeep(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
