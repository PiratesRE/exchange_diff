using System;
using System.Globalization;

namespace Microsoft.Exchange.Data.Metering.ResourceMonitoring
{
	internal struct PressureTransitions
	{
		public PressureTransitions(long mediumToHigh, long highToMedium, long lowToMedium, long mediumToLow)
		{
			if (mediumToHigh <= highToMedium || highToMedium <= lowToMedium || lowToMedium <= mediumToLow)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The pressure transitions should define strict non-overlapping ranges : mediumToLow < lowToMedium < highToMedium < mediumToHigh : {0}, {1}, {2}, {3}", new object[]
				{
					mediumToLow,
					lowToMedium,
					highToMedium,
					mediumToHigh
				}));
			}
			this.mediumToHigh = mediumToHigh;
			this.highToMedium = highToMedium;
			this.lowToMedium = lowToMedium;
			this.mediumToLow = mediumToLow;
		}

		internal long MediumToHigh
		{
			get
			{
				return this.mediumToHigh;
			}
		}

		internal long HighToMedium
		{
			get
			{
				return this.highToMedium;
			}
		}

		internal long LowToMedium
		{
			get
			{
				return this.lowToMedium;
			}
		}

		internal long MediumToLow
		{
			get
			{
				return this.mediumToLow;
			}
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "[PressureTransitions: MediumToHigh={0} HighToMedium={1} LowToMedium={2} MediumToLow={3}]", new object[]
			{
				this.MediumToHigh,
				this.HighToMedium,
				this.LowToMedium,
				this.MediumToLow
			});
		}

		internal UseLevel GetUseLevel(long pressure, UseLevel previousUseLevel)
		{
			if (pressure > this.mediumToHigh)
			{
				return UseLevel.High;
			}
			if (PressureTransitions.Between(pressure, this.highToMedium, this.mediumToHigh))
			{
				if (previousUseLevel == UseLevel.High)
				{
					return UseLevel.High;
				}
				return UseLevel.Medium;
			}
			else
			{
				if (PressureTransitions.Between(pressure, this.lowToMedium, this.highToMedium))
				{
					return UseLevel.Medium;
				}
				if (PressureTransitions.Between(pressure, this.mediumToLow, this.lowToMedium) && (previousUseLevel == UseLevel.Medium || previousUseLevel == UseLevel.High))
				{
					return UseLevel.Medium;
				}
				return UseLevel.Low;
			}
		}

		private static bool Between(long number, long lowerLimit, long upperLimit)
		{
			return lowerLimit < number && number <= upperLimit;
		}

		private readonly long mediumToHigh;

		private readonly long highToMedium;

		private readonly long lowToMedium;

		private readonly long mediumToLow;
	}
}
