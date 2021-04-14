using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionDataIntegrity : MapiPermanentException
	{
		internal MapiExceptionDataIntegrity(string message) : base("MapiExceptionDataIntegrity", message)
		{
		}

		private MapiExceptionDataIntegrity(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
