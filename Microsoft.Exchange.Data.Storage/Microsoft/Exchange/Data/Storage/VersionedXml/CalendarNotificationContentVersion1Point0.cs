using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Data.Storage.VersionedXml
{
	[CalendarNotificationContentRoot]
	[Serializable]
	public class CalendarNotificationContentVersion1Point0 : CalendarNotificationContentBase
	{
		public CalendarNotificationContentVersion1Point0() : base(new Version(1, 0))
		{
		}

		public CalendarNotificationContentVersion1Point0(CalendarNotificationType type, string typeDesc, IEnumerable<CalendarEvent> events)
		{
			this.CalNotifType = type;
			this.CalNotifTypeDesc = typeDesc;
			if (events != null)
			{
				this.CalEvents = new List<CalendarEvent>(events);
			}
		}

		[XmlElement("CalNotifType")]
		public CalendarNotificationType CalNotifType { get; set; }

		[XmlElement("CalNotifTypeDesc")]
		public string CalNotifTypeDesc { get; set; }

		[XmlElement("CalEvent")]
		public List<CalendarEvent> CalEvents
		{
			get
			{
				return AccessorTemplates.ListPropertyGetter<CalendarEvent>(ref this.calEvents);
			}
			set
			{
				AccessorTemplates.ListPropertySetter<CalendarEvent>(ref this.calEvents, value);
			}
		}

		private List<CalendarEvent> calEvents;
	}
}
