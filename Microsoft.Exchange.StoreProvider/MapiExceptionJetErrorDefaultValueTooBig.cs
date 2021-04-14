using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionJetErrorDefaultValueTooBig : MapiPermanentException
	{
		internal MapiExceptionJetErrorDefaultValueTooBig(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionJetErrorDefaultValueTooBig", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionJetErrorDefaultValueTooBig(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
