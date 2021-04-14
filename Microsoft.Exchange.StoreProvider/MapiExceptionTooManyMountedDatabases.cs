using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionTooManyMountedDatabases : MapiPermanentException
	{
		internal MapiExceptionTooManyMountedDatabases(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionTooManyMountedDatabases", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionTooManyMountedDatabases(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
