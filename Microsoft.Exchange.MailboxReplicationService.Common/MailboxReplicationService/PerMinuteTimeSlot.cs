using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public class PerMinuteTimeSlot : TimeSlot
	{
		public PerMinuteTimeSlot(uint capacity) : base(capacity, 60000UL)
		{
		}

		public PerMinuteTimeSlot(ulong[] inputArray) : base(120U, inputArray, 60000UL)
		{
		}

		public PerMinuteTimeSlot() : base(120U, 60000UL)
		{
		}

		protected override uint GetElapsedSlotCount(DateTime lastUpdateTime, DateTime currentTime)
		{
			DateTime value = new DateTime(lastUpdateTime.Year, lastUpdateTime.Month, lastUpdateTime.Day, lastUpdateTime.Hour, lastUpdateTime.Minute, 0);
			DateTime dateTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, currentTime.Hour, currentTime.Minute, 0);
			return (uint)dateTime.Subtract(value).TotalMinutes;
		}

		protected override ulong GetLatestSlotMilliseconds(DateTime currentTime)
		{
			return (ulong)(currentTime.Second * 1000 + currentTime.Millisecond);
		}

		protected override TimeSlotXML GetSlotXML(DateTime time, ulong value)
		{
			return new TimeSlotXML
			{
				StartTime = time.ToString("yyyy-MM-dd HH:mm:00"),
				Value = value
			};
		}

		public const uint DefaultNumberOfMinutes = 120U;
	}
}
