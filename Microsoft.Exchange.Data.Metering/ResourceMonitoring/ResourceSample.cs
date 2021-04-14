using System;

namespace Microsoft.Exchange.Data.Metering.ResourceMonitoring
{
	internal struct ResourceSample
	{
		public ResourceSample(UseLevel useLevel, long pressure)
		{
			this.useLevel = useLevel;
			this.pressure = pressure;
		}

		public UseLevel UseLevel
		{
			get
			{
				return this.useLevel;
			}
		}

		public long Pressure
		{
			get
			{
				return this.pressure;
			}
		}

		private readonly UseLevel useLevel;

		private readonly long pressure;
	}
}
