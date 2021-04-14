using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionNamedPropsQuotaExceeded : MapiPermanentException
	{
		internal MapiExceptionNamedPropsQuotaExceeded(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionNamedPropsQuotaExceeded", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionNamedPropsQuotaExceeded(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
