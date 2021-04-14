using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Clients.Common
{
	internal class FixedSizeQueueBool
	{
		public double Mean
		{
			get
			{
				if (this.queue.Count > 0)
				{
					return (double)this.trueCount / (double)this.queue.Count;
				}
				return 0.0;
			}
		}

		public int Count
		{
			get
			{
				return this.queue.Count;
			}
		}

		public int TrueCount
		{
			get
			{
				return this.trueCount;
			}
		}

		public FixedSizeQueueBool(int windowSize)
		{
			if (windowSize <= 0)
			{
				throw new ArgumentException("Input windowSize cannot be <= 0");
			}
			this.queue = new Queue<bool>(windowSize);
			this.windowSize = windowSize;
		}

		public void Clear()
		{
			this.queue.Clear();
			this.trueCount = 0;
		}

		public void AddSample(bool sample)
		{
			if (this.queue.Count == this.windowSize && this.queue.Dequeue())
			{
				this.trueCount--;
			}
			this.queue.Enqueue(sample);
			if (sample)
			{
				this.trueCount++;
			}
		}

		private readonly int windowSize;

		private Queue<bool> queue;

		private int trueCount;
	}
}
