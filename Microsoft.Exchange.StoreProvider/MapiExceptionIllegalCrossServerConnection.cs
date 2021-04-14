using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionIllegalCrossServerConnection : MapiPermanentException
	{
		internal MapiExceptionIllegalCrossServerConnection(string message) : base("MapiExceptionIllegalCrossServerConnection", message)
		{
		}

		private MapiExceptionIllegalCrossServerConnection(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
