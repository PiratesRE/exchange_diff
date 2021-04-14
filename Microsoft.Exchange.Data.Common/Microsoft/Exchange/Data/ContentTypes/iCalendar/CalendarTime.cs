using System;
using System.Globalization;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.ContentTypes.Internal;

namespace Microsoft.Exchange.Data.ContentTypes.iCalendar
{
	public struct CalendarTime
	{
		public CalendarTime(TimeSpan time, bool isUtc)
		{
			this.time = time;
			this.isUtc = isUtc;
		}

		internal CalendarTime(string s, ComplianceTracker tracker)
		{
			this.isUtc = false;
			if (s.Length != 6 && s.Length != 7)
			{
				tracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.InvalidTimeStringLength);
				this.time = TimeSpan.Zero;
				return;
			}
			if (s.Length == 7)
			{
				if (s[6] != 'Z')
				{
					tracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.ExpectedZ);
					this.time = TimeSpan.Zero;
					return;
				}
				this.isUtc = true;
				s = s.Substring(0, 6);
			}
			DateTime dateTime;
			if (!DateTime.TryParseExact(s, "HHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
			{
				tracker.SetComplianceStatus(ComplianceStatus.InvalidValueFormat, CalendarStrings.InvalidTimeFormat);
				this.time = TimeSpan.Zero;
				return;
			}
			this.time = new TimeSpan(dateTime.Hour, dateTime.Minute, dateTime.Second);
		}

		public TimeSpan Time
		{
			get
			{
				return this.time;
			}
			set
			{
				this.time = value;
			}
		}

		public bool IsUtc
		{
			get
			{
				return this.isUtc;
			}
			set
			{
				this.isUtc = value;
			}
		}

		public override string ToString()
		{
			return new DateTime(1, 1, 1, this.time.Hours, this.time.Minutes, this.time.Seconds).ToString(this.isUtc ? "HHmmss\\Z" : "HHmmss");
		}

		private const string TimeFormatUtc = "HHmmss\\Z";

		private const string TimeFormat = "HHmmss";

		private TimeSpan time;

		private bool isUtc;
	}
}
