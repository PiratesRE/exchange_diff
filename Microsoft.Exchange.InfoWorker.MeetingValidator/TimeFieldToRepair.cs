using System;

namespace Microsoft.Exchange.Infoworker.MeetingValidator
{
	[Flags]
	internal enum TimeFieldToRepair
	{
		None = 0,
		StartTime = 1,
		EndTime = 2,
		StartAndEndTime = 3
	}
}
