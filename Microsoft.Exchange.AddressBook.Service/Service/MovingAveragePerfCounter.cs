using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.AddressBook.Service
{
	internal class MovingAveragePerfCounter
	{
		public MovingAveragePerfCounter(IExPerformanceCounter perfCounter, int sampleSize)
		{
			this.perfCounter = perfCounter;
			this.samples = new long[sampleSize];
		}

		public void AddSample(long sample)
		{
			lock (this.samples)
			{
				this.total -= this.samples[this.nextSample];
				this.total += sample;
				this.samples[this.nextSample++] = sample;
				if (this.nextSample == this.samples.Length)
				{
					this.nextSample = 0;
				}
				if (this.samplesFilled != this.samples.Length)
				{
					this.samplesFilled++;
				}
				this.perfCounter.RawValue = this.total / (long)this.samplesFilled;
			}
		}

		private readonly IExPerformanceCounter perfCounter;

		private readonly long[] samples;

		private int samplesFilled;

		private int nextSample;

		private long total;
	}
}
