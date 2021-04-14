using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionObjectNotLocked : BaseException
	{
		internal MapiExceptionObjectNotLocked(string message) : base("MapiExceptionObjectNotLocked: " + message)
		{
		}

		private MapiExceptionObjectNotLocked(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
