using System;
using System.Collections.Generic;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	internal class CalendarItemBaseData
	{
		public StoreObjectId Id
		{
			get
			{
				return this.id;
			}
			set
			{
				this.id = value;
			}
		}

		public string ChangeKey
		{
			get
			{
				return this.changeKey;
			}
			set
			{
				this.changeKey = value;
			}
		}

		public StoreObjectId FolderId
		{
			get
			{
				return this.folderId;
			}
			set
			{
				this.folderId = value;
			}
		}

		public List<AttachmentId> AttachmentIds
		{
			get
			{
				return this.attachmentIds;
			}
		}

		public List<AttendeeData> Attendees
		{
			get
			{
				return this.attendees;
			}
		}

		public string BodyText
		{
			get
			{
				return this.bodyText;
			}
			set
			{
				this.bodyText = value;
			}
		}

		public BodyFormat BodyFormat
		{
			get
			{
				return this.bodyFormat;
			}
			set
			{
				this.bodyFormat = value;
			}
		}

		public CalendarItemType CalendarItemType
		{
			get
			{
				return this.calendarItemType;
			}
			set
			{
				this.calendarItemType = value;
			}
		}

		public ExDateTime EndTime
		{
			get
			{
				return this.endTime;
			}
			set
			{
				this.endTime = value;
			}
		}

		public BusyType FreeBusyStatus
		{
			get
			{
				return this.freeBusyStatus;
			}
			set
			{
				this.freeBusyStatus = value;
			}
		}

		public Importance Importance
		{
			get
			{
				return this.importance;
			}
			set
			{
				this.importance = value;
			}
		}

		public bool IsAllDayEvent
		{
			get
			{
				return this.isAllDayEvent;
			}
			set
			{
				this.isAllDayEvent = value;
			}
		}

		public bool IsMeeting
		{
			get
			{
				return this.isMeeting;
			}
			set
			{
				this.isMeeting = value;
			}
		}

		public bool IsOrganizer
		{
			get
			{
				return this.isOrganizer;
			}
			set
			{
				this.isOrganizer = value;
			}
		}

		public bool IsResponseRequested
		{
			get
			{
				return this.isResponseRequested;
			}
			set
			{
				this.isResponseRequested = value;
			}
		}

		public string Location
		{
			get
			{
				return this.location;
			}
			set
			{
				this.location = value;
			}
		}

		public bool MeetingRequestWasSent
		{
			get
			{
				return this.meetingRequestWasSent;
			}
			set
			{
				this.meetingRequestWasSent = value;
			}
		}

		public Participant Organizer
		{
			get
			{
				return this.organizer;
			}
			set
			{
				this.organizer = value;
			}
		}

		public StoreObjectId ParentId
		{
			get
			{
				return this.parentId;
			}
			set
			{
				this.parentId = value;
			}
		}

		public Sensitivity Sensitivity
		{
			get
			{
				return this.sensitivity;
			}
			set
			{
				this.sensitivity = value;
			}
		}

		public string Subject
		{
			get
			{
				return this.subject;
			}
			set
			{
				this.subject = value;
			}
		}

		public ExDateTime StartTime
		{
			get
			{
				return this.startTime;
			}
			set
			{
				this.startTime = value;
			}
		}

		public CalendarItemBaseData()
		{
			this.attachmentIds = new List<AttachmentId>();
			this.attendees = new List<AttendeeData>();
		}

		public CalendarItemBaseData(CalendarItemBase calendarItemBase)
		{
			if (calendarItemBase is CalendarItem || calendarItemBase is CalendarItemOccurrence)
			{
				throw new ArgumentException("Constructing a CalendarItemBase from sub-class type CalendarItem or CalendarItemOccurrence probably causes unexpected behavior. Use Create() instead.");
			}
			this.SetFrom(calendarItemBase);
		}

		public CalendarItemBaseData(CalendarItemBaseData other)
		{
			if (other.attachmentIds != null)
			{
				this.attachmentIds = new List<AttachmentId>();
				foreach (AttachmentId item in other.attachmentIds)
				{
					this.attachmentIds.Add(item);
				}
			}
			if (other.attendees != null)
			{
				this.attendees = new List<AttendeeData>();
				foreach (AttendeeData other2 in other.attendees)
				{
					this.attendees.Add(new AttendeeData(other2));
				}
			}
			this.bodyText = other.bodyText;
			this.bodyFormat = other.bodyFormat;
			this.calendarItemType = other.calendarItemType;
			this.endTime = other.endTime;
			this.freeBusyStatus = other.freeBusyStatus;
			try
			{
				if (other.folderId != null)
				{
					this.folderId = StoreObjectId.Deserialize(other.folderId.GetBytes());
				}
				if (other.id != null)
				{
					this.id = StoreObjectId.Deserialize(other.id.GetBytes());
				}
			}
			catch (ArgumentException)
			{
				throw new OwaInvalidRequestException("Invalid store object id");
			}
			catch (FormatException)
			{
				throw new OwaInvalidRequestException("Invalid store object id");
			}
			this.changeKey = other.changeKey;
			this.importance = other.importance;
			this.isAllDayEvent = other.isAllDayEvent;
			this.isMeeting = other.isMeeting;
			this.isOrganizer = other.isOrganizer;
			this.isResponseRequested = other.isResponseRequested;
			this.location = other.location;
			this.meetingRequestWasSent = other.meetingRequestWasSent;
			this.organizer = other.organizer;
			this.parentId = other.parentId;
			this.sensitivity = other.sensitivity;
			this.startTime = other.startTime;
			this.subject = other.subject;
		}

		public static CalendarItemBaseData Create(CalendarItemBase calendarItemBase)
		{
			CalendarItem calendarItem = calendarItemBase as CalendarItem;
			if (calendarItem != null)
			{
				return new CalendarItemData(calendarItem);
			}
			CalendarItemOccurrence calendarItemOccurrence = calendarItemBase as CalendarItemOccurrence;
			if (calendarItemOccurrence != null)
			{
				return new CalendarItemOccurrenceData(calendarItemOccurrence);
			}
			return new CalendarItemBaseData(calendarItemBase);
		}

		public static bool SyncAttendeesToCalendarItem(CalendarItemBaseData data, CalendarItemBase calendarItemBase)
		{
			bool flag = !data.IsAttendeesEqual(calendarItemBase);
			if (flag)
			{
				calendarItemBase.AttendeeCollection.Clear();
				foreach (AttendeeData attendeeData in data.attendees)
				{
					calendarItemBase.AttendeeCollection.Add(attendeeData.Participant, attendeeData.AttendeeType, null, null, false);
				}
			}
			return flag;
		}

		public static Attendee GetFirstResourceAttendee(CalendarItemBase calendarItemBase)
		{
			Attendee result = null;
			if (calendarItemBase != null)
			{
				foreach (Attendee attendee in calendarItemBase.AttendeeCollection)
				{
					if (attendee.AttendeeType == AttendeeType.Resource && attendee.Participant != null && !string.IsNullOrEmpty(attendee.Participant.DisplayName))
					{
						result = attendee;
						break;
					}
				}
			}
			return result;
		}

		public static bool GetIsResponseRequested(CalendarItemBase calendarItemBase)
		{
			object obj = calendarItemBase.TryGetProperty(ItemSchema.IsResponseRequested);
			return obj is bool && (bool)obj;
		}

		public static void SetIsResponseRequested(CalendarItemBase calendarItemBase, bool value)
		{
			bool flag = CalendarItemBaseData.GetIsResponseRequested(calendarItemBase);
			if (flag != value)
			{
				calendarItemBase.SetProperties(CalendarItemBaseData.isResponseRequestedPropertyDefinition, new object[]
				{
					value
				});
			}
		}

		public virtual void SetFrom(CalendarItemBase calendarItemBase)
		{
			if (this.attachmentIds == null)
			{
				this.attachmentIds = new List<AttachmentId>();
			}
			else
			{
				this.attachmentIds.Clear();
			}
			if (this.attendees == null)
			{
				this.attendees = new List<AttendeeData>();
			}
			else
			{
				this.attendees.Clear();
			}
			if (calendarItemBase.AttachmentCollection != null)
			{
				foreach (AttachmentHandle handle in calendarItemBase.AttachmentCollection)
				{
					using (Attachment attachment = calendarItemBase.AttachmentCollection.Open(handle))
					{
						if (attachment.Id == null)
						{
							throw new ArgumentNullException("attachment.Id");
						}
						this.attachmentIds.Add(attachment.Id);
					}
				}
			}
			if (calendarItemBase.Body != null)
			{
				this.bodyText = ItemUtility.GetItemBody(calendarItemBase, BodyFormat.TextPlain);
				this.bodyFormat = BodyFormat.TextPlain;
			}
			this.calendarItemType = calendarItemBase.CalendarItemType;
			this.endTime = calendarItemBase.EndTime;
			this.freeBusyStatus = calendarItemBase.FreeBusyStatus;
			try
			{
				if (calendarItemBase.ParentId != null)
				{
					this.folderId = StoreObjectId.Deserialize(calendarItemBase.ParentId.GetBytes());
				}
				else
				{
					this.folderId = null;
				}
				if (calendarItemBase.Id != null && calendarItemBase.Id.ObjectId != null)
				{
					this.id = StoreObjectId.Deserialize(calendarItemBase.Id.ObjectId.GetBytes());
				}
				else
				{
					this.id = null;
				}
			}
			catch (ArgumentException)
			{
				throw new OwaInvalidRequestException("Invalid store object id");
			}
			catch (FormatException)
			{
				throw new OwaInvalidRequestException("Invalid store object id");
			}
			if (calendarItemBase.Id != null)
			{
				this.changeKey = calendarItemBase.Id.ChangeKeyAsBase64String();
			}
			else
			{
				this.changeKey = null;
			}
			this.importance = calendarItemBase.Importance;
			this.isAllDayEvent = calendarItemBase.IsAllDayEvent;
			this.isMeeting = calendarItemBase.IsMeeting;
			this.isOrganizer = calendarItemBase.IsOrganizer();
			this.isResponseRequested = CalendarItemBaseData.GetIsResponseRequested(calendarItemBase);
			this.location = calendarItemBase.Location;
			this.meetingRequestWasSent = calendarItemBase.MeetingRequestWasSent;
			this.organizer = AttendeeData.CloneParticipant(calendarItemBase.Organizer);
			if (calendarItemBase.ParentId != null)
			{
				this.parentId = StoreObjectId.FromProviderSpecificId(calendarItemBase.ParentId.ProviderLevelItemId);
			}
			if (calendarItemBase.AttendeeCollection != null)
			{
				foreach (Attendee attendee in calendarItemBase.AttendeeCollection)
				{
					this.attendees.Add(new AttendeeData(attendee));
				}
			}
			this.sensitivity = calendarItemBase.Sensitivity;
			this.startTime = calendarItemBase.StartTime;
			this.subject = calendarItemBase.Subject;
		}

		public bool SetLocation(CalendarItemBase calendarItemBase)
		{
			bool result = false;
			if (string.IsNullOrEmpty(calendarItemBase.Location))
			{
				Attendee firstResourceAttendee = CalendarItemBaseData.GetFirstResourceAttendee(calendarItemBase);
				if (firstResourceAttendee != null)
				{
					this.Location = firstResourceAttendee.Participant.DisplayName;
					result = true;
				}
			}
			return result;
		}

		public virtual EditCalendarItemHelper.CalendarItemUpdateFlags CopyTo(CalendarItemBase calendarItemBase)
		{
			if (calendarItemBase.Id != null && (this.id == null || this.id.CompareTo(calendarItemBase.Id.ObjectId) != 0))
			{
				throw new OwaLostContextException("Lost changes since last save.");
			}
			EditCalendarItemHelper.CalendarItemUpdateFlags calendarItemUpdateFlags = EditCalendarItemHelper.CalendarItemUpdateFlags.None;
			if (EditCalendarItemHelper.BodyChanged(this.bodyText, calendarItemBase))
			{
				if (!string.IsNullOrEmpty(this.bodyText))
				{
					if (this.bodyFormat == BodyFormat.TextHtml)
					{
						ItemUtility.SetItemBody(calendarItemBase, BodyFormat.TextHtml, this.bodyText);
						calendarItemUpdateFlags |= EditCalendarItemHelper.CalendarItemUpdateFlags.OtherChanged;
					}
					else
					{
						if (this.bodyFormat != BodyFormat.TextPlain)
						{
							throw new ArgumentOutOfRangeException("calendarItemBase", "Unhandled body format type : " + this.bodyFormat);
						}
						ItemUtility.SetItemBody(calendarItemBase, BodyFormat.TextPlain, this.bodyText);
						calendarItemUpdateFlags |= EditCalendarItemHelper.CalendarItemUpdateFlags.OtherChanged;
					}
				}
				else
				{
					ItemUtility.SetItemBody(calendarItemBase, BodyFormat.TextPlain, string.Empty);
					calendarItemUpdateFlags |= EditCalendarItemHelper.CalendarItemUpdateFlags.OtherChanged;
				}
			}
			if (this.freeBusyStatus != calendarItemBase.FreeBusyStatus)
			{
				calendarItemBase.FreeBusyStatus = this.freeBusyStatus;
				calendarItemUpdateFlags |= EditCalendarItemHelper.CalendarItemUpdateFlags.OtherChanged;
			}
			if (calendarItemBase.Importance != this.importance)
			{
				calendarItemBase.Importance = this.importance;
				calendarItemUpdateFlags |= EditCalendarItemHelper.CalendarItemUpdateFlags.OtherChanged;
			}
			if (calendarItemBase.IsAllDayEvent != this.isAllDayEvent)
			{
				calendarItemBase.IsAllDayEvent = this.isAllDayEvent;
				calendarItemUpdateFlags |= EditCalendarItemHelper.CalendarItemUpdateFlags.TimeChanged;
			}
			if (calendarItemBase.IsMeeting != this.isMeeting)
			{
				calendarItemBase.IsMeeting = this.isMeeting;
				calendarItemUpdateFlags |= EditCalendarItemHelper.CalendarItemUpdateFlags.OtherChanged;
			}
			if (CalendarItemBaseData.GetIsResponseRequested(calendarItemBase) != this.isResponseRequested)
			{
				CalendarItemBaseData.SetIsResponseRequested(calendarItemBase, this.isResponseRequested);
				calendarItemUpdateFlags |= EditCalendarItemHelper.CalendarItemUpdateFlags.OtherChanged;
			}
			if (!CalendarUtilities.StringsEqualNullEmpty(calendarItemBase.Location, this.location, StringComparison.CurrentCulture))
			{
				calendarItemUpdateFlags |= EditCalendarItemHelper.CalendarItemUpdateFlags.LocationChanged;
				calendarItemBase.Location = ((this.location != null) ? this.location : string.Empty);
			}
			CalendarItemBaseData.SyncAttendeesToCalendarItem(this, calendarItemBase);
			if (calendarItemBase.AttendeesChanged)
			{
				calendarItemUpdateFlags |= EditCalendarItemHelper.CalendarItemUpdateFlags.AttendeesChanged;
			}
			if (calendarItemBase.Sensitivity != this.sensitivity)
			{
				calendarItemBase.Sensitivity = this.sensitivity;
				calendarItemUpdateFlags |= EditCalendarItemHelper.CalendarItemUpdateFlags.OtherChanged;
			}
			if (calendarItemBase.EndTime != this.endTime)
			{
				calendarItemUpdateFlags |= EditCalendarItemHelper.CalendarItemUpdateFlags.TimeChanged;
				calendarItemBase.EndTime = this.endTime;
			}
			if (calendarItemBase.StartTime != this.startTime)
			{
				calendarItemUpdateFlags |= EditCalendarItemHelper.CalendarItemUpdateFlags.TimeChanged;
				calendarItemBase.StartTime = this.startTime;
			}
			if (!CalendarUtilities.StringsEqualNullEmpty(calendarItemBase.Subject, this.subject, StringComparison.CurrentCulture))
			{
				calendarItemBase.Subject = ((this.subject != null) ? this.subject : string.Empty);
				calendarItemUpdateFlags |= EditCalendarItemHelper.CalendarItemUpdateFlags.OtherChanged;
			}
			return calendarItemUpdateFlags;
		}

		public bool IsAttendeesEqual(CalendarItemBase calendarItemBase)
		{
			bool flag = true;
			if (this.attendees.Count != calendarItemBase.AttendeeCollection.Count)
			{
				flag = false;
			}
			if (flag)
			{
				foreach (Attendee attendee in calendarItemBase.AttendeeCollection)
				{
					if (!this.attendees.Contains(new AttendeeData(attendee)))
					{
						flag = false;
						break;
					}
				}
			}
			return flag;
		}

		private static PropertyDefinition[] isResponseRequestedPropertyDefinition = new PropertyDefinition[]
		{
			ItemSchema.IsResponseRequested
		};

		private StoreObjectId id;

		private string changeKey;

		private StoreObjectId folderId;

		private List<AttachmentId> attachmentIds;

		private List<AttendeeData> attendees;

		private string bodyText;

		private BodyFormat bodyFormat = BodyFormat.TextPlain;

		private CalendarItemType calendarItemType;

		private ExDateTime endTime;

		private BusyType freeBusyStatus;

		private Importance importance;

		private bool isAllDayEvent;

		private bool isMeeting;

		private bool isOrganizer;

		private bool isResponseRequested;

		private string location;

		private bool meetingRequestWasSent;

		private Participant organizer;

		private StoreObjectId parentId;

		private Sensitivity sensitivity;

		private string subject;

		private ExDateTime startTime;
	}
}
