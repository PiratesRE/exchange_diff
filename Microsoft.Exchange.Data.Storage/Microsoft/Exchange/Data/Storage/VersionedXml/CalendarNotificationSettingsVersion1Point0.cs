using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Storage.VersionedXml
{
	[CalendarNotificationSettingsRoot]
	[Serializable]
	public class CalendarNotificationSettingsVersion1Point0 : CalendarNotificationSettingsBase
	{
		public CalendarNotificationSettingsVersion1Point0() : base(new Version(1, 0))
		{
		}

		public CalendarNotificationSettingsVersion1Point0(TimeSlotMonitoringSettings updateSettings, TimeSlotMonitoringSettings reminderSettings, TimePointScaningSettings summarySettings, IEnumerable<Emitter> emitters) : base(new Version(1, 0))
		{
			this.UpdateSettings = updateSettings;
			this.ReminderSettings = reminderSettings;
			this.SummarySettings = summarySettings;
			if (emitters != null)
			{
				this.Emitters = new List<Emitter>(emitters);
			}
		}

		[XmlElement("UpdateSettings")]
		public TimeSlotMonitoringSettings UpdateSettings
		{
			get
			{
				return AccessorTemplates.DefaultConstructionPropertyGetter<TimeSlotMonitoringSettings>(ref this.updateSettings);
			}
			set
			{
				this.updateSettings = value;
			}
		}

		[XmlElement("ReminderSettings")]
		public TimeSlotMonitoringSettings ReminderSettings
		{
			get
			{
				return AccessorTemplates.DefaultConstructionPropertyGetter<TimeSlotMonitoringSettings>(ref this.reminderSettings);
			}
			set
			{
				this.reminderSettings = value;
			}
		}

		[XmlElement("SummarySettings")]
		public TimePointScaningSettings SummarySettings
		{
			get
			{
				return AccessorTemplates.DefaultConstructionPropertyGetter<TimePointScaningSettings>(ref this.summarySettings);
			}
			set
			{
				this.summarySettings = value;
			}
		}

		[XmlElement("Emitter")]
		public List<Emitter> Emitters
		{
			get
			{
				return AccessorTemplates.ListPropertyGetter<Emitter>(ref this.emitters);
			}
			set
			{
				AccessorTemplates.ListPropertySetter<Emitter>(ref this.emitters, value);
			}
		}

		private TimeSlotMonitoringSettings updateSettings;

		private TimeSlotMonitoringSettings reminderSettings;

		private TimePointScaningSettings summarySettings;

		private List<Emitter> emitters;
	}
}
