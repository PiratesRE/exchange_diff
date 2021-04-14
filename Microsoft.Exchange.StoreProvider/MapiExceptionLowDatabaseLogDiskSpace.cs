using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionLowDatabaseLogDiskSpace : MapiRetryableException
	{
		internal MapiExceptionLowDatabaseLogDiskSpace(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionLowDatabaseLogDiskSpace", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionLowDatabaseLogDiskSpace(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
