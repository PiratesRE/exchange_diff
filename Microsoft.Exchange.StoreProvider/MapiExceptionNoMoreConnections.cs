using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionNoMoreConnections : MapiPermanentException
	{
		internal MapiExceptionNoMoreConnections(string message) : base("MapiExceptionNoMoreConnections", message)
		{
		}

		private MapiExceptionNoMoreConnections(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
