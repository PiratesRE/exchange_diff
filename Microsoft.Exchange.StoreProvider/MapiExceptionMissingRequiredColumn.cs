using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionMissingRequiredColumn : MapiPermanentException
	{
		internal MapiExceptionMissingRequiredColumn(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionMissingRequiredColumn", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionMissingRequiredColumn(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
