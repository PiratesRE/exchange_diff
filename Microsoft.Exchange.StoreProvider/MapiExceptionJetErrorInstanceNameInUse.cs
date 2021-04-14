using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionJetErrorInstanceNameInUse : MapiPermanentException
	{
		internal MapiExceptionJetErrorInstanceNameInUse(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionJetErrorInstanceNameInUse", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionJetErrorInstanceNameInUse(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
