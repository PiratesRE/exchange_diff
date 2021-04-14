using System;
using System.Threading;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public class MovingAverage
	{
		public int Average
		{
			get
			{
				int num = this.currentIndex;
				if (this.windowIsFull || num > this.windowValues.Length)
				{
					return this.windowSum >> 10;
				}
				if (num != 0)
				{
					return this.windowSum / num;
				}
				return 0;
			}
		}

		public void AddValue(int value)
		{
			int num = Interlocked.Increment(ref this.currentIndex) - 1 & 1023;
			int num2 = Interlocked.Exchange(ref this.windowValues[num], value);
			Interlocked.Add(ref this.windowSum, value - num2);
			if (num >= this.windowValues.Length - 1)
			{
				this.windowIsFull = true;
			}
		}

		public void Reset()
		{
			for (int i = 0; i < this.windowValues.Length; i++)
			{
				this.windowValues[i] = 0;
			}
			this.currentIndex = 0;
			this.windowSum = 0;
			this.windowIsFull = false;
		}

		private const int BitsFactor = 10;

		private readonly int[] windowValues = new int[1024];

		private int currentIndex;

		private int windowSum;

		private bool windowIsFull;
	}
}
