using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionJetErrorInvalidBufferSize : MapiPermanentException
	{
		internal MapiExceptionJetErrorInvalidBufferSize(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionJetErrorInvalidBufferSize", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionJetErrorInvalidBufferSize(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
