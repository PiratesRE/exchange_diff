using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionNonStandard : MapiPermanentException
	{
		internal MapiExceptionNonStandard(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionNonStandard", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionNonStandard(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
