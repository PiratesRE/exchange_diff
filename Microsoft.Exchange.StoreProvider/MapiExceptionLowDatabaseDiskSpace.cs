using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionLowDatabaseDiskSpace : MapiRetryableException
	{
		internal MapiExceptionLowDatabaseDiskSpace(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionLowDatabaseDiskSpace", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionLowDatabaseDiskSpace(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
