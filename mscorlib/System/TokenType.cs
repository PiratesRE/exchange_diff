using System;

namespace System
{
	internal enum TokenType
	{
		NumberToken = 1,
		YearNumberToken,
		Am,
		Pm,
		MonthToken,
		EndOfString,
		DayOfWeekToken,
		TimeZoneToken,
		EraToken,
		DateWordToken,
		UnknownToken,
		HebrewNumber,
		JapaneseEraToken,
		TEraToken,
		IgnorableSymbol,
		SEP_Unk = 256,
		SEP_End = 512,
		SEP_Space = 768,
		SEP_Am = 1024,
		SEP_Pm = 1280,
		SEP_Date = 1536,
		SEP_Time = 1792,
		SEP_YearSuff = 2048,
		SEP_MonthSuff = 2304,
		SEP_DaySuff = 2560,
		SEP_HourSuff = 2816,
		SEP_MinuteSuff = 3072,
		SEP_SecondSuff = 3328,
		SEP_LocalTimeMark = 3584,
		SEP_DateOrOffset = 3840,
		RegularTokenMask = 255,
		SeparatorTokenMask = 65280
	}
}
