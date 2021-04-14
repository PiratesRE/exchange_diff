using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class LockableStream : Stream
	{
		public virtual void LockRegion(long offset, long cb, int lockType)
		{
		}

		public virtual void UnlockRegion(long offset, long cb, int lockType)
		{
		}
	}
}
