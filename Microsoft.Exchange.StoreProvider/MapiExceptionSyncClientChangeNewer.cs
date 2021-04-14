using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionSyncClientChangeNewer : MapiPermanentException
	{
		internal MapiExceptionSyncClientChangeNewer(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionSyncClientChangeNewer", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionSyncClientChangeNewer(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
