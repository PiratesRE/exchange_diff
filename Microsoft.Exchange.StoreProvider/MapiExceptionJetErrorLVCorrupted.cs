using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionJetErrorLVCorrupted : MapiPermanentException
	{
		internal MapiExceptionJetErrorLVCorrupted(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionJetErrorLVCorrupted", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionJetErrorLVCorrupted(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
