using System;
using System.Globalization;

namespace System
{
	internal struct ParsingInfo
	{
		internal void Init()
		{
			this.dayOfWeek = -1;
			this.timeMark = DateTimeParse.TM.NotSet;
		}

		internal Calendar calendar;

		internal int dayOfWeek;

		internal DateTimeParse.TM timeMark;

		internal bool fUseHour12;

		internal bool fUseTwoDigitYear;

		internal bool fAllowInnerWhite;

		internal bool fAllowTrailingWhite;

		internal bool fCustomNumberParser;

		internal DateTimeParse.MatchNumberDelegate parseNumberDelegate;
	}
}
