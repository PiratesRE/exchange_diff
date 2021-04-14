using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionTooComplex : MapiPermanentException
	{
		internal MapiExceptionTooComplex(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionTooComplex", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionTooComplex(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
