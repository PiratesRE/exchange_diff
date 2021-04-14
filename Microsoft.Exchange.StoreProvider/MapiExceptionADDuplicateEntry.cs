using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionADDuplicateEntry : MapiPermanentException
	{
		internal MapiExceptionADDuplicateEntry(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionADDuplicateEntry", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionADDuplicateEntry(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
