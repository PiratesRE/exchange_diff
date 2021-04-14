using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionNotEnoughResources : MapiRetryableException
	{
		internal MapiExceptionNotEnoughResources(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionNotEnoughResources", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionNotEnoughResources(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
