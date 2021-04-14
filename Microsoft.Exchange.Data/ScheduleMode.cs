using System;

namespace Microsoft.Exchange.Data
{
	public enum ScheduleMode
	{
		[LocDescription(DataStrings.IDs.ScheduleModeNever)]
		Never,
		[LocDescription(DataStrings.IDs.ScheduleModeScheduledTimes)]
		ScheduledTimes,
		[LocDescription(DataStrings.IDs.ScheduleModeAlways)]
		Always
	}
}
