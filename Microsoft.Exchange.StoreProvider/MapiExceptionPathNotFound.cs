using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionPathNotFound : MapiPermanentException
	{
		internal MapiExceptionPathNotFound(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionPathNotFound", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionPathNotFound(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
