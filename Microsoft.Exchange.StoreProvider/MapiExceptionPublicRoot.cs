using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionPublicRoot : MapiPermanentException
	{
		internal MapiExceptionPublicRoot(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionPublicRoot", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionPublicRoot(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
