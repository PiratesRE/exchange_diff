using System;

namespace Microsoft.Exchange.Monitoring
{
	public class DailyPhysicalAvailability : DailyAvailability
	{
		public DailyPhysicalAvailability(DateTime date) : base(date)
		{
		}

		public double RawAvailabilityPercentage
		{
			get
			{
				return this.rawAvailabilityPercentage;
			}
			internal set
			{
				this.rawAvailabilityPercentage = value;
			}
		}

		public override string ToString()
		{
			return string.Format("{0} - {1:p2} ({2:p2})", base.Date.ToString("d"), base.AvailabilityPercentage, this.RawAvailabilityPercentage);
		}

		private double rawAvailabilityPercentage;
	}
}
