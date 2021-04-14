using System;

namespace Microsoft.Exchange.Monitoring
{
	public class DailyServiceAvailability : DailyAvailability
	{
		public DailyServiceAvailability(DateTime date) : base(date)
		{
		}

		public override string ToString()
		{
			return string.Format("{0} - {1:p2}", base.Date.ToString("d"), base.AvailabilityPercentage);
		}
	}
}
