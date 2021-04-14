using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Storage.VersionedXml
{
	[Serializable]
	public class TimeSlotMonitoringSettings : SwitchableSettingsBase
	{
		public TimeSlotMonitoringSettings()
		{
		}

		public TimeSlotMonitoringSettings(bool enabled, bool notifyInWorkHoursTimeSlot, DateTime notifyingStartTimeInDay, DateTime notifyingEndTimeInDay, Duration duration) : base(enabled)
		{
			this.NotifyInWorkHoursTimeSlot = notifyInWorkHoursTimeSlot;
			this.NotifyingStartTimeInDay = notifyingStartTimeInDay;
			this.NotifyingEndTimeInDay = notifyingEndTimeInDay;
			this.Duration = duration;
		}

		[XmlElement("NotifyInWorkHoursTimeSlot")]
		public bool NotifyInWorkHoursTimeSlot { get; set; }

		[XmlElement("NotifyingStartTimeInDay")]
		public DateTime NotifyingStartTimeInDay { get; set; }

		[XmlElement("NotifyingEndTimeInDay")]
		public DateTime NotifyingEndTimeInDay { get; set; }

		[XmlElement("Duration")]
		public Duration Duration
		{
			get
			{
				return AccessorTemplates.DefaultConstructionPropertyGetter<Duration>(ref this.duration);
			}
			set
			{
				this.duration = value;
			}
		}

		private Duration duration;
	}
}
