using System;
using System.Threading;

namespace Microsoft.Exchange.TextProcessing
{
	internal class SmallCounterMap
	{
		public SmallCounterMap()
		{
			this.keys = new long[20];
			this.counts = new int[20];
			this.counterNumbers = 0;
		}

		public int CounterNumbers
		{
			get
			{
				return this.counterNumbers;
			}
		}

		public int NumberOfMajorSource(int majorityThresholdPercent)
		{
			int num = this.CounterNumbers;
			if (num == 0)
			{
				return 0;
			}
			int[] array = new int[num];
			array[0] = this.CounterValue(0);
			int num2 = array[0];
			for (int i = 1; i < num; i++)
			{
				int num3 = this.CounterValue(i);
				num2 += num3;
				int num4 = i;
				while (num4 > 0 && array[num4 - 1] < num3)
				{
					array[num4] = array[num4 - 1];
					num4--;
				}
				array[num4] = num3;
			}
			int num5 = num2 * majorityThresholdPercent / 100;
			int num6 = 0;
			int num7 = 0;
			for (int j = 0; j < num; j++)
			{
				num7 += array[j];
				if (num7 > num5)
				{
					return num6 + 1;
				}
				num6++;
			}
			return num6;
		}

		public int CounterValue(long key)
		{
			int num = this.CounterNumbers;
			for (int i = 0; i < num; i++)
			{
				if (key == this.keys[i])
				{
					return this.CounterValue(i);
				}
			}
			return 0;
		}

		public void Increment(long key)
		{
			for (int i = 0; i < 100; i++)
			{
				if (this.InternalIncrement(key))
				{
					return;
				}
			}
		}

		private bool NotInUse(int i)
		{
			return this.CounterValue(i) == 0;
		}

		private int CounterValue(int i)
		{
			if (i < 20)
			{
				return this.counts[i];
			}
			return 0;
		}

		private bool InternalIncrement(long key)
		{
			int num = 0;
			while (num < 20 && !this.NotInUse(num))
			{
				if (key == this.keys[num])
				{
					Interlocked.Increment(ref this.counts[num]);
					return true;
				}
				num++;
			}
			if (num >= 20)
			{
				return true;
			}
			if (this.NotInUse(num))
			{
				long num2 = Interlocked.CompareExchange(ref this.keys[num], key, 0L);
				if (num2 == 0L)
				{
					Interlocked.Increment(ref this.counterNumbers);
					Interlocked.Increment(ref this.counts[num]);
					return true;
				}
			}
			return false;
		}

		public const int MaxCounters = 20;

		private readonly long[] keys;

		private readonly int[] counts;

		private int counterNumbers;
	}
}
