using System;

namespace System.Globalization
{
	internal enum FORMATFLAGS
	{
		None,
		UseGenitiveMonth,
		UseLeapYearMonth,
		UseSpacesInMonthNames = 4,
		UseHebrewParsing = 8,
		UseSpacesInDayNames = 16,
		UseDigitPrefixInTokens = 32
	}
}
