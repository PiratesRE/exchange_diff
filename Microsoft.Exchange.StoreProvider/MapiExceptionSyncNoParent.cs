using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionSyncNoParent : MapiPermanentException
	{
		internal MapiExceptionSyncNoParent(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionSyncNoParent", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionSyncNoParent(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
