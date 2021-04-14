using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionJetErrorUnicodeTranslationFail : MapiPermanentException
	{
		internal MapiExceptionJetErrorUnicodeTranslationFail(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionJetErrorUnicodeTranslationFail", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionJetErrorUnicodeTranslationFail(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
