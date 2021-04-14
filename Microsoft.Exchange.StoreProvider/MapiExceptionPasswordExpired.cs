using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionPasswordExpired : MapiPermanentException
	{
		internal MapiExceptionPasswordExpired(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionPasswordExpired", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionPasswordExpired(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
