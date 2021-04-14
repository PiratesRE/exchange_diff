using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionPasswordChangeRequired : MapiPermanentException
	{
		internal MapiExceptionPasswordChangeRequired(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionPasswordChangeRequired", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionPasswordChangeRequired(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
