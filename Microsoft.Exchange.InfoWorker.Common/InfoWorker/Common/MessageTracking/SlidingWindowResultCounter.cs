using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal class SlidingWindowResultCounter
	{
		public SlidingWindowResultCounter(int slidingWindowSize)
		{
			if (slidingWindowSize <= 0)
			{
				throw new ArgumentException("Sliding window size must be greater than zero.");
			}
			this.queue = new Queue<bool>(slidingWindowSize);
			this.slidingWindowSize = slidingWindowSize;
		}

		public void Clear()
		{
			lock (SlidingWindowResultCounter.lockObject)
			{
				this.queue.Clear();
				this.numberOfFailures = 0;
			}
		}

		public void AddSuccess()
		{
			this.AddValue(true);
		}

		public void AddFailure()
		{
			this.AddValue(false);
		}

		public double FailurePercentage
		{
			get
			{
				double result;
				lock (SlidingWindowResultCounter.lockObject)
				{
					if (this.queue.Count > 0)
					{
						result = (double)(this.numberOfFailures * 100) / (double)this.queue.Count;
					}
					else
					{
						result = 0.0;
					}
				}
				return result;
			}
		}

		private void AddValue(bool newValue)
		{
			lock (SlidingWindowResultCounter.lockObject)
			{
				if (this.queue.Count < this.slidingWindowSize)
				{
					this.queue.Enqueue(newValue);
					if (!newValue)
					{
						this.numberOfFailures++;
					}
				}
				else
				{
					bool flag2 = this.queue.Dequeue();
					this.queue.Enqueue(newValue);
					if (flag2 != newValue)
					{
						if (!newValue)
						{
							this.numberOfFailures++;
						}
						else
						{
							this.numberOfFailures--;
						}
					}
				}
			}
		}

		private Queue<bool> queue;

		private int slidingWindowSize;

		private int numberOfFailures;

		private static object lockObject = new object();
	}
}
