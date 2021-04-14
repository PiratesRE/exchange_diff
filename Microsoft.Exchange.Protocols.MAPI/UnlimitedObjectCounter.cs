using System;
using System.Threading;

namespace Microsoft.Exchange.Protocols.MAPI
{
	internal class UnlimitedObjectCounter : IMapiObjectCounter
	{
		private UnlimitedObjectCounter()
		{
			this.objectCounter = 0L;
		}

		public static IMapiObjectCounter Instance
		{
			get
			{
				return UnlimitedObjectCounter.instance;
			}
		}

		public long GetCount()
		{
			return Interlocked.Read(ref this.objectCounter);
		}

		public void IncrementCount()
		{
			Interlocked.Increment(ref this.objectCounter);
		}

		public void DecrementCount()
		{
			Interlocked.Decrement(ref this.objectCounter);
		}

		public void CheckObjectQuota(bool mustBeStrictlyUnderQuota)
		{
		}

		private static IMapiObjectCounter instance = new UnlimitedObjectCounter();

		private long objectCounter;
	}
}
