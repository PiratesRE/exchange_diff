using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionGranularReplInvalidParameter : MapiRetryableException
	{
		internal MapiExceptionGranularReplInvalidParameter(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionGranularReplInvalidParameter", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionGranularReplInvalidParameter(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
