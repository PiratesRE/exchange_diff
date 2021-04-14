using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionRecoveryMDBMismatch : MapiPermanentException
	{
		internal MapiExceptionRecoveryMDBMismatch(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionRecoveryMDBMismatch", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionRecoveryMDBMismatch(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
