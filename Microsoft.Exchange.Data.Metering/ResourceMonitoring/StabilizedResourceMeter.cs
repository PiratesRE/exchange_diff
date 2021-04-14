using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Metering.ResourceMonitoring
{
	internal class StabilizedResourceMeter : IResourceMeter
	{
		public StabilizedResourceMeter(IResourceMeter rawResourceMeter, int sampleCount)
		{
			ArgumentValidator.ThrowIfNull("rawResourceMeter", rawResourceMeter);
			ArgumentValidator.ThrowIfInvalidValue<int>("sampleCount", sampleCount, (int count) => count > 1);
			this.rawResourceMeter = rawResourceMeter;
			this.rawResourceMeter.Refresh();
			ResourceSample initialSample = new ResourceSample(this.rawResourceMeter.ResourceUse.CurrentUseLevel, this.rawResourceMeter.Pressure);
			this.stabilizer = new ResourceSampleStabilizer(sampleCount, initialSample);
			this.pressure = this.rawResourceMeter.Pressure;
			this.resourceUse = new ResourceUse(this.rawResourceMeter.Resource, this.rawResourceMeter.ResourceUse.CurrentUseLevel, this.rawResourceMeter.ResourceUse.PreviousUseLevel);
		}

		public long Pressure
		{
			get
			{
				return this.pressure;
			}
		}

		public PressureTransitions PressureTransitions
		{
			get
			{
				return this.rawResourceMeter.PressureTransitions;
			}
		}

		public ResourceIdentifier Resource
		{
			get
			{
				return this.rawResourceMeter.Resource;
			}
		}

		public ResourceUse ResourceUse
		{
			get
			{
				return this.resourceUse;
			}
		}

		public ResourceUse RawResourceUse
		{
			get
			{
				return this.rawResourceMeter.RawResourceUse;
			}
		}

		public void Refresh()
		{
			this.rawResourceMeter.Refresh();
			ResourceSample sample = new ResourceSample(this.rawResourceMeter.ResourceUse.CurrentUseLevel, this.rawResourceMeter.Pressure);
			ResourceSample stabilizedSample = this.stabilizer.GetStabilizedSample(sample);
			ResourceUse resourceUse = new ResourceUse(this.Resource, stabilizedSample.UseLevel, this.resourceUse.CurrentUseLevel);
			this.resourceUse = resourceUse;
			this.pressure = stabilizedSample.Pressure;
		}

		private readonly IResourceMeter rawResourceMeter;

		private readonly ResourceSampleStabilizer stabilizer;

		private ResourceUse resourceUse;

		private long pressure;
	}
}
