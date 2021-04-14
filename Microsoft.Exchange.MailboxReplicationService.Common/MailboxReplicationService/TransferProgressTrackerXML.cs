using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.MailboxReplicationService
{
	public sealed class TransferProgressTrackerXML : XMLSerializableBase
	{
		[XmlElement(ElementName = "TotalBytesTransferred")]
		public ulong TotalBytesTransferred { get; set; }

		[XmlElement(ElementName = "TotalItemsTransferred")]
		public ulong TotalItemsTransferred { get; set; }

		public TransferProgressTrackerXML()
		{
		}

		public TransferProgressTrackerXML(TransferProgressTracker tracker, bool showTimeSlots = false)
		{
			this.TotalBytesTransferred = tracker.BytesTransferred;
			this.TotalItemsTransferred = tracker.ItemsTransferred;
			if (showTimeSlots)
			{
				this.PerMinuteBytes = this.Convert(tracker.PerMinuteBytes);
				this.PerMinuteItems = this.Convert(tracker.PerMinuteItems);
				this.PerHourBytes = this.Convert(tracker.PerHourBytes);
				this.PerHourItems = this.Convert(tracker.PerHourItems);
				this.PerDayBytes = this.Convert(tracker.PerDayBytes);
				this.PerDayItems = this.Convert(tracker.PerDayItems);
				this.PerMonthBytes = this.Convert(tracker.PerMonthBytes);
				this.PerMonthItems = this.Convert(tracker.PerMonthItems);
			}
		}

		private TimeSlotXML[] Convert(FixedTimeSumSlot[] slots)
		{
			return Array.ConvertAll<FixedTimeSumSlot, TimeSlotXML>(slots, (FixedTimeSumSlot s) => new TimeSlotXML
			{
				StartTime = new DateTime(s.StartTimeInTicks, DateTimeKind.Utc).ToString("O"),
				Value = (ulong)s.Value
			});
		}

		[XmlArray("PerMinuteBytes")]
		[XmlArrayItem("Minute")]
		public TimeSlotXML[] PerMinuteBytes { get; set; }

		[XmlArrayItem("Hour")]
		[XmlArray("PerHourBytes")]
		public TimeSlotXML[] PerHourBytes { get; set; }

		[XmlArray("PerDayBytes")]
		[XmlArrayItem("Day")]
		public TimeSlotXML[] PerDayBytes { get; set; }

		[XmlArray("PerMonthBytes")]
		[XmlArrayItem("Month")]
		public TimeSlotXML[] PerMonthBytes { get; set; }

		[XmlArray("PerMinuteItems")]
		[XmlArrayItem("Minute")]
		public TimeSlotXML[] PerMinuteItems { get; set; }

		[XmlArray("PerHourItems")]
		[XmlArrayItem("Hour")]
		public TimeSlotXML[] PerHourItems { get; set; }

		[XmlArrayItem("Day")]
		[XmlArray("PerDayItems")]
		public TimeSlotXML[] PerDayItems { get; set; }

		[XmlArray("PerMonthItems")]
		[XmlArrayItem("Month")]
		public TimeSlotXML[] PerMonthItems { get; set; }
	}
}
