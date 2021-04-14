using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionNotSupported : MapiPermanentException
	{
		internal MapiExceptionNotSupported(string message) : base("MapiExceptionNotSupported", message)
		{
		}

		private MapiExceptionNotSupported(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
