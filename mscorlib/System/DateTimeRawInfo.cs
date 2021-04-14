using System;
using System.Security;

namespace System
{
	internal struct DateTimeRawInfo
	{
		[SecurityCritical]
		internal unsafe void Init(int* numberBuffer)
		{
			this.month = -1;
			this.year = -1;
			this.dayOfWeek = -1;
			this.era = -1;
			this.timeMark = DateTimeParse.TM.NotSet;
			this.fraction = -1.0;
			this.num = numberBuffer;
		}

		[SecuritySafeCritical]
		internal unsafe void AddNumber(int value)
		{
			ref int ptr = ref *this.num;
			int num = this.numCount;
			this.numCount = num + 1;
			*(ref ptr + (IntPtr)num * 4) = value;
		}

		[SecuritySafeCritical]
		internal unsafe int GetNumber(int index)
		{
			return this.num[index];
		}

		[SecurityCritical]
		private unsafe int* num;

		internal int numCount;

		internal int month;

		internal int year;

		internal int dayOfWeek;

		internal int era;

		internal DateTimeParse.TM timeMark;

		internal double fraction;

		internal bool hasSameDateAndTimeSeparators;

		internal bool timeZone;
	}
}
