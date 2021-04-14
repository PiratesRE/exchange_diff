using System;
using System.Threading;

namespace Microsoft.Exchange.Transport
{
	internal class ForkCount
	{
		internal int Get()
		{
			return this.forkCount;
		}

		internal int Increment()
		{
			return Interlocked.Increment(ref this.forkCount);
		}

		private int forkCount;
	}
}
