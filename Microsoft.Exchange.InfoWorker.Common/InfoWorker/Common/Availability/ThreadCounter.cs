using System;
using System.Threading;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	internal sealed class ThreadCounter
	{
		internal ThreadCounter()
		{
		}

		public int Count
		{
			get
			{
				return this.count;
			}
		}

		public void Increment()
		{
			Interlocked.Increment(ref this.count);
		}

		public void Decrement()
		{
			Interlocked.Decrement(ref this.count);
		}

		private int count;
	}
}
