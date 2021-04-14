using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ResourceTracker : IResourceTracker
	{
		public ResourceTracker(int streamSizeLimit)
		{
			this.streamSizeLimit = streamSizeLimit;
			this.sharedMemoryLimit = (long)streamSizeLimit * 5L;
			if (this.sharedMemoryLimit > 2147483647L)
			{
				this.sharedMemoryLimit = 2147483647L;
			}
		}

		public bool TryReserveMemory(int size)
		{
			if (this.reservedSharedMemory + (long)size < 0L)
			{
				throw new ArgumentOutOfRangeException("size", string.Format("Attempted to return more memory than was originally reserved. Current reserved memory = {0}. Reserve memory request = {1}.", this.reservedSharedMemory, size));
			}
			if (this.reservedSharedMemory + (long)size > this.sharedMemoryLimit)
			{
				return false;
			}
			this.reservedSharedMemory += (long)size;
			return true;
		}

		public int StreamSizeLimit
		{
			get
			{
				return this.streamSizeLimit;
			}
		}

		private readonly long sharedMemoryLimit;

		private readonly int streamSizeLimit;

		private long reservedSharedMemory;
	}
}
