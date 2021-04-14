using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionBackupInProgress : MapiRetryableException
	{
		internal MapiExceptionBackupInProgress(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionBackupInProgress", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionBackupInProgress(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
