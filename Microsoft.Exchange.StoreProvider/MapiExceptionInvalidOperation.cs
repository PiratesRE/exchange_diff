using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionInvalidOperation : MapiPermanentException
	{
		internal MapiExceptionInvalidOperation(string message) : base("MapiExceptionInvalidOperation", message)
		{
		}

		private MapiExceptionInvalidOperation(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
