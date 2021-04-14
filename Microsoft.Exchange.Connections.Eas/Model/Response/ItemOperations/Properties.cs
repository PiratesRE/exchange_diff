using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Exchange.Connections.Eas.Model.Common.Email;
using Microsoft.Exchange.Connections.Eas.Model.Common.WindowsLive;
using Microsoft.Exchange.Connections.Eas.Model.Response.AirSyncBase;
using Microsoft.Exchange.Connections.Eas.Model.Response.Calendar;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Response.ItemOperations
{
	[XmlType(Namespace = "ItemOperations", TypeName = "Properties")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class Properties
	{
		[XmlElement(ElementName = "Body", Namespace = "AirSyncBase")]
		public Body Body { get; set; }

		[XmlArray(ElementName = "Categories", Namespace = "Email")]
		public List<Category> Categories { get; set; }

		[XmlArray(ElementName = "SystemCategories", Namespace = "WindowsLive")]
		public List<CategoryId> SystemCategories { get; set; }

		[XmlElement(ElementName = "ConversationId", Namespace = "Email2")]
		public string ConversationId { get; set; }

		[XmlElement(ElementName = "ConversationIndex", Namespace = "Email2")]
		public string ConversationIndex { get; set; }

		[XmlElement(ElementName = "DateReceived", Namespace = "Email")]
		public string DateReceived { get; set; }

		[XmlElement(ElementName = "Flag", Namespace = "Email")]
		public Flag Flag { get; set; }

		[XmlElement(ElementName = "From", Namespace = "Email")]
		public string From { get; set; }

		[XmlElement(ElementName = "Importance", Namespace = "Email")]
		public byte? Importance { get; set; }

		[XmlElement(ElementName = "MessageClass", Namespace = "Email")]
		public string MessageClass { get; set; }

		[XmlElement(ElementName = "Read", Namespace = "Email")]
		public byte? Read { get; set; }

		[XmlElement(ElementName = "Subject", Namespace = "Email")]
		public string Subject { get; set; }

		[XmlElement(ElementName = "Subject", Namespace = "Calendar")]
		public string CalendarSubject { get; set; }

		[XmlElement(ElementName = "TimeZone", Namespace = "Calendar")]
		public string TimeZone { get; set; }

		[XmlElement(ElementName = "StartTime", Namespace = "Calendar")]
		public string StartTime { get; set; }

		[XmlElement(ElementName = "EndTime", Namespace = "Calendar")]
		public string EndTime { get; set; }

		[XmlElement(ElementName = "UID", Namespace = "Calendar")]
		public string Uid { get; set; }

		[XmlElement(ElementName = "AllDayEvent", Namespace = "Calendar")]
		public bool AllDayEvent { get; set; }

		[XmlElement(ElementName = "BusyStatus", Namespace = "Calendar")]
		public int BusyStatus { get; set; }

		[XmlElement(ElementName = "Location", Namespace = "Calendar")]
		public string Location { get; set; }

		[XmlElement(ElementName = "Sensitivity", Namespace = "Calendar")]
		public int Sensitivity { get; set; }

		[XmlElement(ElementName = "Reminder", Namespace = "Calendar")]
		public int Reminder { get; set; }

		[XmlElement(ElementName = "OrganizerEmail", Namespace = "Calendar")]
		public string OrganizerEmail { get; set; }

		[XmlElement(ElementName = "OrganizerName", Namespace = "Calendar")]
		public string OrganizerName { get; set; }

		[XmlElement(ElementName = "MeetingStatus", Namespace = "Calendar")]
		public int MeetingStatus { get; set; }

		[XmlArray(ElementName = "Attendees", Namespace = "Calendar")]
		public List<Attendee> Attendees { get; set; }

		[XmlArray(ElementName = "Exceptions", Namespace = "Calendar")]
		public List<Microsoft.Exchange.Connections.Eas.Model.Response.Calendar.Exception> Exceptions { get; set; }

		[XmlElement(ElementName = "Recurrence", Namespace = "Calendar")]
		public Recurrence Recurrence { get; set; }

		[XmlElement(ElementName = "ResponseType", Namespace = "Calendar")]
		public int ResponseType { get; set; }

		[XmlElement(ElementName = "To", Namespace = "Email")]
		public string To { get; set; }
	}
}
