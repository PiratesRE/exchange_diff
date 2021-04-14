using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public class DailyTimeSlot : TimeSlot
	{
		public DailyTimeSlot(uint capacity) : base(capacity, 86400000UL)
		{
		}

		public DailyTimeSlot(ulong[] inputArray) : base(90U, inputArray, 86400000UL)
		{
		}

		public DailyTimeSlot() : base(90U, 86400000UL)
		{
		}

		protected override uint GetElapsedSlotCount(DateTime lastUpdateTime, DateTime currentTime)
		{
			DateTime value = new DateTime(lastUpdateTime.Year, lastUpdateTime.Month, lastUpdateTime.Day, 0, 0, 0);
			DateTime dateTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 0, 0, 0);
			return (uint)dateTime.Subtract(value).TotalDays;
		}

		protected override ulong GetLatestSlotMilliseconds(DateTime currentTime)
		{
			return (ulong)currentTime.Hour * 3600000UL + (ulong)currentTime.Minute * 60000UL + (ulong)(currentTime.Second * 1000) + (ulong)currentTime.Millisecond;
		}

		protected override TimeSlotXML GetSlotXML(DateTime time, ulong value)
		{
			return new TimeSlotXML
			{
				StartTime = time.ToString("yyyy-MM-dd"),
				Value = value
			};
		}

		public const uint DefaultNumberOfDays = 90U;
	}
}
