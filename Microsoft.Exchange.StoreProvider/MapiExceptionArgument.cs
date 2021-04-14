using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionArgument : ExArgumentException
	{
		internal MapiExceptionArgument(string argumentName, string message) : base("MapiExceptionArgument: Param \"" + argumentName + "\" - " + message)
		{
		}

		private MapiExceptionArgument(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
