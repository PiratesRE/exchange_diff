using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class TimeOfDay
	{
		public TimeOfDayType TimeOfDayType { get; set; }

		public DaysOfWeek CustomPeriodDays { get; set; }

		public TimeSpan? CustomPeriodStartTime { get; set; }

		public TimeSpan? CustomPeriodEndTime { get; set; }

		private TimeOfDay()
		{
		}

		public TimeOfDay(TimeOfDayType timeOfDayType, DaysOfWeek daysOfWeek, TimeSpan? startTime, TimeSpan? endTime)
		{
			this.TimeOfDayType = timeOfDayType;
			this.CustomPeriodDays = daysOfWeek;
			this.CustomPeriodStartTime = startTime;
			this.CustomPeriodEndTime = endTime;
			this.Validate();
		}

		public static TimeOfDay Parse(string timeOfDay)
		{
			if (string.IsNullOrEmpty(timeOfDay))
			{
				throw new FormatException(DataStrings.InvalidTimeOfDayFormat);
			}
			string[] array = timeOfDay.Split(new char[]
			{
				','
			});
			if (array == null || array.Length != 4)
			{
				throw new FormatException(DataStrings.InvalidTimeOfDayFormat);
			}
			TimeOfDayType timeOfDayType = (TimeOfDayType)CallerIdItem.ValidateEnumValue(array[0], "TimeOfDayType", 1, 3);
			DaysOfWeek daysOfWeek = (DaysOfWeek)CallerIdItem.ValidateEnumValue(array[1], "DaysOfWeek", 0, 127);
			switch (timeOfDayType)
			{
			case TimeOfDayType.WorkingHours:
			case TimeOfDayType.NonWorkingHours:
				if (daysOfWeek != DaysOfWeek.None || !string.IsNullOrEmpty(array[2]) || !string.IsNullOrEmpty(array[3]))
				{
					throw new FormatException(DataStrings.InvalidTimeOfDayFormatWorkingHours);
				}
				return new TimeOfDay(timeOfDayType, daysOfWeek, null, null);
			case TimeOfDayType.CustomPeriod:
				if (string.IsNullOrEmpty(array[2]) || string.IsNullOrEmpty(array[3]))
				{
					throw new FormatException(DataStrings.InvalidTimeOfDayFormatCustomWorkingHours);
				}
				return new TimeOfDay(timeOfDayType, daysOfWeek, new TimeSpan?(TimeSpan.Parse(array[2])), new TimeSpan?(TimeSpan.Parse(array[3])));
			default:
				throw new Exception("Unknown enum type.");
			}
		}

		public void Validate()
		{
			switch (this.TimeOfDayType)
			{
			case TimeOfDayType.WorkingHours:
			case TimeOfDayType.NonWorkingHours:
				if (this.CustomPeriodDays != DaysOfWeek.None || this.CustomPeriodStartTime != null || this.CustomPeriodEndTime != null)
				{
					throw new FormatException(DataStrings.InvalidTimeOfDayFormatWorkingHours);
				}
				break;
			case TimeOfDayType.CustomPeriod:
				if (this.CustomPeriodStartTime == null || this.CustomPeriodEndTime == null || this.CustomPeriodDays == DaysOfWeek.None || this.CustomPeriodStartTime.Value > this.CustomPeriodEndTime.Value || this.CustomPeriodStartTime.Value < TimeSpan.FromDays(0.0) || this.CustomPeriodStartTime.Value > TimeSpan.FromDays(1.0) - TimeSpan.FromTicks(1L) || this.CustomPeriodEndTime.Value < TimeSpan.FromDays(0.0) || this.CustomPeriodEndTime.Value > TimeSpan.FromDays(1.0) - TimeSpan.FromTicks(1L) || this.CustomPeriodStartTime == this.CustomPeriodEndTime)
				{
					throw new FormatException(DataStrings.InvalidTimeOfDayFormatCustomWorkingHours);
				}
				break;
			default:
				throw new Exception("Unknown TimeOfDayType");
			}
		}

		public override string ToString()
		{
			return string.Format("{0},{1},{2},{3}", new object[]
			{
				(int)this.TimeOfDayType,
				(int)this.CustomPeriodDays,
				(this.CustomPeriodStartTime != null) ? this.CustomPeriodStartTime.Value.ToString() : string.Empty,
				(this.CustomPeriodEndTime != null) ? this.CustomPeriodEndTime.Value.ToString() : string.Empty
			});
		}

		private const int RequiredNumberOfTokens = 4;
	}
}
