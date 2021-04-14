using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionJetErrorPageNotInitialized : MapiPermanentException
	{
		internal MapiExceptionJetErrorPageNotInitialized(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionJetErrorPageNotInitialized", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionJetErrorPageNotInitialized(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
