using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionVirusScannerError : MapiPermanentException
	{
		internal MapiExceptionVirusScannerError(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionVirusScannerError", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionVirusScannerError(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
