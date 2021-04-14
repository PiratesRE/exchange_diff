using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Common.Availability;

namespace Microsoft.Exchange.InfoWorker.Common.MeetingSuggestions
{
	public class AttendeeWorkHours
	{
		public AttendeeWorkHours(WorkingHours workingHours)
		{
			if (workingHours != null)
			{
				WorkingPeriod workingPeriod = workingHours.WorkingPeriodArray[0];
				TimeSpan timeSpan = TimeSpan.FromMinutes((double)workingPeriod.StartTimeInMinutes);
				TimeSpan timeSpan2 = TimeSpan.FromMinutes((double)workingPeriod.EndTimeInMinutes);
				AttendeeWorkHours.Validate(timeSpan, timeSpan2);
				this.startTime = timeSpan;
				this.endTime = timeSpan2;
				this.daysOfWeek = workingPeriod.DayOfWeek;
				this.timeZone = workingHours.ExTimeZone;
			}
			else
			{
				this.startTime = TimeSpan.Zero;
				this.endTime = TimeSpan.Zero;
				this.daysOfWeek = (DaysOfWeek.Sunday | DaysOfWeek.Monday | DaysOfWeek.Tuesday | DaysOfWeek.Wednesday | DaysOfWeek.Thursday | DaysOfWeek.Friday | DaysOfWeek.Saturday);
				this.timeZone = ExTimeZone.CurrentTimeZone;
			}
			this.CalculateWorkDayInconvenience();
		}

		internal AttendeeWorkHours(TimeSpan startTime, TimeSpan endTime, DaysOfWeek daysOfWeek) : this(startTime, endTime, daysOfWeek, ExTimeZone.CurrentTimeZone)
		{
		}

		internal AttendeeWorkHours(TimeSpan startTime, TimeSpan endTime, DaysOfWeek daysOfWeek, ExTimeZone exTimeZone)
		{
			AttendeeWorkHours.Validate(startTime, endTime);
			this.startTime = startTime;
			this.endTime = endTime;
			this.daysOfWeek = daysOfWeek;
			this.timeZone = exTimeZone;
			this.CalculateWorkDayInconvenience();
		}

		private static void Validate(TimeSpan startTime, TimeSpan endTime)
		{
			if (startTime < TimeSpan.Zero || startTime > TimeConstants.OneDay)
			{
				throw new InvalidParameterException(Strings.descWorkHoursStartTimeInvalid);
			}
			if (endTime < TimeSpan.Zero || endTime > TimeConstants.OneDay)
			{
				throw new InvalidParameterException(Strings.descWorkHoursEndTimeInvalid);
			}
			if (startTime > endTime)
			{
				throw new InvalidParameterException(Strings.descWorkHoursStartEndInvalid);
			}
		}

		public bool IsWorkTime(ExDateTime startUtc, ExDateTime endUtc)
		{
			ExDateTime exDateTime = this.timeZone.ConvertDateTime(startUtc);
			ExDateTime exDateTime2 = this.timeZone.ConvertDateTime(endUtc);
			if (this.IsWorkDay(exDateTime.DayOfWeek))
			{
				if (this.startTime == this.endTime && this.IsWorkDay(exDateTime2.DayOfWeek))
				{
					return true;
				}
				if (exDateTime.TimeOfDay >= this.startTime && exDateTime.DayOfWeek == exDateTime2.DayOfWeek && exDateTime2.TimeOfDay <= this.endTime)
				{
					return true;
				}
			}
			return false;
		}

		private bool IsWorkDay(DayOfWeek dayOfWeek)
		{
			switch (dayOfWeek)
			{
			case DayOfWeek.Sunday:
				return (this.daysOfWeek & DaysOfWeek.Sunday) == DaysOfWeek.Sunday;
			case DayOfWeek.Monday:
				return (this.daysOfWeek & DaysOfWeek.Monday) == DaysOfWeek.Monday;
			case DayOfWeek.Tuesday:
				return (this.daysOfWeek & DaysOfWeek.Tuesday) == DaysOfWeek.Tuesday;
			case DayOfWeek.Wednesday:
				return (this.daysOfWeek & DaysOfWeek.Wednesday) == DaysOfWeek.Wednesday;
			case DayOfWeek.Thursday:
				return (this.daysOfWeek & DaysOfWeek.Thursday) == DaysOfWeek.Thursday;
			case DayOfWeek.Friday:
				return (this.daysOfWeek & DaysOfWeek.Friday) == DaysOfWeek.Friday;
			case DayOfWeek.Saturday:
				return (this.daysOfWeek & DaysOfWeek.Saturday) == DaysOfWeek.Saturday;
			default:
				return false;
			}
		}

		internal int GetConvenience(ExDateTime startUtc, int duration)
		{
			ExDateTime exDateTime = this.timeZone.ConvertDateTime(startUtc);
			int num = 0;
			int num2 = exDateTime.Hour * 2 + exDateTime.Minute / 30;
			int num3 = num2 + duration / 30 - 1;
			int num4 = 0;
			for (int i = num2; i <= num3; i++)
			{
				if (i >= 48)
				{
					num3 -= 48;
					i -= 48;
				}
				num += (int)this.inconvenienceValues[i];
				if ((int)this.inconvenienceValues[i] > num4)
				{
					num4 = (int)this.inconvenienceValues[i];
				}
			}
			return num4;
		}

		private void CalculateWorkDayInconvenience()
		{
			int num = this.startTime.Hours * 2 + this.startTime.Minutes / 30;
			int num2 = this.endTime.Hours * 2 - 1 + this.endTime.Minutes / 30;
			int num3 = 48 - (num2 - num + 1);
			int num4 = (num3 + 1) / 2;
			int i;
			for (i = num; i <= num2; i++)
			{
				this.inconvenienceValues[i] = 0;
			}
			int num5 = 1;
			int num6 = (num3 % 2 == 0) ? num4 : (num4 - 1);
			i = num2 + 1;
			for (int num7 = i % 48; num7 != num; num7 = i % 48)
			{
				if (num5 <= num4)
				{
					this.inconvenienceValues[num7] = (byte)num5++;
				}
				else
				{
					this.inconvenienceValues[num7] = (byte)num6--;
				}
				i++;
			}
		}

		private const byte MaximumInconvenienceValue = 48;

		private const byte NumberOfInconvenienceSlots = 48;

		private DaysOfWeek daysOfWeek;

		private TimeSpan startTime;

		private TimeSpan endTime;

		private ExTimeZone timeZone;

		private byte[] inconvenienceValues = new byte[48];
	}
}
