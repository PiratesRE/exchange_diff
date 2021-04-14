using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.ControlPanel.DataContracts
{
	[DataContract]
	public class TimeOfDayItem
	{
		public TimeOfDayItem()
		{
		}

		public TimeOfDayItem(TimeOfDay taskTimeOfDay)
		{
			this.CustomPeriodDays = (int)taskTimeOfDay.CustomPeriodDays;
			this.CustomPeriodEndTime = ((taskTimeOfDay.CustomPeriodEndTime != null) ? taskTimeOfDay.CustomPeriodEndTime.Value.TotalMinutes.ToString() : null);
			this.CustomPeriodStartTime = ((taskTimeOfDay.CustomPeriodStartTime != null) ? taskTimeOfDay.CustomPeriodStartTime.Value.TotalMinutes.ToString() : null);
			this.TimeOfDayType = (int)taskTimeOfDay.TimeOfDayType;
		}

		[DataMember]
		public int CustomPeriodDays { get; set; }

		[DataMember]
		public string CustomPeriodEndTime { get; set; }

		[DataMember]
		public string CustomPeriodStartTime { get; set; }

		[DataMember]
		public int TimeOfDayType { get; set; }

		public TimeOfDay ToTaskObject()
		{
			TimeOfDayType timeOfDayType = (TimeOfDayType)this.TimeOfDayType;
			DaysOfWeek customPeriodDays = (DaysOfWeek)this.CustomPeriodDays;
			if (timeOfDayType == Microsoft.Exchange.Data.TimeOfDayType.CustomPeriod)
			{
				int num = 0;
				int num2 = 0;
				int.TryParse(this.CustomPeriodStartTime, out num);
				int.TryParse(this.CustomPeriodEndTime, out num2);
				TimeSpan value = TimeSpan.FromMinutes((double)num);
				TimeSpan value2 = TimeSpan.FromMinutes((double)num2);
				return new TimeOfDay(timeOfDayType, customPeriodDays, new TimeSpan?(value), new TimeSpan?(value2));
			}
			return new TimeOfDay(timeOfDayType, customPeriodDays, null, null);
		}
	}
}
