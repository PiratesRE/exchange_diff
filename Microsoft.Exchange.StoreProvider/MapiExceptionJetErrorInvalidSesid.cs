using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionJetErrorInvalidSesid : MapiPermanentException
	{
		internal MapiExceptionJetErrorInvalidSesid(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionJetErrorInvalidSesid", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionJetErrorInvalidSesid(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
