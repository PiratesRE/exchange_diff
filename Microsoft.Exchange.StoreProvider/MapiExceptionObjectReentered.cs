using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionObjectReentered : BaseException
	{
		internal MapiExceptionObjectReentered(string message) : base("MapiExceptionObjectReentered: " + message)
		{
		}

		private MapiExceptionObjectReentered(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
