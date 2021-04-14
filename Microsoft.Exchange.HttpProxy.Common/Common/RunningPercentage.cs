using System;

namespace Microsoft.Exchange.HttpProxy.Common
{
	internal class RunningPercentage
	{
		public RunningPercentage(ushort numberOfSamples)
		{
			if (numberOfSamples < 2)
			{
				throw new ArgumentException("numberOfSamples must be greater than 1", "numberOfSamples");
			}
			this.numberOfSamples = numberOfSamples;
			this.cachedNumerators = new bool[(int)this.numberOfSamples];
		}

		public long Value
		{
			get
			{
				return this.cachedValue;
			}
		}

		public long Update()
		{
			lock (this.lockObject)
			{
				if (!this.cachedNumerators[this.movingIndex])
				{
					this.totalSetCachedNumerators += 1L;
				}
				this.cachedNumerators[this.movingIndex] = true;
				this.UpdateCachedValue();
			}
			return this.cachedValue;
		}

		public long IncrementBase()
		{
			lock (this.lockObject)
			{
				this.IncrementNumeratorIndex();
				if (this.cachedNumerators[this.movingIndex])
				{
					this.totalSetCachedNumerators -= 1L;
				}
				this.cachedNumerators[this.movingIndex] = false;
				if (this.cachedDenominator < (long)((ulong)this.numberOfSamples))
				{
					this.cachedDenominator += 1L;
				}
				this.UpdateCachedValue();
			}
			return this.cachedValue;
		}

		private void IncrementNumeratorIndex()
		{
			this.movingIndex++;
			if (this.movingIndex >= (int)this.numberOfSamples)
			{
				this.movingIndex = 0;
			}
		}

		private void UpdateCachedValue()
		{
			if (this.cachedDenominator > 0L)
			{
				this.cachedValue = this.totalSetCachedNumerators * 100L / this.cachedDenominator;
			}
		}

		private readonly ushort numberOfSamples;

		private bool[] cachedNumerators;

		private long totalSetCachedNumerators;

		private long cachedDenominator;

		private long cachedValue;

		private int movingIndex;

		private object lockObject = new object();
	}
}
