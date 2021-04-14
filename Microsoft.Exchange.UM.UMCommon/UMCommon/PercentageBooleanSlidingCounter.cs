using System;
using System.Collections.Generic;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class PercentageBooleanSlidingCounter
	{
		private PercentageBooleanSlidingCounter(int maxSize, TimeSpan entryMaxTime, bool invert)
		{
			this.maxSize = maxSize;
			this.invert = invert;
			this.entryMaxTime = entryMaxTime;
			this.queue = new Queue<PercentageBooleanSlidingCounter.SlidingData>(this.maxSize);
			this.syncLock = new object();
		}

		internal static PercentageBooleanSlidingCounter CreateSuccessCounter(int maxSize, TimeSpan entryMaxTime)
		{
			return new PercentageBooleanSlidingCounter(maxSize, entryMaxTime, false);
		}

		internal static PercentageBooleanSlidingCounter CreateFailureCounter(int maxSize, TimeSpan entryMaxTime)
		{
			return new PercentageBooleanSlidingCounter(maxSize, entryMaxTime, true);
		}

		internal int Update(bool operationSucceeded)
		{
			int percentage;
			lock (this.syncLock)
			{
				this.PurgeOldEntries();
				this.AddEntry(operationSucceeded);
				percentage = this.GetPercentage();
			}
			return percentage;
		}

		private void AddEntry(bool operationSucceeded)
		{
			this.queue.Enqueue(new PercentageBooleanSlidingCounter.SlidingData(operationSucceeded));
			if (operationSucceeded)
			{
				this.successCounter++;
				return;
			}
			this.failureCounter++;
		}

		private void PurgeOldEntries()
		{
			while (this.queue.Count >= this.maxSize || (this.queue.Count > 0 && ExDateTime.UtcNow.Subtract(this.queue.Peek().CreationTime) >= this.entryMaxTime))
			{
				PercentageBooleanSlidingCounter.SlidingData slidingData = this.queue.Dequeue();
				if (slidingData.Success)
				{
					this.successCounter--;
				}
				else
				{
					this.failureCounter--;
				}
			}
		}

		private int GetPercentage()
		{
			int num = this.failureCounter + this.successCounter;
			if (this.invert)
			{
				return this.failureCounter * 100 / num;
			}
			return this.successCounter * 100 / num;
		}

		private Queue<PercentageBooleanSlidingCounter.SlidingData> queue;

		private int maxSize;

		private TimeSpan entryMaxTime;

		private object syncLock;

		private bool invert;

		private int successCounter;

		private int failureCounter;

		private class SlidingData
		{
			internal SlidingData(bool success)
			{
				this.CreationTime = ExDateTime.UtcNow;
				this.Success = success;
			}

			internal ExDateTime CreationTime { get; private set; }

			internal bool Success { get; private set; }
		}
	}
}
