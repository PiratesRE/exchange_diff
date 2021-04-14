using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class SerializableTimeZoneTime
	{
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
				if (value < -720 || value > 720)
				{
					throw new InvalidParameterException(Strings.descInvalidTransitionBias(-720, 720));
				}
				this.bias = value;
			}
		}

		[XmlElement]
		[DataMember]
		public string Time
		{
			get
			{
				return string.Format("{0:00}:{1:00}:{2:00}", this.time.Hours, this.time.Minutes, this.time.Seconds);
			}
			set
			{
				if (!TimeSpan.TryParse(value, out this.time))
				{
					throw new InvalidParameterException(Strings.descInvalidTransitionTime);
				}
			}
		}

		[DataMember]
		[XmlElement]
		public short DayOrder
		{
			get
			{
				return this.dayOrder;
			}
			set
			{
				if (value < 0 || value > 31)
				{
					throw new InvalidParameterException(Strings.descInvalidDayOrder(0, 31));
				}
				this.dayOrder = value;
			}
		}

		[XmlElement]
		[DataMember]
		public short Month
		{
			get
			{
				return this.month;
			}
			set
			{
				if (value < 0 || value > 12)
				{
					throw new InvalidParameterException(Strings.descInvalidMonth(0, 12));
				}
				this.month = value;
			}
		}

		[DataMember]
		[XmlElement]
		public DayOfWeek DayOfWeek
		{
			get
			{
				return this.dayOfWeek;
			}
			set
			{
				this.dayOfWeek = value;
			}
		}

		[XmlElement(IsNullable = false)]
		[DataMember]
		public string Year
		{
			get
			{
				if (this.year != 0)
				{
					return Convert.ToString(this.year);
				}
				return null;
			}
			set
			{
				short num;
				if (!short.TryParse(value, out num))
				{
					throw new InvalidParameterException(Strings.descInvalidYear(1601, 4500));
				}
				if (num < 1601 || num > 4500)
				{
					throw new InvalidParameterException(Strings.descInvalidYear(1601, 4500));
				}
				this.year = num;
			}
		}

		public override string ToString()
		{
			return string.Format("Bias = {0}, Month = {1}, DayOrder = {2}, DayOfWeek = {3}, Time = {4}", new object[]
			{
				this.bias,
				this.month,
				this.dayOrder,
				this.dayOfWeek,
				this.time
			});
		}

		public SerializableTimeZoneTime()
		{
		}

		internal SerializableTimeZoneTime(int bias, NativeMethods.SystemTime systemTime)
		{
			this.bias = bias;
			this.month = (short)systemTime.Month;
			this.dayOrder = (short)systemTime.Day;
			this.dayOfWeek = (DayOfWeek)systemTime.DayOfWeek;
			this.year = (short)systemTime.Year;
			this.time = new TimeSpan((int)systemTime.Hour, (int)systemTime.Minute, (int)systemTime.Second);
		}

		internal NativeMethods.SystemTime SystemTime
		{
			get
			{
				return new NativeMethods.SystemTime
				{
					Year = (ushort)this.year,
					Month = (ushort)this.month,
					Day = (ushort)this.dayOrder,
					DayOfWeek = (ushort)this.DayOfWeek,
					Hour = (ushort)this.time.Hours,
					Minute = (ushort)this.time.Minutes,
					Second = (ushort)this.time.Seconds
				};
			}
		}

		[XmlIgnore]
		internal short TransitionYear
		{
			get
			{
				return this.year;
			}
		}

		public const int MaximumNegativeTransitionBias = -720;

		public const int MaximumPositiveTransitionBias = 720;

		public const int MinimumDayOrder = 0;

		public const int MaximumDayOrder = 31;

		public const int MinimumMonth = 0;

		public const int MaximumMonth = 12;

		public const int MinimumYear = 1601;

		public const int MaximumYear = 4500;

		private int bias;

		private short year;

		private short month;

		private short dayOrder;

		private DayOfWeek dayOfWeek;

		private TimeSpan time;
	}
}
