using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public class HourlyTimeSlot : TimeSlot
	{
		public HourlyTimeSlot(uint capacity) : base(capacity, 3600000UL)
		{
		}

		public HourlyTimeSlot(ulong[] inputArray) : base(120U, inputArray, 3600000UL)
		{
		}

		public HourlyTimeSlot() : base(120U, 3600000UL)
		{
		}

		protected override uint GetElapsedSlotCount(DateTime lastUpdateTime, DateTime currentTime)
		{
			DateTime value = new DateTime(lastUpdateTime.Year, lastUpdateTime.Month, lastUpdateTime.Day, lastUpdateTime.Hour, 0, 0);
			DateTime dateTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, currentTime.Hour, 0, 0);
			return (uint)dateTime.Subtract(value).TotalHours;
		}

		protected override ulong GetLatestSlotMilliseconds(DateTime currentTime)
		{
			return (ulong)currentTime.Minute * 60000UL + (ulong)(currentTime.Second * 1000) + (ulong)currentTime.Millisecond;
		}

		protected override TimeSlotXML GetSlotXML(DateTime time, ulong value)
		{
			return new TimeSlotXML
			{
				StartTime = time.ToString("yyyy-MM-dd HH:00:00"),
				Value = value
			};
		}

		public const uint DefaultNumberOfHours = 120U;
	}
}
