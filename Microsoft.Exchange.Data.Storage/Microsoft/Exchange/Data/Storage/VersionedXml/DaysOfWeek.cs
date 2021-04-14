﻿using System;

namespace Microsoft.Exchange.Data.Storage.VersionedXml
{
	[Flags]
	public enum DaysOfWeek
	{
		None = 0,
		Sunday = 1,
		Monday = 2,
		Tuesday = 4,
		Wednesday = 8,
		Thursday = 16,
		Friday = 32,
		Saturday = 64,
		Weekdays = 62,
		WeekendDays = 65,
		AllDays = 127
	}
}
