using System;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class DayOfWeekTimeContext
	{
		private DayOfWeekTimeContext(ExDateTime dt, bool showDay, bool showTime)
		{
			this.DateTime = dt;
			this.ShowDay = showDay;
			this.ShowTime = showTime;
		}

		public bool ShowDay { get; private set; }

		public bool ShowTime { get; private set; }

		public ExDateTime DateTime { get; private set; }

		public static DayOfWeekTimeContext WithDayAndTime(ExDateTime dt)
		{
			return new DayOfWeekTimeContext(dt, true, true);
		}

		public static DayOfWeekTimeContext WithDayOnly(ExDateTime dt)
		{
			return new DayOfWeekTimeContext(dt, true, false);
		}

		public static DayOfWeekTimeContext WithTimeOnly(ExDateTime dt)
		{
			return new DayOfWeekTimeContext(dt, false, true);
		}
	}
}
