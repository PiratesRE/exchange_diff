using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionJetErrorBadColumnId : MapiPermanentException
	{
		internal MapiExceptionJetErrorBadColumnId(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionJetErrorBadColumnId", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionJetErrorBadColumnId(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
