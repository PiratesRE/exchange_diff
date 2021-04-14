using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionJetErrorIndexNotFound : MapiPermanentException
	{
		internal MapiExceptionJetErrorIndexNotFound(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionJetErrorIndexNotFound", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionJetErrorIndexNotFound(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
