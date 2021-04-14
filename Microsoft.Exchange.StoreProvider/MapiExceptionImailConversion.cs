using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionImailConversion : MapiPermanentException
	{
		internal MapiExceptionImailConversion(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionImailConversion", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionImailConversion(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
