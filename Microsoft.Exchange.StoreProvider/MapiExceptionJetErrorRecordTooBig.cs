using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionJetErrorRecordTooBig : MapiPermanentException
	{
		internal MapiExceptionJetErrorRecordTooBig(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionJetErrorRecordTooBig", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionJetErrorRecordTooBig(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
