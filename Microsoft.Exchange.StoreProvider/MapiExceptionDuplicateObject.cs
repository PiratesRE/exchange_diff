using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionDuplicateObject : MapiPermanentException
	{
		internal MapiExceptionDuplicateObject(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionDuplicateObject", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionDuplicateObject(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
