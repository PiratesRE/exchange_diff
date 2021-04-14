using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionJetErrorFileNotFound : MapiPermanentException
	{
		internal MapiExceptionJetErrorFileNotFound(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionJetErrorFileNotFound", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionJetErrorFileNotFound(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
