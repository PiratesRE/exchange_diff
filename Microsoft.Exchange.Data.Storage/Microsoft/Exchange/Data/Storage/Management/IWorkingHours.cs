using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IWorkingHours
	{
		DaysOfWeek WorkDays { get; set; }

		TimeSpan WorkingHoursStartTime { get; set; }

		TimeSpan WorkingHoursEndTime { get; set; }

		ExTimeZoneValue WorkingHoursTimeZone { get; set; }
	}
}
