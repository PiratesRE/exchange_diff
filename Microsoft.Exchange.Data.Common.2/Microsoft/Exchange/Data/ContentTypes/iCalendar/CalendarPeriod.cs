using System;
using System.Text;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.ContentTypes.Internal;

namespace Microsoft.Exchange.Data.ContentTypes.iCalendar
{
	public struct CalendarPeriod
	{
		public CalendarPeriod(DateTime start, DateTime end)
		{
			this.start = start;
			this.end = end;
			this.duration = start - end;
			this.isExplicitPeriod = true;
		}

		public CalendarPeriod(DateTime start, TimeSpan duration)
		{
			this.start = start;
			this.end = start + duration;
			this.duration = duration;
			this.isExplicitPeriod = false;
		}

		internal CalendarPeriod(string s, ComplianceTracker tracker)
		{
			int num = s.IndexOf('/');
			if (num <= 0 || s.Length - 1 == num)
			{
				tracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.InvalidTimeFormat);
				this.start = CalendarCommon.MinDateTime;
				this.end = CalendarCommon.MinDateTime;
				this.duration = TimeSpan.Zero;
				this.isExplicitPeriod = false;
				return;
			}
			DateTime dateTime = CalendarCommon.ParseDateTime(s.Substring(0, num), tracker);
			char c = s[num + 1];
			if (c == '+' || c == '-' || c == 'P')
			{
				TimeSpan t = CalendarCommon.ParseDuration(s.Substring(num + 1), tracker);
				this.start = dateTime;
				this.end = dateTime + t;
				this.duration = t;
				this.isExplicitPeriod = false;
				return;
			}
			DateTime d = CalendarCommon.ParseDateTime(s.Substring(num + 1), tracker);
			this.start = dateTime;
			this.end = d;
			this.duration = dateTime - d;
			this.isExplicitPeriod = true;
		}

		public DateTime Start
		{
			get
			{
				return this.start;
			}
			set
			{
				this.start = value;
			}
		}

		public DateTime End
		{
			get
			{
				return this.end;
			}
			set
			{
				this.end = value;
				this.isExplicitPeriod = true;
			}
		}

		public TimeSpan Duration
		{
			get
			{
				return this.duration;
			}
			set
			{
				this.duration = value;
				this.isExplicitPeriod = false;
			}
		}

		public bool IsExplicit
		{
			get
			{
				return this.isExplicitPeriod;
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(CalendarCommon.FormatDateTime(this.start));
			stringBuilder.Append('/');
			if (this.isExplicitPeriod)
			{
				stringBuilder.Append(CalendarCommon.FormatDateTime(this.end));
			}
			else
			{
				stringBuilder.Append(CalendarCommon.FormatDuration(this.duration));
			}
			return stringBuilder.ToString();
		}

		private DateTime start;

		private DateTime end;

		private TimeSpan duration;

		private bool isExplicitPeriod;
	}
}
