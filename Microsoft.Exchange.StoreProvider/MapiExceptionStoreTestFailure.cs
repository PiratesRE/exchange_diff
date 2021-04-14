using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionStoreTestFailure : MapiRetryableException
	{
		internal MapiExceptionStoreTestFailure(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionStoreTestFailure", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionStoreTestFailure(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
