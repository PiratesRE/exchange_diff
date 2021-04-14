﻿using System;
using System.Globalization;

namespace System
{
	internal struct DateTimeResult
	{
		internal void Init()
		{
			this.Year = -1;
			this.Month = -1;
			this.Day = -1;
			this.fraction = -1.0;
			this.era = -1;
		}

		internal void SetDate(int year, int month, int day)
		{
			this.Year = year;
			this.Month = month;
			this.Day = day;
		}

		internal void SetFailure(ParseFailureKind failure, string failureMessageID, object failureMessageFormatArgument)
		{
			this.failure = failure;
			this.failureMessageID = failureMessageID;
			this.failureMessageFormatArgument = failureMessageFormatArgument;
		}

		internal void SetFailure(ParseFailureKind failure, string failureMessageID, object failureMessageFormatArgument, string failureArgumentName)
		{
			this.failure = failure;
			this.failureMessageID = failureMessageID;
			this.failureMessageFormatArgument = failureMessageFormatArgument;
			this.failureArgumentName = failureArgumentName;
		}

		internal int Year;

		internal int Month;

		internal int Day;

		internal int Hour;

		internal int Minute;

		internal int Second;

		internal double fraction;

		internal int era;

		internal ParseFlags flags;

		internal TimeSpan timeZoneOffset;

		internal Calendar calendar;

		internal DateTime parsedDate;

		internal ParseFailureKind failure;

		internal string failureMessageID;

		internal object failureMessageFormatArgument;

		internal string failureArgumentName;
	}
}
