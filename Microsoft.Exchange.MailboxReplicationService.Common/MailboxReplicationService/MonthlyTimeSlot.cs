using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public class MonthlyTimeSlot : TimeSlot
	{
		public MonthlyTimeSlot(uint capacity) : base(capacity, (ulong)-1702967296)
		{
		}

		public MonthlyTimeSlot(ulong[] inputArray) : base(24U, inputArray, (ulong)-1702967296)
		{
		}

		public MonthlyTimeSlot() : base(24U, (ulong)-1702967296)
		{
		}

		protected override uint GetElapsedSlotCount(DateTime lastUpdateTime, DateTime currentTime)
		{
			return (uint)(currentTime.Year * 12 + currentTime.Month - (lastUpdateTime.Year * 12 + lastUpdateTime.Month));
		}

		protected override ulong GetLatestSlotMilliseconds(DateTime currentTime)
		{
			return (ulong)((long)(currentTime.Day - 1) * 86400000L + (long)((ulong)currentTime.Hour * 3600000UL) + (long)((ulong)currentTime.Minute * 60000UL) + (long)((ulong)(currentTime.Second * 1000)) + (long)((ulong)currentTime.Millisecond));
		}

		protected override TimeSlotXML GetSlotXML(DateTime time, ulong value)
		{
			return new TimeSlotXML
			{
				StartTime = time.ToString("yyyy-MM-01"),
				Value = value
			};
		}

		public const uint DefaultNumberOfMonths = 24U;
	}
}
