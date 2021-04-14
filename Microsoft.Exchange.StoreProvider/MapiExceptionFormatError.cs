using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionFormatError : MapiPermanentException
	{
		internal MapiExceptionFormatError(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionFormatError", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionFormatError(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
