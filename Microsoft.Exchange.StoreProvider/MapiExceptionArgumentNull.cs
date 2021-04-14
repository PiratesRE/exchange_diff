using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionArgumentNull : ExArgumentException
	{
		internal MapiExceptionArgumentNull(string argumentName) : base("MapiExceptionArgumentNull: Param \"" + argumentName + "\" is null.")
		{
		}

		private MapiExceptionArgumentNull(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
