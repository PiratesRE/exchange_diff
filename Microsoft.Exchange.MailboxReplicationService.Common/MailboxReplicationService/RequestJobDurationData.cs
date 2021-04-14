using System;
using System.Text;
using System.Xml.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public class RequestJobDurationData
	{
		public RequestJobDurationData(RequestState state) : this()
		{
			this.state = state;
		}

		public RequestJobDurationData()
		{
			this.minutes = new PerMinuteTimeSlot();
			this.hours = new HourlyTimeSlot();
			this.days = new DailyTimeSlot();
			this.months = new MonthlyTimeSlot();
		}

		[XmlIgnore]
		public RequestState State
		{
			get
			{
				return this.state;
			}
			set
			{
				this.state = value;
			}
		}

		[XmlAttribute("S")]
		public int StateInt
		{
			get
			{
				return (int)this.state;
			}
			set
			{
				this.state = (RequestState)value;
			}
		}

		[XmlIgnore]
		public TimeSpan Duration
		{
			get
			{
				return this.duration;
			}
			set
			{
				this.duration = value;
			}
		}

		[XmlAttribute("D")]
		public long DurationTicks
		{
			get
			{
				return this.duration.Ticks;
			}
			set
			{
				this.duration = new TimeSpan(value);
			}
		}

		[XmlElement(ElementName = "Minutes")]
		public ulong[] Minutes
		{
			get
			{
				return this.minutes.ToArray();
			}
			set
			{
				this.minutes = new PerMinuteTimeSlot(value);
			}
		}

		[XmlElement(ElementName = "Hours")]
		public ulong[] Hours
		{
			get
			{
				return this.hours.ToArray();
			}
			set
			{
				this.hours = new HourlyTimeSlot(value);
			}
		}

		[XmlElement(ElementName = "Days")]
		public ulong[] Days
		{
			get
			{
				return this.days.ToArray();
			}
			set
			{
				this.days = new DailyTimeSlot(value);
			}
		}

		[XmlElement(ElementName = "Months")]
		public ulong[] Months
		{
			get
			{
				return this.months.ToArray();
			}
			set
			{
				this.months = new MonthlyTimeSlot(value);
			}
		}

		public static RequestJobDurationData operator +(RequestJobDurationData data1, RequestJobDurationData data2)
		{
			if (data1 != null && data2 == null)
			{
				return data1;
			}
			if (data2 != null && data1 == null)
			{
				return data2;
			}
			RequestJobDurationData requestJobDurationData = new RequestJobDurationData();
			requestJobDurationData.Duration = data1.Duration + data2.Duration;
			requestJobDurationData.minutes.PopulateFrom(data1.minutes, data2.minutes);
			requestJobDurationData.hours.PopulateFrom(data1.hours, data2.hours);
			requestJobDurationData.days.PopulateFrom(data1.days, data2.days);
			requestJobDurationData.months.PopulateFrom(data1.months, data2.months);
			return requestJobDurationData;
		}

		public void Refresh(DateTime lastUpdateTime)
		{
			this.minutes.Refresh(lastUpdateTime);
			this.hours.Refresh(lastUpdateTime);
			this.days.Refresh(lastUpdateTime);
			this.months.Refresh(lastUpdateTime);
		}

		public void AddTime(TimeSpan duration)
		{
			this.Duration += duration;
			this.minutes.AddTime(duration);
			this.hours.AddTime(duration);
			this.days.AddTime(duration);
			this.months.AddTime(duration);
		}

		public RequestJobTimeTrackerXML.DurationRec GetDurationRec(RequestState state, bool showTimeSlots = false)
		{
			RequestJobTimeTrackerXML.DurationRec durationRec = new RequestJobTimeTrackerXML.DurationRec();
			durationRec.State = state.ToString();
			durationRec.Duration = this.duration.ToString();
			if (showTimeSlots)
			{
				durationRec.PerMinute = this.minutes.GetDiagnosticXML();
				durationRec.PerHour = this.hours.GetDiagnosticXML();
				durationRec.PerDay = this.days.GetDiagnosticXML();
				durationRec.PerMonth = this.months.GetDiagnosticXML();
			}
			return durationRec;
		}

		public override string ToString()
		{
			long ticks = this.Duration.Ticks;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(new TimeSpan(ticks - ticks % 10000000L).ToString());
			if (this.Minutes.Length > 0)
			{
				stringBuilder.AppendFormat(" Minutes:{0}", string.Join("|", new object[]
				{
					this.minutes
				}));
			}
			if (this.Hours.Length > 0)
			{
				stringBuilder.AppendFormat(" Hours:{0}", string.Join("|", new object[]
				{
					this.hours
				}));
			}
			if (this.Days.Length > 0)
			{
				stringBuilder.AppendFormat(" Days:{0}", string.Join("|", new object[]
				{
					this.days
				}));
			}
			if (this.Months.Length > 0)
			{
				stringBuilder.AppendFormat(" Months:{0}", string.Join("|", new object[]
				{
					this.months
				}));
			}
			return stringBuilder.ToString();
		}

		private RequestState state;

		private TimeSpan duration;

		private TimeSlot minutes;

		private TimeSlot hours;

		private TimeSlot days;

		private TimeSlot months;
	}
}
