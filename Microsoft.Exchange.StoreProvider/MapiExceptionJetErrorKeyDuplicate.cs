using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionJetErrorKeyDuplicate : MapiPermanentException
	{
		internal MapiExceptionJetErrorKeyDuplicate(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionJetErrorKeyDuplicate", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionJetErrorKeyDuplicate(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
