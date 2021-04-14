using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionNoCreateRight : MapiPermanentException
	{
		internal MapiExceptionNoCreateRight(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionNoCreateRight", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionNoCreateRight(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
