using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionCannotRegisterNewNamedPropertyMapping : MapiPermanentException
	{
		internal MapiExceptionCannotRegisterNewNamedPropertyMapping(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionCannotRegisterNewNamedPropertyMapping", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionCannotRegisterNewNamedPropertyMapping(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
