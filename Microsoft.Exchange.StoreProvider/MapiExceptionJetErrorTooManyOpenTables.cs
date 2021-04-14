using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionJetErrorTooManyOpenTables : MapiRetryableException
	{
		internal MapiExceptionJetErrorTooManyOpenTables(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionJetErrorTooManyOpenTables", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionJetErrorTooManyOpenTables(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
