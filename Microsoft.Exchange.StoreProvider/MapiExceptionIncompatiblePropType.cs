using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionIncompatiblePropType : MapiPermanentException
	{
		internal MapiExceptionIncompatiblePropType(string message) : base("MapiExceptionIncompatiblePropType", message)
		{
		}

		private MapiExceptionIncompatiblePropType(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
