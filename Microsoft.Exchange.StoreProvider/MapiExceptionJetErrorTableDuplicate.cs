using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionJetErrorTableDuplicate : MapiPermanentException
	{
		internal MapiExceptionJetErrorTableDuplicate(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionJetErrorTableDuplicate", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionJetErrorTableDuplicate(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
