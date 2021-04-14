using System;
using System.Threading;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public class LimitedCounter
	{
		public LimitedCounter(uint limit)
		{
			this.limit = (int)limit;
			this.counter = 0;
		}

		public bool IsIncrementedValueOverLimit()
		{
			bool result = false;
			int num = Interlocked.Increment(ref this.counter);
			if (num > this.limit)
			{
				result = true;
			}
			return result;
		}

		public void Decrement()
		{
			Interlocked.Decrement(ref this.counter);
		}

		private int counter;

		private readonly int limit;
	}
}
