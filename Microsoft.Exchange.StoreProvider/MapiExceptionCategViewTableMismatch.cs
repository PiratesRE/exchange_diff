using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionCategViewTableMismatch : MapiPermanentException
	{
		internal MapiExceptionCategViewTableMismatch(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionCategViewTableMismatch", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionCategViewTableMismatch(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
