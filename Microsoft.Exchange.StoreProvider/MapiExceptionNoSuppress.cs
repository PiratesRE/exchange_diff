using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionNoSuppress : MapiPermanentException
	{
		internal MapiExceptionNoSuppress(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionNoSuppress", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionNoSuppress(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
