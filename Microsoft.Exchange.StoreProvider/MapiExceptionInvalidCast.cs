using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionInvalidCast : ExInvalidCastException
	{
		internal MapiExceptionInvalidCast(string message) : base("MapiExceptionInvalidCast: " + message)
		{
		}

		private MapiExceptionInvalidCast(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
