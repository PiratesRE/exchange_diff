using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionInvalidParameter : MapiPermanentException
	{
		internal MapiExceptionInvalidParameter(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionInvalidParameter", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionInvalidParameter(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
