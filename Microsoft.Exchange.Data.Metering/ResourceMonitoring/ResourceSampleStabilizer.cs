using System;
using System.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Metering.ResourceMonitoring
{
	internal class ResourceSampleStabilizer
	{
		public ResourceSampleStabilizer(int maxSamples, ResourceSample initialSample)
		{
			ArgumentValidator.ThrowIfInvalidValue<int>("maxSamples", maxSamples, (int count) => count > 1);
			this.samples = new ResourceSample[maxSamples];
			for (int i = 0; i < maxSamples; i++)
			{
				this.samples[i] = new ResourceSample(initialSample.UseLevel, initialSample.Pressure);
			}
			this.useLevelSampleCounts[(int)initialSample.UseLevel] = maxSamples;
		}

		public ResourceSample GetStabilizedSample(ResourceSample sample)
		{
			this.AddSample(sample);
			return this.CalculateStableSample();
		}

		public void AddSample(ResourceSample sample)
		{
			this.currentIndex = (this.currentIndex + 1) % this.samples.Length;
			ResourceSample resourceSample = this.samples[this.currentIndex];
			this.useLevelSampleCounts[(int)resourceSample.UseLevel]--;
			this.samples[this.currentIndex] = sample;
			this.useLevelSampleCounts[(int)sample.UseLevel]++;
		}

		public int GetUseLevelPercentage(UseLevel useLevel)
		{
			return this.useLevelSampleCounts[(int)useLevel] * 100 / this.samples.Length;
		}

		private ResourceSample CalculateStableSample()
		{
			UseLevel stableUseLevel = UseLevel.Low;
			for (int i = 0; i < this.useLevelSampleCounts.Length; i++)
			{
				if (this.useLevelSampleCounts[i] != 0)
				{
					stableUseLevel = (UseLevel)i;
					break;
				}
			}
			long pressure = (long)(from sample in this.samples
			where sample.UseLevel == stableUseLevel
			select sample).Average((ResourceSample sample) => sample.Pressure);
			return new ResourceSample(stableUseLevel, pressure);
		}

		private readonly ResourceSample[] samples;

		private readonly int[] useLevelSampleCounts = new int[Enum.GetNames(typeof(UseLevel)).Length];

		private int currentIndex;
	}
}
