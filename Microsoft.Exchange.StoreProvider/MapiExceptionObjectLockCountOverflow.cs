using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionObjectLockCountOverflow : MapiPermanentException
	{
		internal MapiExceptionObjectLockCountOverflow(string message) : base("MapiExceptionObjectLockCountOverflow", message)
		{
		}

		private MapiExceptionObjectLockCountOverflow(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
