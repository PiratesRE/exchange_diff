using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Storage.VersionedXml
{
	[Serializable]
	public class TimePointScaningSettings : SwitchableSettingsBase
	{
		public TimePointScaningSettings()
		{
		}

		public TimePointScaningSettings(bool enabled, DateTime notifyingTimeInDay, Duration duration, Recurrence recurrence) : base(enabled)
		{
			this.NotifyingTimeInDay = notifyingTimeInDay;
			this.Duration = duration;
			this.Recurrence = recurrence;
		}

		[XmlElement("NotifyingTimeInDay")]
		public DateTime NotifyingTimeInDay { get; set; }

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

		[XmlElement("Recurrence")]
		public Recurrence Recurrence
		{
			get
			{
				return AccessorTemplates.DefaultConstructionPropertyGetter<Recurrence>(ref this.recurrence);
			}
			set
			{
				this.recurrence = value;
			}
		}

		private Duration duration;

		private Recurrence recurrence;
	}
}
