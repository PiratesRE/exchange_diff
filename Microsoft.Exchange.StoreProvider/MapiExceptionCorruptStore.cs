using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionCorruptStore : MapiPermanentException
	{
		internal MapiExceptionCorruptStore(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionCorruptStore", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionCorruptStore(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
