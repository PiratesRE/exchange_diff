using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionExceededMapiStoreLimit : MapiPermanentException
	{
		internal MapiExceptionExceededMapiStoreLimit(string message) : base("MapiExceptionExceededMapiStoreLimit", message)
		{
		}

		private MapiExceptionExceededMapiStoreLimit(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
