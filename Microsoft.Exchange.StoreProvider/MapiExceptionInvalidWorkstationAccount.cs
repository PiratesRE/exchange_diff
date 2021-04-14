using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionInvalidWorkstationAccount : MapiPermanentException
	{
		internal MapiExceptionInvalidWorkstationAccount(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionInvalidWorkstationAccount", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionInvalidWorkstationAccount(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
