using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionJetErrorDiskFull : MapiPermanentException
	{
		internal MapiExceptionJetErrorDiskFull(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionJetErrorDiskFull", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionJetErrorDiskFull(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
