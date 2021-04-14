using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionFileOpenFailure : MapiPermanentException
	{
		internal MapiExceptionFileOpenFailure(string message, int hr) : base("MapiExceptionFileOpenFailure", message, hr, 0, null, null)
		{
		}

		private MapiExceptionFileOpenFailure(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
