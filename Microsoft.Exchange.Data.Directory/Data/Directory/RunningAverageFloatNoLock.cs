using System;

namespace Microsoft.Exchange.Data.Directory
{
	internal class RunningAverageFloatNoLock
	{
		public RunningAverageFloatNoLock(ushort numberOfSamples)
		{
			if (numberOfSamples < 2)
			{
				throw new ArgumentException("numberOfSamples must be greater than 1", "numberOfSamples");
			}
			this.averageMultiplier = 1f / (float)numberOfSamples;
		}

		public float Update(float newValue)
		{
			if (this.initialized)
			{
				this.cachedValue = (1f - this.averageMultiplier) * this.cachedValue + this.averageMultiplier * newValue;
			}
			else
			{
				this.cachedValue = newValue;
				this.initialized = true;
			}
			return this.cachedValue;
		}

		public void Reset(float valueToResetTo)
		{
			this.cachedValue = valueToResetTo;
		}

		public float Value
		{
			get
			{
				return this.cachedValue;
			}
		}

		private volatile float cachedValue;

		private float averageMultiplier;

		private bool initialized;
	}
}
