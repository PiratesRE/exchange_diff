using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionOutOfMemory : MapiPermanentException
	{
		internal MapiExceptionOutOfMemory(string message) : base("MapiExceptionOutOfMemory", message)
		{
		}

		private MapiExceptionOutOfMemory(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
