using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionFxInvalidState : MapiPermanentException
	{
		internal MapiExceptionFxInvalidState(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionFxInvalidState", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionFxInvalidState(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
