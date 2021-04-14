using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionSyncIncest : MapiPermanentException
	{
		internal MapiExceptionSyncIncest(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionSyncIncest", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionSyncIncest(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
