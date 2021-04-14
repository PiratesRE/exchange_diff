using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionObjectDisposed : ExObjectDisposedException
	{
		internal MapiExceptionObjectDisposed(string message) : base("MapiExceptionObjectDisposed: " + message)
		{
		}

		private MapiExceptionObjectDisposed(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
