using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class CalendarItemOccurrence : CalendarItemInstance, ICalendarItemOccurrence, ICalendarItemInstance, ICalendarItemBase, IItem, IStoreObject, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		internal CalendarItemOccurrence(ICoreItem coreItem) : base(coreItem)
		{
			this.usingMasterAttachments = !this.IsException;
		}

		public override Schema Schema
		{
			get
			{
				this.CheckDisposed("Schema::get");
				return CalendarItemOccurrenceSchema.Instance;
			}
		}

		public ExDateTime OriginalStartTime
		{
			get
			{
				this.CheckDisposed("OriginalStartTime::get");
				return this.OccurrencePropertyBag.OriginalStartTime;
			}
		}

		public override bool IsDirty
		{
			get
			{
				return base.IsDirty || this.IsAttendeeListDirty;
			}
		}

		public OccurrencePropertyBag OccurrencePropertyBag
		{
			get
			{
				this.CheckDisposed("OccurrencePropertyBag::get");
				OccurrencePropertyBag occurrencePropertyBag = base.PropertyBag as OccurrencePropertyBag;
				if (occurrencePropertyBag == null)
				{
					occurrencePropertyBag = (OccurrencePropertyBag)((AcrPropertyBag)base.PropertyBag).PropertyBag;
				}
				return occurrencePropertyBag;
			}
		}

		public VersionedId MasterId
		{
			get
			{
				this.CheckDisposed("MasterId::get");
				StoreObjectId storeObjectId = base.StoreObjectId;
				StoreObjectId itemId = StoreObjectId.FromProviderSpecificId(storeObjectId.ProviderLevelItemId, StoreObjectType.CalendarItem);
				return new VersionedId(itemId, base.Id.ChangeKeyAsByteArray());
			}
		}

		public bool IsException
		{
			get
			{
				this.CheckDisposed("IsException::get");
				return this.OccurrencePropertyBag.IsException;
			}
		}

		public override bool IsForwardAllowed
		{
			get
			{
				this.CheckDisposed("IsForwardAllowed::get");
				return base.IsMeeting || this.IsException;
			}
		}

		public override int AppointmentLastSequenceNumber
		{
			get
			{
				this.CheckDisposed("AppointmentLastSequenceNumber::get");
				return this.OccurrencePropertyBag.MasterCalendarItem.AppointmentLastSequenceNumber;
			}
			set
			{
				this.CheckDisposed("AppointmentLastSequenceNumber::set");
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(57343U);
				this.OccurrencePropertyBag.MasterCalendarItem.AppointmentLastSequenceNumber = value;
				this.OccurrencePropertyBag.MasterCalendarItem.LocationIdentifierHelperInstance.SetLocationIdentifier(45055U);
			}
		}

		internal override bool AreAttachmentsDirty
		{
			get
			{
				return base.AreAttachmentsDirty || (this.IsException && this.OccurrencePropertyBag.ExceptionMessage != null && this.OccurrencePropertyBag.ExceptionMessage.AreAttachmentsDirty);
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

		protected override bool IsInThePast
		{
			get
			{
				return this.EndTime < ExDateTime.GetNow(base.PropertyBag.ExTimeZone);
			}
		}

		protected override bool CanDoObjectUpdate
		{
			get
			{
				return this.IsDirty;
			}
		}

		public new static CalendarItemOccurrence Bind(StoreSession session, StoreId storeId)
		{
			return CalendarItemOccurrence.Bind(session, storeId, null);
		}

		public new static CalendarItemOccurrence Bind(StoreSession session, StoreId storeId, ICollection<PropertyDefinition> propsToReturn)
		{
			return ItemBuilder.ItemBind<CalendarItemOccurrence>(session, storeId, CalendarItemOccurrenceSchema.Instance, propsToReturn);
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<CalendarItemOccurrence>(this);
		}

		public CalendarItem GetMaster()
		{
			this.CheckDisposed("GetMaster");
			return CalendarItem.Bind(base.Session, this.MasterId);
		}

		public void MakeModifiedOccurrence()
		{
			this.CheckDisposed("MakeModifiedOccurrence");
			ExTraceGlobals.RecurrenceTracer.Information<int>((long)this.GetHashCode(), "Storage.CalendarItemOccurrence.MakeModifiedOccurrence. HashCode = {0}.", this.GetHashCode());
			OccurrencePropertyBag occurrencePropertyBag = base.PropertyBag as OccurrencePropertyBag;
			if (!this.IsException)
			{
				if (occurrencePropertyBag == null)
				{
					occurrencePropertyBag = (OccurrencePropertyBag)((AcrPropertyBag)base.PropertyBag).PropertyBag;
				}
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(51317U);
				occurrencePropertyBag.MakeException();
			}
		}

		protected override void ValidateForwardArguments(MailboxSession session, StoreObjectId parentFolderId, ReplyForwardConfiguration replyForwardParameters)
		{
			base.ValidateForwardArguments(session, parentFolderId, replyForwardParameters);
			if (!this.IsForwardAllowed)
			{
				throw new InvalidOperationException("A forward can't be created on a read-only calendar item. Call MakeModifiedOccurrence() first.");
			}
		}

		public override string GenerateWhen()
		{
			return CalendarItem.InternalWhen(this, null, false).ToString(base.Session.InternalPreferedCulture);
		}

		public bool IsModifiedProperty(PropertyDefinition propertyDefinition)
		{
			this.CheckDisposed("IsModifiedProperty");
			if (!this.IsException)
			{
				return false;
			}
			SmartPropertyDefinition smartPropertyDefinition = propertyDefinition as SmartPropertyDefinition;
			if (smartPropertyDefinition != null)
			{
				for (int i = 0; i < smartPropertyDefinition.Dependencies.Length; i++)
				{
					PropertyDependency propertyDependency = smartPropertyDefinition.Dependencies[i];
					if ((propertyDependency.Type & PropertyDependencyType.NeedForRead) != PropertyDependencyType.None && this.OccurrencePropertyBag.IsModifiedProperty(propertyDependency.Property))
					{
						return true;
					}
				}
				return false;
			}
			return this.OccurrencePropertyBag.IsModifiedProperty(propertyDefinition);
		}

		public override MeetingResponse RespondToMeetingRequest(ResponseType responseType, string subjectPrefix, ExDateTime? proposedStart = null, ExDateTime? proposedEnd = null)
		{
			ExTraceGlobals.MeetingMessageTracer.Information<GlobalObjectId, ResponseType>((long)this.GetHashCode(), "Storage.CalendarItem.RespondToMeetingRequest: GOID={0}; responseType={1}.", base.GlobalObjectId, responseType);
			return base.RespondToMeetingRequest(responseType, subjectPrefix, proposedStart, proposedEnd);
		}

		protected override void SetDeclineIntent(bool intendToSendResponse)
		{
			this.OccurrencePropertyBag.MasterCalendarItem.ClientIntent = CalendarItemOccurrence.GetDeclineIntent(intendToSendResponse);
		}

		internal static ClientIntentFlags GetDeclineIntent(bool intendToSendResponse)
		{
			if (!intendToSendResponse)
			{
				return ClientIntentFlags.DeletedExceptionWithNoResponse;
			}
			return ClientIntentFlags.RespondedExceptionDecline;
		}

		public override void CopyToFolder(MailboxSession destinationSession, StoreObjectId destinationFolderId)
		{
			this.CheckDisposed("CopyToFolder");
			throw new InvalidOperationException("Occurrences can't be copied to another folder");
		}

		protected override void InitializeMeetingResponse(MeetingResponse meetingResponse, ResponseType responseType, bool isCalendarDelegateAccess, ExDateTime? proposedStart, ExDateTime? proposedEnd)
		{
			base.InitializeMeetingResponse(meetingResponse, responseType, isCalendarDelegateAccess, proposedStart, proposedEnd);
			meetingResponse[InternalSchema.AppointmentRecurrenceBlob] = this.OccurrencePropertyBag.MasterCalendarItem.PropertyBag.GetLargeBinaryProperty(InternalSchema.AppointmentRecurrenceBlob);
		}

		public override void MoveToFolder(MailboxSession destinationSession, StoreObjectId destinationFolderId)
		{
			this.CheckDisposed("MoveToFolder");
			throw new InvalidOperationException("Occurrences can't be moved to another folder");
		}

		internal override IAttendeeCollection FetchAttendeeCollection(bool forceOpen)
		{
			if (this.attendees == null)
			{
				CoreRecipientCollection coreRecipientCollection = null;
				if (this.OccurrencePropertyBag.ExceptionMessage != null)
				{
					coreRecipientCollection = this.OccurrencePropertyBag.ExceptionMessage.CoreItem.GetRecipientCollection(forceOpen);
				}
				CoreRecipientCollection recipientCollection = this.OccurrencePropertyBag.MasterCalendarItem.CoreItem.GetRecipientCollection(forceOpen);
				if (coreRecipientCollection == null && recipientCollection == null)
				{
					return null;
				}
				this.attendees = new OccurrenceAttendeeCollection(this);
				base.ResetAttendeeCache();
			}
			return this.attendees;
		}

		protected override AttachmentCollection FetchAttachmentCollection()
		{
			if (this.usingMasterAttachments && this.IsException)
			{
				base.CoreItem.DisposeAttachmentCollection();
				this.attachmentCollection = null;
			}
			if (this.attachmentCollection == null)
			{
				if (this.IsException)
				{
					this.usingMasterAttachments = false;
					base.CoreItem.OpenAttachmentCollection();
					this.attachmentCollection = new AttachmentCollection(this, false);
				}
				else
				{
					Item masterCalendarItem = this.OccurrencePropertyBag.MasterCalendarItem;
					base.CoreItem.OpenAttachmentCollection(masterCalendarItem.CoreItem);
					this.attachmentCollection = new AttachmentCollection(this, true);
				}
			}
			return this.attachmentCollection;
		}

		protected override Reminders<EventTimeBasedInboxReminder> FetchEventTimeBasedInboxReminders()
		{
			Reminders<EventTimeBasedInboxReminder> reminders = Reminders<EventTimeBasedInboxReminder>.Get(this, CalendarItemBaseSchema.EventTimeBasedInboxReminders);
			if (this.IsException)
			{
				Reminders<EventTimeBasedInboxReminder> reminders2 = Reminders<EventTimeBasedInboxReminder>.Get(this.OccurrencePropertyBag.MasterCalendarItem, CalendarItemBaseSchema.EventTimeBasedInboxReminders);
				if (reminders2 == null)
				{
					return reminders;
				}
				if (reminders == null)
				{
					return reminders2;
				}
				List<EventTimeBasedInboxReminder> list = new List<EventTimeBasedInboxReminder>();
				foreach (EventTimeBasedInboxReminder eventTimeBasedInboxReminder in reminders.ReminderList)
				{
					if (eventTimeBasedInboxReminder.OccurrenceChange == EmailReminderChangeType.Deleted && reminders2.GetReminder(eventTimeBasedInboxReminder.SeriesReminderId) == null)
					{
						list.Add(eventTimeBasedInboxReminder);
					}
				}
				foreach (EventTimeBasedInboxReminder item in list)
				{
					reminders.ReminderList.Remove(item);
				}
				foreach (EventTimeBasedInboxReminder eventTimeBasedInboxReminder2 in reminders2.ReminderList)
				{
					if (EventTimeBasedInboxReminder.GetSeriesReminder(reminders, eventTimeBasedInboxReminder2.Identifier) == null)
					{
						reminders.ReminderList.Add(eventTimeBasedInboxReminder2);
					}
				}
			}
			return reminders;
		}

		protected override void UpdateEventTimeBasedInboxRemindersForSave(Reminders<EventTimeBasedInboxReminder> reminders)
		{
			if (reminders == null)
			{
				return;
			}
			List<EventTimeBasedInboxReminder> list = new List<EventTimeBasedInboxReminder>();
			foreach (EventTimeBasedInboxReminder eventTimeBasedInboxReminder in reminders.ReminderList)
			{
				if (eventTimeBasedInboxReminder.OccurrenceChange == EmailReminderChangeType.None)
				{
					list.Add(eventTimeBasedInboxReminder);
				}
			}
			foreach (EventTimeBasedInboxReminder item in list)
			{
				reminders.ReminderList.Remove(item);
			}
			EventTimeBasedInboxReminder.UpdateIdentifiersForModifiedReminders(reminders);
		}

		protected override void OnAfterSave(ConflictResolutionResult acrResults)
		{
			base.OnAfterSave(acrResults);
			if (!base.IsInMemoryObject)
			{
				base.CoreItem.DisposeAttachmentCollection();
			}
		}

		protected override Reminder CreateReminderObject()
		{
			return new CalendarItemOccurrence.CustomReminder(this);
		}

		protected override void UpdateAttendeesOnException()
		{
			if (this.IsAttendeeListDirty)
			{
				this.MakeModifiedOccurrence();
				this.attendees.ApplyChangesToExceptionAttendeeCollection(base.MapiMessage);
			}
		}

		protected override void OnBeforeSave()
		{
			base.OnBeforeSave();
			this.OnBeforeSaveUpdateExceptionProperties();
			this.attendees = null;
			this.attachmentCollection = null;
		}

		private void OnBeforeSaveUpdateExceptionProperties()
		{
			if (this.IsAttendeeListDirty)
			{
				this[InternalSchema.ExceptionalAttendees] = true;
			}
		}

		private OccurrenceAttendeeCollection attendees;

		private bool usingMasterAttachments;

		private class CustomReminder : Reminder
		{
			internal CustomReminder(CalendarItemOccurrence item) : base(item)
			{
			}

			public override ExDateTime? DueBy
			{
				get
				{
					return base.DueBy;
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
					this.Item[InternalSchema.ReminderMinutesBeforeStart] = value;
				}
			}

			public override ExDateTime? ReminderNextTime
			{
				get
				{
					return this.Master.Reminder.ReminderNextTime;
				}
				protected set
				{
					throw base.PropertyNotSupported("ReminderNextTime::set");
				}
			}

			private bool IsItemStateValid
			{
				get
				{
					ExDateTime? valueAsNullable = this.Item.GetValueAsNullable<ExDateTime>(InternalSchema.StartTime);
					ExDateTime? valueAsNullable2 = this.Item.GetValueAsNullable<ExDateTime>(InternalSchema.EndTime);
					return valueAsNullable != null && valueAsNullable2 != null && valueAsNullable.Value <= valueAsNullable2.Value;
				}
			}

			private new CalendarItemOccurrence Item
			{
				get
				{
					return (CalendarItemOccurrence)base.Item;
				}
			}

			private Item Master
			{
				get
				{
					return this.Item.OccurrencePropertyBag.MasterCalendarItem;
				}
			}

			public override void Adjust()
			{
				this.Item.LocationIdentifierHelperInstance.SetLocationIdentifier(53877U);
				base.Adjust();
			}

			protected internal override void SaveStateAsInitial(bool throwOnFailure)
			{
			}

			protected override void Adjust(ExDateTime actualizationTime)
			{
				if (Reminder.GetDefaultReminderNextTime(new ExDateTime?(this.Item.StartTime), this.MinutesBeforeStart) != null && this.IsItemStateValid)
				{
					this.Item.OccurrencePropertyBag.UpdateMasterRecurrence();
					this.Item.LocationIdentifierHelperInstance.SetLocationIdentifier(37493U);
					Reminder.Adjust(this.Master.Reminder, actualizationTime);
				}
			}

			protected override Reminder.ReminderInfo GetPertinentItemInfo(ExDateTime actualizationTime)
			{
				throw new NotSupportedException();
			}

			protected override Reminder.ReminderInfo GetNextPertinentItemInfo(ExDateTime actualizationTime)
			{
				throw new NotSupportedException();
			}
		}
	}
}
