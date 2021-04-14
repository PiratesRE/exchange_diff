using System;
using Microsoft.Exchange.Common;

namespace Microsoft.Exchange.Data
{
	internal class ScheduleBuilder
	{
		internal Schedule Schedule
		{
			get
			{
				return Schedule.FromByteArray(this.schedule);
			}
		}

		internal ScheduleBuilder() : this(Schedule.Never)
		{
		}

		internal ScheduleBuilder(Schedule schedule)
		{
			this.schedule = (schedule ?? Schedule.Never).ToByteArray();
		}

		internal bool GetStateOfInterval(DayOfWeek day, int intervalOfDay)
		{
			int bitPosition = ScheduleBuilder.GetBitPosition(day, intervalOfDay);
			int num = bitPosition / 8;
			byte b = (byte)(128 >> bitPosition % 8);
			return 0 != (this.schedule[num] & b);
		}

		internal void SetStateOfInterval(DayOfWeek day, int intervalOfDay, bool state)
		{
			int bitPosition = ScheduleBuilder.GetBitPosition(day, intervalOfDay);
			int num = bitPosition / 8;
			byte b = (byte)(128 >> bitPosition % 8);
			if (state)
			{
				byte[] array = this.schedule;
				int num2 = num;
				array[num2] |= b;
				return;
			}
			byte[] array2 = this.schedule;
			int num3 = num;
			array2[num3] &= ~b;
		}

		internal bool GetStateOfInterval(int intervalOfDay)
		{
			for (DayOfWeek dayOfWeek = DayOfWeek.Sunday; dayOfWeek <= DayOfWeek.Saturday; dayOfWeek++)
			{
				if (this.GetStateOfInterval(dayOfWeek, intervalOfDay))
				{
					return true;
				}
			}
			return false;
		}

		internal void SetStateOfInterval(int intervalOfDay, bool state)
		{
			for (DayOfWeek dayOfWeek = DayOfWeek.Sunday; dayOfWeek <= DayOfWeek.Saturday; dayOfWeek++)
			{
				this.SetStateOfInterval(dayOfWeek, intervalOfDay, state);
			}
		}

		internal bool GetStateOfHour(DayOfWeek day, int hour)
		{
			for (int i = 0; i < 4; i++)
			{
				if (this.GetStateOfInterval(day, hour * 4 + i))
				{
					return true;
				}
			}
			return false;
		}

		internal void SetStateOfHour(DayOfWeek day, int hour, bool state)
		{
			for (int i = 0; i < 4; i++)
			{
				this.SetStateOfInterval(day, hour * 4 + i, state);
			}
		}

		internal bool GetStateOfHour(int hour)
		{
			for (DayOfWeek dayOfWeek = DayOfWeek.Sunday; dayOfWeek <= DayOfWeek.Saturday; dayOfWeek++)
			{
				if (this.GetStateOfHour(dayOfWeek, hour))
				{
					return true;
				}
			}
			return false;
		}

		internal void SetStateOfHour(int hour, bool state)
		{
			for (DayOfWeek dayOfWeek = DayOfWeek.Sunday; dayOfWeek <= DayOfWeek.Saturday; dayOfWeek++)
			{
				this.SetStateOfHour(dayOfWeek, hour, state);
			}
		}

		internal bool GetStateOfDay(DayOfWeek day)
		{
			for (int i = 0; i < 96; i++)
			{
				if (this.GetStateOfInterval(day, i))
				{
					return true;
				}
			}
			return false;
		}

		internal void SetStateOfDay(DayOfWeek day, bool state)
		{
			for (int i = 0; i < 96; i++)
			{
				this.SetStateOfInterval(day, i, state);
			}
		}

		internal void SetEntireState(bool[] bitMap)
		{
			if (bitMap == null)
			{
				throw new ArgumentNullException("bitMap");
			}
			if (bitMap.Length != 672)
			{
				throw new ArgumentException(DataStrings.ErrorInputSchedulerBuilder(bitMap.Length, 672), "bitMap");
			}
			int num = 0;
			for (DayOfWeek dayOfWeek = DayOfWeek.Sunday; dayOfWeek <= DayOfWeek.Saturday; dayOfWeek++)
			{
				for (int i = 0; i < 96; i++)
				{
					this.SetStateOfInterval(dayOfWeek, i, bitMap[num]);
					num++;
				}
			}
		}

		internal bool[] GetEntireState()
		{
			bool[] array = new bool[672];
			int num = 0;
			for (DayOfWeek dayOfWeek = DayOfWeek.Sunday; dayOfWeek <= DayOfWeek.Saturday; dayOfWeek++)
			{
				for (int i = 0; i < 96; i++)
				{
					array[num] = this.GetStateOfInterval(dayOfWeek, i);
					num++;
				}
			}
			return array;
		}

		private static int GetBitPosition(DayOfWeek day, int intervalOfDay)
		{
			int hour = intervalOfDay / 4;
			int minute = intervalOfDay % 4 * 15;
			WeekDayAndTime weekDayAndTime = new WeekDayAndTime(day, hour, minute).ToUniversalTime(ScheduleInterval.WeekBitmapReference);
			return (int)(weekDayAndTime.DayOfWeek * (DayOfWeek)96 + (weekDayAndTime.Hour * 60 + weekDayAndTime.Minute) / 15);
		}

		internal const int IntervalTime = 15;

		internal const int MinutesOfHour = 60;

		internal const int IntervalsOfHour = 4;

		internal const int HoursOfDay = 24;

		internal const int IntervalsOfDay = 96;

		internal const int DaysOfWeek = 7;

		internal const int TotalBits = 672;

		private byte[] schedule;
	}
}
