using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MeetingForwardNotification : MeetingMessageInstance, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		internal MeetingForwardNotification(ICoreItem coreItem) : base(coreItem)
		{
		}

		public new static MeetingForwardNotification Bind(StoreSession session, StoreId storeId, ICollection<PropertyDefinition> propsToReturn)
		{
			return ItemBuilder.ItemBind<MeetingForwardNotification>(session, storeId, MeetingMessageInstanceSchema.Instance, propsToReturn);
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<MeetingForwardNotification>(this);
		}

		public override Schema Schema
		{
			get
			{
				this.CheckDisposed("Schema::get");
				return MeetingForwardNotificationSchema.Instance;
			}
		}

		public List<Participant> GetParticipantCollection()
		{
			List<Participant> list = new List<Participant>();
			List<BlobRecipient> forwardedAttendees = this.GetForwardedAttendees();
			if (forwardedAttendees != null)
			{
				foreach (BlobRecipient blobRecipient in forwardedAttendees)
				{
					if (blobRecipient.Participant != null)
					{
						list.Add(blobRecipient.Participant);
					}
				}
			}
			return list;
		}

		public void SendRumUpdate(ref CalendarItemBase originalCalendarItem)
		{
			if (originalCalendarItem == null)
			{
				return;
			}
			CalendarInconsistencyFlag inconsistencyFlag;
			if (this.MatchesOrganizerItem(ref originalCalendarItem, out inconsistencyFlag))
			{
				return;
			}
			List<BlobRecipient> mfnaddedAttendees = this.GetMFNAddedAttendees();
			IAttendeeCollection attendeeCollection = originalCalendarItem.AttendeeCollection;
			Participant organizer = originalCalendarItem.Organizer;
			List<Attendee> list = new List<Attendee>();
			MailboxSession mailboxSession = base.MailboxSession;
			foreach (BlobRecipient blobRecipient in mfnaddedAttendees)
			{
				list.Add(originalCalendarItem.AttendeeCollection.Add(blobRecipient.Participant, AttendeeType.Required, null, null, false));
			}
			if (list.Count > 0)
			{
				UpdateRumInfo rumInfo;
				if (originalCalendarItem.GlobalObjectId.IsCleanGlobalObjectId)
				{
					rumInfo = UpdateRumInfo.CreateMasterInstance(list, inconsistencyFlag);
				}
				else
				{
					rumInfo = UpdateRumInfo.CreateOccurrenceInstance(originalCalendarItem.GlobalObjectId.Date, list, inconsistencyFlag);
				}
				originalCalendarItem.SendUpdateRums(rumInfo, false);
			}
		}

		internal static MeetingForwardNotification Create(MeetingRequest request)
		{
			MailboxSession mailboxSession = request.Session as MailboxSession;
			if (mailboxSession == null)
			{
				throw new NotSupportedException();
			}
			MeetingForwardNotification meetingForwardNotification = null;
			bool flag = false;
			MeetingForwardNotification result;
			try
			{
				StoreObjectId defaultFolderId = mailboxSession.GetDefaultFolderId((mailboxSession.LogonType == LogonType.Transport) ? DefaultFolderType.SentItems : DefaultFolderType.Drafts);
				meetingForwardNotification = ItemBuilder.CreateNewItem<MeetingForwardNotification>(mailboxSession, defaultFolderId, request.IsSeriesMessage ? ItemCreateInfo.MeetingForwardNotificationSeriesInfo : ItemCreateInfo.MeetingForwardNotificationInfo);
				meetingForwardNotification.Load(InternalSchema.ContentConversionProperties);
				meetingForwardNotification.Initialize(request, request.IsSeriesMessage ? "IPM.MeetingMessageSeries.Notification.Forward" : "IPM.Schedule.Meeting.Notification.Forward");
				flag = true;
				result = meetingForwardNotification;
			}
			finally
			{
				if (!flag && meetingForwardNotification != null)
				{
					meetingForwardNotification.Dispose();
				}
			}
			return result;
		}

		internal List<BlobRecipient> GetForwardedAttendees()
		{
			return BlobRecipientParser.ReadRecipients(this, InternalSchema.ForwardNotificationRecipients);
		}

		internal List<BlobRecipient> GetMFNAddedAttendees()
		{
			return BlobRecipientParser.ReadRecipients(this, InternalSchema.MFNAddedRecipients);
		}

		internal void SetForwardedAttendees(List<BlobRecipient> list)
		{
			base.LocationIdentifierHelperInstance.SetLocationIdentifier(60277U, LastChangeAction.SetForwardedAttendees);
			BlobRecipientParser.WriteRecipients(this, InternalSchema.ForwardNotificationRecipients, list);
		}

		private bool IsMfnProcessed
		{
			get
			{
				CalendarProcessingSteps valueOrDefault = base.GetValueOrDefault<CalendarProcessingSteps>(InternalSchema.CalendarProcessingSteps);
				return (valueOrDefault & CalendarProcessingSteps.ProcessedMeetingForwardNotification) == CalendarProcessingSteps.ProcessedMeetingForwardNotification;
			}
		}

		private void Initialize(MeetingRequest request, string className)
		{
			base.Initialize();
			this.ClassName = className;
			base.Recipients.Add(request.From);
			CalendarItemBase.CopyPropertiesTo(request, this, MeetingForwardNotification.MeetingForwardNotificationProperties);
			List<BlobRecipient> list = new List<BlobRecipient>();
			foreach (Recipient recipient in request.Recipients)
			{
				BlobRecipient item = new BlobRecipient(recipient);
				list.Add(item);
			}
			this.SetForwardedAttendees(list);
			this.Subject = base.GetValueOrDefault<string>(InternalSchema.NormalizedSubjectInternal, string.Empty);
		}

		private bool AddAttendee(IAttendeeCollection attendees, Participant organizer, BlobRecipient recipient, ILocationIdentifierSetter locationIdentifierSetter)
		{
			Participant participant = recipient.Participant;
			MailboxSession mailboxSession = base.MailboxSession;
			if (Participant.HasSameEmail(participant, organizer, mailboxSession, true))
			{
				return false;
			}
			foreach (Attendee attendee in attendees)
			{
				if (Participant.HasSameEmail(participant, attendee.Participant, mailboxSession, true))
				{
					return false;
				}
			}
			locationIdentifierSetter.SetLocationIdentifier(43893U);
			attendees.Add(participant, AttendeeType.Optional, null, null, false);
			return true;
		}

		private bool MatchesOrganizerItem(ref CalendarItemBase organizerCalendarItem, out CalendarInconsistencyFlag inconsistencyFlag)
		{
			inconsistencyFlag = CalendarInconsistencyFlag.None;
			int appointmentSequenceNumber = organizerCalendarItem.AppointmentSequenceNumber;
			int valueOrDefault = base.GetValueOrDefault<int>(CalendarItemBaseSchema.AppointmentSequenceNumber, -1);
			if (valueOrDefault < appointmentSequenceNumber)
			{
				inconsistencyFlag = CalendarInconsistencyFlag.VersionInfo;
				return false;
			}
			ExDateTime startTime = organizerCalendarItem.StartTime;
			ExDateTime endTime = organizerCalendarItem.EndTime;
			ExDateTime valueOrDefault2 = base.GetValueOrDefault<ExDateTime>(InternalSchema.StartTime);
			ExDateTime valueOrDefault3 = base.GetValueOrDefault<ExDateTime>(InternalSchema.EndTime);
			if (!startTime.Equals(valueOrDefault2))
			{
				inconsistencyFlag = CalendarInconsistencyFlag.StartTime;
				return false;
			}
			if (!endTime.Equals(valueOrDefault3))
			{
				inconsistencyFlag = CalendarInconsistencyFlag.EndTime;
				return false;
			}
			string location = organizerCalendarItem.Location;
			string valueOrDefault4 = base.GetValueOrDefault<string>(InternalSchema.Location, string.Empty);
			if (!string.Equals(location, valueOrDefault4, StringComparison.OrdinalIgnoreCase))
			{
				inconsistencyFlag = CalendarInconsistencyFlag.Location;
				return false;
			}
			return true;
		}

		protected override bool ShouldBeSentFromOrganizer
		{
			get
			{
				return false;
			}
		}

		protected override void UpdateCalendarItemInternal(ref CalendarItemBase originalCalendarItem)
		{
			if (!this.IsMfnProcessed)
			{
				List<BlobRecipient> forwardedAttendees = this.GetForwardedAttendees();
				IAttendeeCollection attendeeCollection = originalCalendarItem.AttendeeCollection;
				Participant organizer = originalCalendarItem.Organizer;
				List<BlobRecipient> list = new List<BlobRecipient>();
				foreach (BlobRecipient blobRecipient in forwardedAttendees)
				{
					bool flag = this.AddAttendee(attendeeCollection, organizer, blobRecipient, originalCalendarItem.LocationIdentifierHelperInstance);
					if (flag)
					{
						list.Add(blobRecipient);
					}
				}
				if (list.Count > 0)
				{
					BlobRecipientParser.WriteRecipients(this, InternalSchema.MFNAddedRecipients, list);
				}
				this.SetCalendarProcessingSteps(CalendarProcessingSteps.ProcessedMeetingForwardNotification);
			}
		}

		protected internal override int CompareToCalendarItem(CalendarItemBase correlatedCalendarItem)
		{
			return 1;
		}

		internal static readonly StorePropertyDefinition[] MeetingForwardNotificationProperties = new StorePropertyDefinition[]
		{
			InternalSchema.OwnerAppointmentID,
			InternalSchema.GlobalObjectId,
			InternalSchema.CleanGlobalObjectId,
			InternalSchema.OwnerCriticalChangeTime,
			InternalSchema.AppointmentSequenceNumber,
			InternalSchema.Subject,
			InternalSchema.StartTime,
			InternalSchema.EndTime,
			InternalSchema.TimeZone,
			InternalSchema.IsResponseRequested,
			InternalSchema.StartRecurDate,
			InternalSchema.StartRecurTime,
			InternalSchema.AppointmentRecurring,
			InternalSchema.AppointmentRecurrenceBlob,
			InternalSchema.TimeZoneBlob,
			InternalSchema.MapiSensitivity,
			InternalSchema.Location
		};
	}
}
