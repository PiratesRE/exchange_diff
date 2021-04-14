using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionCannotDeriveMsgViewFromBase : MapiPermanentException
	{
		internal MapiExceptionCannotDeriveMsgViewFromBase(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionCannotDeriveMsgViewFromBase", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionCannotDeriveMsgViewFromBase(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
