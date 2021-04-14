using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionObjectDeleted : MapiPermanentException
	{
		internal MapiExceptionObjectDeleted(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionObjectDeleted", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionObjectDeleted(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
