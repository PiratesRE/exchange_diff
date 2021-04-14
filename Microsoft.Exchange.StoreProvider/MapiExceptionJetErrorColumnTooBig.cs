using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionJetErrorColumnTooBig : MapiPermanentException
	{
		internal MapiExceptionJetErrorColumnTooBig(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionJetErrorColumnTooBig", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionJetErrorColumnTooBig(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
