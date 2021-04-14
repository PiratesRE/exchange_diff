using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionRetryableImportFailure : MapiRetryableException
	{
		internal MapiExceptionRetryableImportFailure(string message, Exception innerException) : base("MapiExceptionRetryableImportFailure", message, innerException)
		{
		}

		private MapiExceptionRetryableImportFailure(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
