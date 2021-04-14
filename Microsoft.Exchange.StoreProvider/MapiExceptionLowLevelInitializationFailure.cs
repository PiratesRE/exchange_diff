using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionLowLevelInitializationFailure : BaseException
	{
		internal MapiExceptionLowLevelInitializationFailure(string message) : base("MapiExceptionLowLevelInitializationFailure: " + message)
		{
		}

		private MapiExceptionLowLevelInitializationFailure(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
