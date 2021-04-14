using System;
using System.Collections.Generic;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class ScheduleGroup
	{
		internal ScheduleGroup(List<ExDateTime> dayOfWeekList, List<TimeRange> scheduleIntervalList)
		{
			this.dayOfWeekList = dayOfWeekList;
			this.scheduleIntervalList = scheduleIntervalList;
		}

		internal List<ExDateTime> DaysOfWeek
		{
			get
			{
				return this.dayOfWeekList;
			}
		}

		internal List<TimeRange> ScheduleIntervals
		{
			get
			{
				return this.scheduleIntervalList;
			}
		}

		private List<ExDateTime> dayOfWeekList;

		private List<TimeRange> scheduleIntervalList;
	}
}
