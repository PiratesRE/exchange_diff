using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionArgumentOutOfRange : ExArgumentException
	{
		internal MapiExceptionArgumentOutOfRange(string argumentName, string message) : base("MapiExceptionArgumentOutOfRange: Param \"" + argumentName + "\" - " + message)
		{
		}

		private MapiExceptionArgumentOutOfRange(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
