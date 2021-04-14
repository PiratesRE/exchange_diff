using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionNotEncrypted : MapiPermanentException
	{
		internal MapiExceptionNotEncrypted(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionNotEncrypted", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionNotEncrypted(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
