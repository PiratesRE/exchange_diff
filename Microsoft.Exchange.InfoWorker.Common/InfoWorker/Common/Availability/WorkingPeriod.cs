using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.InfoWorker.Common.Availability
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public class WorkingPeriod
	{
		public static DaysOfWeek DayToDays(DayOfWeek dow)
		{
			switch (dow)
			{
			case System.DayOfWeek.Sunday:
				return DaysOfWeek.Sunday;
			case System.DayOfWeek.Monday:
				return DaysOfWeek.Monday;
			case System.DayOfWeek.Tuesday:
				return DaysOfWeek.Tuesday;
			case System.DayOfWeek.Wednesday:
				return DaysOfWeek.Wednesday;
			case System.DayOfWeek.Thursday:
				return DaysOfWeek.Thursday;
			case System.DayOfWeek.Friday:
				return DaysOfWeek.Friday;
			case System.DayOfWeek.Saturday:
				return DaysOfWeek.Saturday;
			default:
				throw new ArgumentException("dow");
			}
		}

		[DataMember]
		public DaysOfWeek DayOfWeek
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

		[DataMember]
		public int StartTimeInMinutes
		{
			get
			{
				return this.startTimeInMinutes;
			}
			set
			{
				this.startTimeInMinutes = value;
			}
		}

		[DataMember]
		public int EndTimeInMinutes
		{
			get
			{
				return this.endTimeInMinutes;
			}
			set
			{
				this.endTimeInMinutes = value;
			}
		}

		public override string ToString()
		{
			if (this.toString == null)
			{
				this.toString = string.Concat(new object[]
				{
					"Day of week=",
					this.dayOfWeek,
					"Start time=",
					this.startTimeInMinutes,
					"End time=",
					this.endTimeInMinutes
				});
			}
			return this.toString;
		}

		public WorkingPeriod()
		{
		}

		internal WorkingPeriod(DaysOfWeek dayOfWeek, int startTimeInMinutes, int endTimeInMinutes)
		{
			this.dayOfWeek = dayOfWeek;
			this.startTimeInMinutes = startTimeInMinutes;
			this.endTimeInMinutes = endTimeInMinutes;
		}

		internal bool IsSameWorkDay(DaysOfWeek dayToTest)
		{
			return this.DayOfWeek == dayToTest;
		}

		private DaysOfWeek dayOfWeek;

		private int startTimeInMinutes;

		private int endTimeInMinutes;

		private string toString;
	}
}
