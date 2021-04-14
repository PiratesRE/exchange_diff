using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Exchange.Connections.Eas.Model.Common.Email;
using Microsoft.Exchange.Connections.Eas.Model.Request.AirSyncBase;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Request.Calendar
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[XmlType(Namespace = "Calendar", TypeName = "Exception")]
	public class Exception : ICalendarData
	{
		[XmlElement(ElementName = "Deleted")]
		public byte? Deleted { get; set; }

		[XmlElement(ElementName = "StartTime")]
		public string StartTime { get; set; }

		[XmlElement(ElementName = "Body", Namespace = "AirSyncBase")]
		public Body Body { get; set; }

		[XmlElement(ElementName = "Subject")]
		public string CalendarSubject { get; set; }

		[XmlElement(ElementName = "EndTime")]
		public string EndTime { get; set; }

		[XmlElement(ElementName = "ExceptionStartTime")]
		public string ExceptionStartTime { get; set; }

		[XmlElement(ElementName = "BusyStatus")]
		public byte? BusyStatus { get; set; }

		[XmlElement(ElementName = "AllDayEvent")]
		public byte? AllDayEvent { get; set; }

		[XmlElement(ElementName = "Location")]
		public string Location { get; set; }

		[XmlElement(ElementName = "Reminder")]
		public uint? Reminder { get; set; }

		[XmlElement(ElementName = "Sensitivity", Namespace = "Calendar")]
		public byte? Sensitivity { get; set; }

		[XmlElement(ElementName = "DtStamp", Namespace = "Calendar")]
		public string DtStamp { get; set; }

		[XmlElement(ElementName = "MeetingStatus", Namespace = "Calendar")]
		public byte? MeetingStatus { get; set; }

		[XmlArray(ElementName = "Attendees", Namespace = "Calendar")]
		public List<Attendee> Attendees { get; set; }

		[XmlArray(ElementName = "Categories", Namespace = "Calendar")]
		public List<Category> CalendarCategories { get; set; }

		[XmlIgnore]
		public bool DeletedSpecified
		{
			get
			{
				return this.Deleted != null;
			}
		}

		[XmlIgnore]
		public bool AllDayEventSpecified
		{
			get
			{
				return this.AllDayEvent != null;
			}
		}

		[XmlIgnore]
		public bool BusyStatusSpecified
		{
			get
			{
				return this.BusyStatus != null;
			}
		}

		[XmlIgnore]
		public bool ReminderSpecified
		{
			get
			{
				return this.Reminder != null;
			}
		}

		[XmlIgnore]
		public bool SensitivitySpecified
		{
			get
			{
				return this.Sensitivity != null;
			}
		}

		[XmlIgnore]
		public bool MeetingStatusSpecified
		{
			get
			{
				return this.MeetingStatus != null;
			}
		}

		[XmlIgnore]
		public bool AttendeesSpecified
		{
			get
			{
				return this.Attendees != null;
			}
		}

		[XmlIgnore]
		public bool CalendarCategoriesSpecified
		{
			get
			{
				return this.CalendarCategories != null;
			}
		}
	}
}
