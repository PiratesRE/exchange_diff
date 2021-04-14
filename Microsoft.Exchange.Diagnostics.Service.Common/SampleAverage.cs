using System;

namespace Microsoft.Exchange.Diagnostics.Service.Common
{
	internal class SampleAverage
	{
		public SampleAverage(int numSamples)
		{
			this.numSamples = numSamples;
			this.samples = new int[numSamples];
			this.lastIndex = -1;
		}

		public double AverageValue
		{
			get
			{
				return this.averageValue;
			}
		}

		public int? LastValue
		{
			get
			{
				if (this.lastIndex != -1)
				{
					return new int?(this.samples[this.lastIndex]);
				}
				return null;
			}
		}

		public double AddNewSample(double newValue)
		{
			this.lastIndex = (this.lastIndex + 1) % this.numSamples;
			if (this.sampleCount < this.numSamples)
			{
				this.averageValue = ((this.sampleCount == 0) ? newValue : ((this.averageValue * (double)this.sampleCount + newValue) / (double)(this.sampleCount + 1)));
				this.sampleCount++;
			}
			else
			{
				this.averageValue += (newValue - (double)this.samples[this.lastIndex]) / (double)this.numSamples;
			}
			this.samples[this.lastIndex] = (int)newValue;
			return this.averageValue;
		}

		private readonly int numSamples;

		private readonly int[] samples;

		private int lastIndex;

		private int sampleCount;

		private double averageValue;
	}
}
