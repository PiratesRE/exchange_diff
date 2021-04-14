using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionNoRecordFound : MapiPermanentException
	{
		internal MapiExceptionNoRecordFound(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionNoRecordFound", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionNoRecordFound(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
