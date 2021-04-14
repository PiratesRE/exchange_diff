using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionVersion : MapiPermanentException
	{
		internal MapiExceptionVersion(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionVersion", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionVersion(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
