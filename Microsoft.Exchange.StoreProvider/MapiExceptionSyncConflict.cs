using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionSyncConflict : MapiPermanentException
	{
		internal MapiExceptionSyncConflict(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionSyncConflict", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionSyncConflict(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
