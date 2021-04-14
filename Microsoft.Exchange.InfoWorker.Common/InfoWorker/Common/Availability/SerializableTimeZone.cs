using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class SerializableTimeZone
	{
		public SerializableTimeZone()
		{
		}

		internal SerializableTimeZone(ExTimeZone timezone)
		{
			this.TimeZone = timezone;
		}

		[XmlElement]
		[DataMember]
		public int Bias
		{
			get
			{
				return this.bias;
			}
			set
			{
				if (value < -1440 || value > 1440)
				{
					throw new InvalidParameterException(Strings.descInvalidTimeZoneBias);
				}
				this.bias = value;
			}
		}

		[DataMember]
		[XmlElement]
		public SerializableTimeZoneTime StandardTime
		{
			get
			{
				return this.standardTime;
			}
			set
			{
				this.standardTime = value;
			}
		}

		[XmlElement]
		[DataMember]
		public SerializableTimeZoneTime DaylightTime
		{
			get
			{
				return this.daylightTime;
			}
			set
			{
				this.daylightTime = value;
			}
		}

		[XmlIgnore]
		internal ExTimeZone TimeZone
		{
			get
			{
				REG_TIMEZONE_INFO regInfo = default(REG_TIMEZONE_INFO);
				regInfo.Bias = this.Bias;
				if (this.StandardTime != null)
				{
					regInfo.StandardBias = this.StandardTime.Bias;
					regInfo.StandardDate = this.StandardTime.SystemTime;
				}
				if (this.DaylightTime != null)
				{
					regInfo.DaylightBias = this.DaylightTime.Bias;
					regInfo.DaylightDate = this.DaylightTime.SystemTime;
				}
				return TimeZoneHelper.CreateCustomExTimeZoneFromRegTimeZoneInfo(regInfo, "tzone://Microsoft/Custom", "Customized Time Zone");
			}
			set
			{
				REG_TIMEZONE_INFO reg_TIMEZONE_INFO = TimeZoneHelper.RegTimeZoneInfoFromExTimeZone(value);
				this.Bias = reg_TIMEZONE_INFO.Bias;
				this.standardTime = new SerializableTimeZoneTime(reg_TIMEZONE_INFO.StandardBias, reg_TIMEZONE_INFO.StandardDate);
				this.daylightTime = new SerializableTimeZoneTime(reg_TIMEZONE_INFO.DaylightBias, reg_TIMEZONE_INFO.DaylightDate);
			}
		}

		internal bool IsDynamicTimeZone()
		{
			return this.daylightTime.TransitionYear > 0 || this.standardTime.TransitionYear > 0;
		}

		public override string ToString()
		{
			return string.Format("Bias = {0}, [StandardTime = {1}], [DaylightTime = {2}]", this.bias, this.standardTime, this.daylightTime);
		}

		public const int MaximumNegativeBias = -1440;

		public const int MaximumPositiveBias = 1440;

		private int bias;

		private SerializableTimeZoneTime daylightTime;

		private SerializableTimeZoneTime standardTime;
	}
}
