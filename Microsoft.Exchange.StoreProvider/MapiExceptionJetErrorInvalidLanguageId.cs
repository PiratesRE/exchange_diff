using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionJetErrorInvalidLanguageId : MapiPermanentException
	{
		internal MapiExceptionJetErrorInvalidLanguageId(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionJetErrorInvalidLanguageId", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionJetErrorInvalidLanguageId(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
