using System;

namespace Microsoft.Exchange.Diagnostics
{
	public class RunningAverageFloat
	{
		public RunningAverageFloat(ushort numberOfSamples)
		{
			if (numberOfSamples < 2)
			{
				throw new ArgumentException("numberOfSamples must be greater than 1", "numberOfSamples");
			}
			this.averageMultiplier = 1f / (float)numberOfSamples;
		}

		public float Update(float newValue)
		{
			float result;
			lock (this.lockObject)
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
				result = this.cachedValue;
			}
			return result;
		}

		public void Reset(float valueToResetTo)
		{
			lock (this.lockObject)
			{
				this.cachedValue = valueToResetTo;
			}
		}

		public float Value
		{
			get
			{
				return this.cachedValue;
			}
		}

		private volatile float cachedValue;

		private object lockObject = new object();

		private readonly float averageMultiplier;

		private bool initialized;
	}
}
