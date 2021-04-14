using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionVirusDetected : MapiPermanentException
	{
		internal MapiExceptionVirusDetected(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionVirusDetected", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionVirusDetected(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
