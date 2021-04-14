using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionLockIdLimit : MapiPermanentException
	{
		internal MapiExceptionLockIdLimit(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionLockIdLimit", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionLockIdLimit(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
