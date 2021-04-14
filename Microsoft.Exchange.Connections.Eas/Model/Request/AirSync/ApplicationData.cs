using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Exchange.Connections.Eas.Model.Common.Email;
using Microsoft.Exchange.Connections.Eas.Model.Request.AirSyncBase;
using Microsoft.Exchange.Connections.Eas.Model.Request.Calendar;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Request.AirSync
{
	[XmlType(Namespace = "AirSync", TypeName = "ApplicationData")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class ApplicationData : ICalendarData
	{
		[XmlElement(ElementName = "Body", Namespace = "AirSyncBase")]
		public Body Body { get; set; }

		[XmlElement(ElementName = "NativeBodyType", Namespace = "AirSyncBase")]
		public byte? NativeBodyType { get; set; }

		[XmlElement(ElementName = "DateReceived", Namespace = "Email")]
		public string DateReceived { get; set; }

		[XmlElement(ElementName = "Flag", Namespace = "Email")]
		public Flag Flag { get; set; }

		[XmlElement(ElementName = "From", Namespace = "Email")]
		public string From { get; set; }

		[XmlElement(ElementName = "Importance", Namespace = "Email")]
		public byte? Importance { get; set; }

		[XmlElement(ElementName = "InternetCpid", Namespace = "Email")]
		public int? InternetCpid { get; set; }

		[XmlElement(ElementName = "Read", Namespace = "Email")]
		public byte? Read { get; set; }

		[XmlElement(ElementName = "To", Namespace = "Email")]
		public string To { get; set; }

		[XmlElement(ElementName = "TimeZone", Namespace = "Calendar")]
		public string TimeZone { get; set; }

		[XmlElement(ElementName = "AllDayEvent", Namespace = "Calendar")]
		public byte? AllDayEvent { get; set; }

		[XmlElement(ElementName = "BusyStatus", Namespace = "Calendar")]
		public byte? BusyStatus { get; set; }

		[XmlElement(ElementName = "OrganizerName", Namespace = "Calendar")]
		public string OrganizerName { get; set; }

		[XmlElement(ElementName = "OrganizerEmail", Namespace = "Calendar")]
		public string OrganizerEmail { get; set; }

		[XmlElement(ElementName = "DtStamp", Namespace = "Calendar")]
		public string DtStamp { get; set; }

		[XmlElement(ElementName = "EndTime", Namespace = "Calendar")]
		public string EndTime { get; set; }

		[XmlElement(ElementName = "Location", Namespace = "Calendar")]
		public string Location { get; set; }

		[XmlElement(ElementName = "Reminder", Namespace = "Calendar")]
		public uint? Reminder { get; set; }

		[XmlElement(ElementName = "Sensitivity", Namespace = "Calendar")]
		public byte? Sensitivity { get; set; }

		[XmlElement(ElementName = "Subject", Namespace = "Calendar")]
		public string CalendarSubject { get; set; }

		[XmlElement(ElementName = "StartTime", Namespace = "Calendar")]
		public string StartTime { get; set; }

		[XmlElement(ElementName = "UID", Namespace = "Calendar")]
		public string Uid { get; set; }

		[XmlElement(ElementName = "MeetingStatus", Namespace = "Calendar")]
		public byte? MeetingStatus { get; set; }

		[XmlArray(ElementName = "Attendees", Namespace = "Calendar")]
		public List<Attendee> Attendees { get; set; }

		[XmlArray(ElementName = "Categories", Namespace = "Calendar")]
		public List<Category> CalendarCategories { get; set; }

		[XmlElement(ElementName = "Recurrence", Namespace = "Calendar")]
		public Recurrence Recurrence { get; set; }

		[XmlArray(ElementName = "Exceptions", Namespace = "Calendar")]
		public List<Microsoft.Exchange.Connections.Eas.Model.Request.Calendar.Exception> Exceptions { get; set; }

		[XmlElement(ElementName = "ResponseRequested", Namespace = "Calendar")]
		public byte? ResponseRequested { get; set; }

		[XmlElement(ElementName = "DisallowNewTimeProposal", Namespace = "Calendar")]
		public byte? DisallowNewTimeProposal { get; set; }

		[XmlElement(ElementName = "OnlineMeetingConfLink", Namespace = "Calendar")]
		public string OnlineMeetingConferenceLink { get; set; }

		[XmlElement(ElementName = "OnlineMeetingExternalLink", Namespace = "Calendar")]
		public string OnlineMeetingExternalLink { get; set; }

		[XmlIgnore]
		public bool NativeBodyTypeSpecified
		{
			get
			{
				return this.NativeBodyType != null;
			}
		}

		[XmlIgnore]
		public bool ImportanceSpecified
		{
			get
			{
				return this.Importance != null;
			}
		}

		[XmlIgnore]
		public bool InternetCpidSpecified
		{
			get
			{
				return this.InternetCpid != null;
			}
		}

		[XmlIgnore]
		public bool ReadSpecified
		{
			get
			{
				return this.Read != null;
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

		[XmlIgnore]
		public bool ExceptionsSpecified
		{
			get
			{
				return this.Exceptions != null;
			}
		}

		[XmlIgnore]
		public bool ResponseRequestedSpecified
		{
			get
			{
				return this.ResponseRequested != null;
			}
		}

		[XmlIgnore]
		public bool DisallowNewTimeProposalSpecified
		{
			get
			{
				return this.DisallowNewTimeProposal != null;
			}
		}
	}
}
