using System;

namespace Microsoft.Exchange.Data.Storage
{
	internal enum ReminderTimeHint
	{
		LaterToday,
		Tomorrow,
		TomorrowMorning,
		TomorrowAfternoon,
		TomorrowEvening,
		ThisWeekend,
		NextWeek,
		NextMonth,
		Someday,
		Custom,
		Now,
		InTwoDays
	}
}
