using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionJetErrorTableInUse : MapiRetryableException
	{
		internal MapiExceptionJetErrorTableInUse(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionJetErrorTableInUse", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionJetErrorTableInUse(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
