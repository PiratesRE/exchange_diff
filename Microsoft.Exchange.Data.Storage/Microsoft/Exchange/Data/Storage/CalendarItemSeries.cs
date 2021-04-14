using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class CalendarItemSeries : CalendarItemBase, ICalendarItemSeries, ICalendarItemBase, IItem, IStoreObject, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		internal CalendarItemSeries(ICoreItem coreItem) : base(coreItem)
		{
		}

		public new static CalendarItemSeries Bind(StoreSession session, StoreId storeId)
		{
			return CalendarItemSeries.Bind(session, storeId, null);
		}

		public new static CalendarItemSeries Bind(StoreSession session, StoreId storeId, params PropertyDefinition[] propsToReturn)
		{
			return CalendarItemSeries.Bind(session, storeId, (ICollection<PropertyDefinition>)propsToReturn);
		}

		public new static CalendarItemSeries Bind(StoreSession session, StoreId storeId, ICollection<PropertyDefinition> propsToReturn)
		{
			return ItemBuilder.ItemBind<CalendarItemSeries>(session, storeId, CalendarItemSeriesSchema.Instance, propsToReturn);
		}

		public static CalendarItemSeries CreateSeries(StoreSession session, StoreId parentFolderId, bool forOrganizer = true)
		{
			if (parentFolderId == null)
			{
				throw new ArgumentNullException("parentFolderId");
			}
			CalendarItemSeries result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				CalendarItemSeries calendarItemSeries = ItemBuilder.CreateNewItem<CalendarItemSeries>(session, parentFolderId, ItemCreateInfo.CalendarItemSeriesInfo);
				disposeGuard.Add<CalendarItemSeries>(calendarItemSeries);
				calendarItemSeries.Initialize(forOrganizer);
				disposeGuard.Success();
				result = calendarItemSeries;
			}
			return result;
		}

		public override void CopyToFolder(MailboxSession destinationSession, StoreObjectId destinationFolderId)
		{
			throw new NotImplementedException();
		}

		public override void MoveToFolder(MailboxSession destinationSession, StoreObjectId destinationFolderId)
		{
			throw new NotImplementedException();
		}

		public override string GenerateWhen()
		{
			return string.Empty;
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<CalendarItemSeries>(this);
		}

		public override Schema Schema
		{
			get
			{
				this.CheckDisposed("Schema::get");
				return CalendarItemSeriesSchema.Instance;
			}
		}

		public override int AppointmentLastSequenceNumber
		{
			get
			{
				this.CheckDisposed("AppointmentLastSequenceNumber::get");
				return base.GetValueOrDefault<int>(CalendarItemBaseSchema.AppointmentLastSequenceNumber);
			}
			set
			{
				this.CheckDisposed("AppointmentLastSequenceNumber::set");
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(61813U);
				this[CalendarItemBaseSchema.AppointmentLastSequenceNumber] = value;
			}
		}

		public override ExDateTime StartTime
		{
			get
			{
				this.CheckDisposed("StartTime::get");
				return base.GetValueOrDefault<ExDateTime>(CalendarItemBaseSchema.ClipStartTime);
			}
			set
			{
				this.CheckDisposed("StartTime::set");
				this[CalendarItemBaseSchema.ClipStartTime] = value;
			}
		}

		public override ExDateTime EndTime
		{
			get
			{
				this.CheckDisposed("EndTime::get");
				return base.GetValueOrDefault<ExDateTime>(CalendarItemBaseSchema.ClipEndTime);
			}
			set
			{
				this.CheckDisposed("EndTime::set");
				this[CalendarItemBaseSchema.ClipEndTime] = value;
			}
		}

		public override ExDateTime StartWallClock
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override ExDateTime EndWallClock
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public bool CalendarInteropActionQueueHasData
		{
			get
			{
				this.CheckDisposed("CalendarInteropActionQueueHasData::get");
				return base.GetValueOrDefault<bool>(CalendarItemSeriesSchema.CalendarInteropActionQueueHasData);
			}
		}

		internal override bool IsAttendeeListDirty
		{
			get
			{
				this.FetchAttendeeCollection(false);
				return this.attendees != null && this.attendees.IsDirty;
			}
		}

		internal override bool IsAttendeeListCreated
		{
			get
			{
				this.FetchAttendeeCollection(false);
				return this.attendees != null;
			}
		}

		public int SeriesSequenceNumber
		{
			get
			{
				this.CheckDisposed("SeriesSequenceNumber::get");
				return base.GetValueOrDefault<int>(CalendarItemBaseSchema.AppointmentSequenceNumber, -1);
			}
			set
			{
				this.CheckDisposed("SeriesSequenceNumber::set");
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(34812U);
				this[CalendarItemBaseSchema.AppointmentSequenceNumber] = value;
			}
		}

		internal override void Initialize(bool forOrganizer)
		{
			base.Initialize(forOrganizer);
			this[InternalSchema.ItemClass] = "IPM.AppointmentSeries";
			base.IsHiddenFromLegacyClients = true;
			this[InternalSchema.ConversationIndexTracking] = true;
			if (forOrganizer)
			{
				base.SeriesId = Guid.NewGuid().ToString();
				this.SeriesSequenceNumber = 0;
			}
		}

		protected override void InternalUpdateSequencingProperties(bool isToAllAttendees, MeetingMessage message, int minSequenceNumber, int? seriesSequenceNumber = null)
		{
			if (seriesSequenceNumber != null)
			{
				this.SeriesSequenceNumber = seriesSequenceNumber.Value;
				message[InternalSchema.AppointmentSequenceNumber] = seriesSequenceNumber.Value;
			}
		}

		protected override void SetSequencingPropertiesForForward(MeetingRequest meetingRequest)
		{
			meetingRequest.SeriesSequenceNumber = this.SeriesSequenceNumber;
			meetingRequest[CalendarItemBaseSchema.AppointmentSequenceNumber] = this.SeriesSequenceNumber;
		}

		internal override IAttendeeCollection FetchAttendeeCollection(bool forceOpen)
		{
			this.CheckDisposed("FetchAttendeeCollection");
			if (this.attendees == null)
			{
				CoreRecipientCollection recipientCollection = base.CoreItem.GetRecipientCollection(forceOpen);
				if (recipientCollection != null)
				{
					this.attendees = new AttendeeCollection(recipientCollection);
					base.ResetAttendeeCache();
				}
			}
			return this.attendees;
		}

		protected override void SendMeetingCancellations(MailboxSession mailboxSession, bool isToAllAttendees, IList<Attendee> removedAttendeeList, bool copyToSentItems, bool ignoreSendAsRight, CancellationRumInfo rumInfo)
		{
			if (removedAttendeeList.Count == 0)
			{
				return;
			}
			ExTraceGlobals.MeetingMessageTracer.Information<GlobalObjectId, int>((long)this.GetHashCode(), "Storage.CalendarItemBase.SendMeetingCancellations: GOID={0}; users={1}", base.GlobalObjectId, removedAttendeeList.Count);
			using (MeetingCancellation meetingCancellation = base.CreateMeetingCancellation(mailboxSession, isToAllAttendees, null, null))
			{
				meetingCancellation.CopySendableParticipantsToMessage(removedAttendeeList);
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(33141U, LastChangeAction.SendMeetingCancellations);
				base.SendMessage(mailboxSession, meetingCancellation, copyToSentItems, ignoreSendAsRight);
			}
		}

		protected override void SetDeclineIntent(bool intendToSendResponse)
		{
		}

		protected override MeetingRequest CreateNewMeetingRequest(MailboxSession mailboxSession)
		{
			return MeetingRequest.CreateMeetingRequestSeries(mailboxSession);
		}

		protected override MeetingCancellation CreateNewMeetingCancelation(MailboxSession mailboxSession)
		{
			return MeetingCancellation.CreateMeetingCancellationSeries(mailboxSession);
		}

		protected override MeetingResponse CreateNewMeetingResponse(MailboxSession mailboxSession, ResponseType responseType)
		{
			return MeetingResponse.CreateMeetingResponseSeries(mailboxSession, responseType);
		}

		protected override bool IsInThePast
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		protected override Reminder CreateReminderObject()
		{
			return new CalendarItemSeries.SeriesReminder(this);
		}

		protected override void CopyMeetingRequestProperties(MeetingRequest meetingRequest)
		{
			base.CopyMeetingRequestProperties(meetingRequest);
			CalendarItemBase.CopyPropertiesTo(this, meetingRequest, MeetingMessage.WriteOnCreateSeriesProperties);
		}

		private AttendeeCollection attendees;

		private class SeriesReminder : Reminder
		{
			internal SeriesReminder(CalendarItemSeries item) : base(item)
			{
			}

			public override ExDateTime? DueBy
			{
				get
				{
					return null;
				}
				set
				{
					throw base.PropertyNotSupported("DueBy");
				}
			}

			public override int MinutesBeforeStart
			{
				get
				{
					return base.MinutesBeforeStart;
				}
				set
				{
					base.Item[ItemSchema.ReminderMinutesBeforeStart] = value;
				}
			}

			public override bool IsSet
			{
				get
				{
					return base.Item.GetValueOrDefault<bool>(CalendarItemSeriesSchema.SeriesReminderIsSet);
				}
				set
				{
					base.Item[CalendarItemSeriesSchema.SeriesReminderIsSet] = value;
				}
			}

			protected override void Adjust(ExDateTime actualizationTime)
			{
			}

			protected override Reminder.ReminderInfo GetNextPertinentItemInfo(ExDateTime actualizationTime)
			{
				return null;
			}

			protected override Reminder.ReminderInfo GetPertinentItemInfo(ExDateTime actualizationTime)
			{
				return null;
			}

			public override ExDateTime? ReminderNextTime
			{
				get
				{
					return null;
				}
				protected set
				{
					throw base.PropertyNotSupported("ReminderNextTime");
				}
			}

			public override void Dismiss(ExDateTime actualizationTime)
			{
			}

			public override void Snooze(ExDateTime actualizationTime, ExDateTime snoozeTime)
			{
			}

			public override void Adjust()
			{
			}

			protected internal override void SaveStateAsInitial(bool throwOnFailure)
			{
			}
		}
	}
}
