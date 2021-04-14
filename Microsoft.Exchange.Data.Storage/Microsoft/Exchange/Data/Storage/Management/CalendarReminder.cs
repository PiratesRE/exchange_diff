using System;

namespace Microsoft.Exchange.Data.Storage.Management
{
	public enum CalendarReminder
	{
		[LocDescription(ServerStrings.IDs.ZeroMinutes)]
		ZeroMinutes,
		[LocDescription(ServerStrings.IDs.FiveMinutes)]
		FiveMinutes = 5,
		[LocDescription(ServerStrings.IDs.TenMinutes)]
		TenMinutes = 10,
		[LocDescription(ServerStrings.IDs.FifteenMinutes)]
		FifteenMinutes = 15,
		[LocDescription(ServerStrings.IDs.ThirtyMinutes)]
		ThirtyMinutes = 30,
		[LocDescription(ServerStrings.IDs.OneHours)]
		OneHours = 60,
		[LocDescription(ServerStrings.IDs.TwoHours)]
		TwoHours = 120,
		[LocDescription(ServerStrings.IDs.ThreeHours)]
		ThreeHours = 180,
		[LocDescription(ServerStrings.IDs.FourHours)]
		FourHours = 240,
		[LocDescription(ServerStrings.IDs.EightHours)]
		EightHours = 480,
		[LocDescription(ServerStrings.IDs.TwelveHours)]
		TwelveHours = 720,
		[LocDescription(ServerStrings.IDs.OneDays)]
		OneDays = 1440,
		[LocDescription(ServerStrings.IDs.TwoDays)]
		TwoDays = 2880,
		[LocDescription(ServerStrings.IDs.ThreeDays)]
		ThreeDays = 4320,
		[LocDescription(ServerStrings.IDs.OneWeeks)]
		OneWeeks = 10080,
		[LocDescription(ServerStrings.IDs.TwoWeeks)]
		TwoWeeks = 20160
	}
}
