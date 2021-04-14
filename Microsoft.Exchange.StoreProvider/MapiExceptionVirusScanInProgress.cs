using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionVirusScanInProgress : MapiRetryableException
	{
		internal MapiExceptionVirusScanInProgress(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionVirusScanInProgress", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionVirusScanInProgress(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
