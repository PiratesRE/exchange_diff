using System;
using System.Xml.Serialization;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class WorkHoursTimeZone
	{
		[XmlElement]
		public int Bias
		{
			get
			{
				return this.timeZoneInfo.Bias;
			}
			set
			{
				this.timeZoneInfo.Bias = value;
			}
		}

		[XmlElement]
		public ZoneTransition Standard
		{
			get
			{
				return new ZoneTransition(this.timeZoneInfo.StandardBias, this.timeZoneInfo.StandardDate);
			}
			set
			{
				this.timeZoneInfo.StandardBias = value.Bias;
				this.timeZoneInfo.StandardDate = value.ChangeDate.SystemTime;
			}
		}

		[XmlElement]
		public ZoneTransition DaylightSavings
		{
			get
			{
				return new ZoneTransition(this.timeZoneInfo.DaylightBias, this.timeZoneInfo.DaylightDate);
			}
			set
			{
				this.timeZoneInfo.DaylightBias = value.Bias;
				this.timeZoneInfo.DaylightDate = value.ChangeDate.SystemTime;
			}
		}

		[XmlElement]
		public string Name { get; set; }

		public WorkHoursTimeZone()
		{
		}

		internal REG_TIMEZONE_INFO TimeZoneInfo
		{
			get
			{
				return this.timeZoneInfo;
			}
		}

		internal WorkHoursTimeZone(ExTimeZone timeZone)
		{
			this.timeZoneInfo = TimeZoneHelper.RegTimeZoneInfoFromExTimeZone(timeZone);
			this.Name = timeZone.Id;
		}

		internal bool IsSameTimeZoneInfo(REG_TIMEZONE_INFO other)
		{
			if (other == this.timeZoneInfo)
			{
				return true;
			}
			REG_TIMEZONE_INFO v = this.timeZoneInfo;
			v.StandardDate.Milliseconds = 0;
			v.DaylightDate.Milliseconds = 0;
			REG_TIMEZONE_INFO v2 = other;
			v2.StandardDate.Milliseconds = 0;
			v2.DaylightDate.Milliseconds = 0;
			return v == v2;
		}

		private REG_TIMEZONE_INFO timeZoneInfo;
	}
}
