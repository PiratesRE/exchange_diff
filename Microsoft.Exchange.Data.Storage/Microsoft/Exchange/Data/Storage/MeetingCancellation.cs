using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MeetingCancellation : MeetingMessageInstance, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		internal MeetingCancellation(ICoreItem coreItem) : base(coreItem)
		{
		}

		internal override void Initialize()
		{
			this.Initialize("IPM.Schedule.Meeting.Canceled");
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<MeetingCancellation>(this);
		}

		public new static MeetingCancellation Bind(StoreSession session, StoreId storeId)
		{
			return MeetingCancellation.Bind(session, storeId, null);
		}

		public new static MeetingCancellation Bind(StoreSession session, StoreId storeId, ICollection<PropertyDefinition> propsToReturn)
		{
			return ItemBuilder.ItemBind<MeetingCancellation>(session, storeId, MeetingMessageInstanceSchema.Instance, propsToReturn);
		}

		public static MeetingCancellation CreateMeetingCancellation(MailboxSession mailboxSession)
		{
			MeetingCancellation meetingCancellation = ItemBuilder.CreateNewItem<MeetingCancellation>(mailboxSession, mailboxSession.GetDefaultFolderId(DefaultFolderType.Drafts), ItemCreateInfo.MeetingCancellationInfo);
			meetingCancellation.Initialize("IPM.Schedule.Meeting.Canceled");
			return meetingCancellation;
		}

		public static MeetingCancellation CreateMeetingCancellationSeries(MailboxSession mailboxSession)
		{
			MeetingCancellation meetingCancellation = ItemBuilder.CreateNewItem<MeetingCancellation>(mailboxSession, mailboxSession.GetDefaultFolderId(DefaultFolderType.Drafts), ItemCreateInfo.MeetingCancellationSeriesInfo);
			meetingCancellation.Initialize("IPM.MeetingMessageSeries.Canceled");
			return meetingCancellation;
		}

		public override MessageItem CreateForward(MailboxSession session, StoreId parentFolderId, ReplyForwardConfiguration configuration)
		{
			this.CheckDisposed("CreateForward");
			Util.ThrowOnNullArgument(session, "session");
			Util.ThrowOnNullArgument(parentFolderId, "parentFolderId");
			Util.ThrowOnNullArgument(configuration, "configuration");
			ExTraceGlobals.MeetingMessageTracer.Information<GlobalObjectId>((long)this.GetHashCode(), "Storage.MeetingCancellation.CreateForward: GOID={0}", this.GlobalObjectId);
			MeetingCancellation meetingCancellation = null;
			bool flag = false;
			MessageItem result;
			try
			{
				meetingCancellation = (base.IsSeriesMessage ? MeetingCancellation.CreateMeetingCancellationSeries(session) : MeetingCancellation.CreateMeetingCancellation(session));
				ForwardCreation forwardCreation = new ForwardCreation(this, meetingCancellation, configuration);
				forwardCreation.PopulateProperties();
				meetingCancellation.AdjustAppointmentStateFlagsForForward();
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(33397U, LastChangeAction.CreateForward);
				flag = true;
				result = meetingCancellation;
			}
			finally
			{
				if (!flag && meetingCancellation != null)
				{
					meetingCancellation.Dispose();
					meetingCancellation = null;
				}
			}
			return result;
		}

		protected override bool ShouldBeSentFromOrganizer
		{
			get
			{
				return true;
			}
		}

		protected override void UpdateCalendarItemInternal(ref CalendarItemBase originalCalendarItem)
		{
			ExTraceGlobals.MeetingMessageTracer.Information<GlobalObjectId>((long)this.GetHashCode(), "Storage.MeetingCancellation.UpdateCalendarItemInternal: GOID={0}", this.GlobalObjectId);
			CalendarItemBase calendarItemBase = originalCalendarItem;
			if (base.IsOutOfDate(calendarItemBase))
			{
				ExTraceGlobals.MeetingMessageTracer.Information<GlobalObjectId>((long)this.GetHashCode(), "Storage.MeetingCancellation.UpdateCalendarItemInternal: GOID={0}; NOOP because message is out of date.", this.GlobalObjectId);
				return;
			}
			calendarItemBase = base.GetCalendarItemToUpdate(calendarItemBase);
			base.AdjustAppointmentState();
			calendarItemBase.LocationIdentifierHelperInstance.SetLocationIdentifier(49781U);
			CalendarItemBase.CopyPropertiesTo(this, calendarItemBase, MeetingMessage.MeetingMessageProperties);
			this.CopyParticipantsToCalendarItem(calendarItemBase);
			string valueOrDefault = base.GetValueOrDefault<string>(InternalSchema.AppointmentClass);
			if (valueOrDefault != null && ObjectClass.IsDerivedClass(valueOrDefault, "IPM.Appointment"))
			{
				calendarItemBase.ClassName = valueOrDefault;
			}
			Microsoft.Exchange.Data.Storage.Item.CopyCustomPublicStrings(this, calendarItemBase);
			if (!base.IsRepairUpdateMessage)
			{
				calendarItemBase.LocationIdentifierHelperInstance.SetLocationIdentifier(65013U);
				Body.CopyBody(this, calendarItemBase, false);
				calendarItemBase.LocationIdentifierHelperInstance.SetLocationIdentifier(56821U);
				base.ReplaceAttachments(calendarItemBase);
			}
			calendarItemBase.FreeBusyStatus = BusyType.Free;
			calendarItemBase.LocationIdentifierHelperInstance.SetLocationIdentifier(40437U);
			calendarItemBase[InternalSchema.AppointmentState] = base.AppointmentState;
			originalCalendarItem = calendarItemBase;
		}

		protected override bool CheckPreConditions(CalendarItemBase originalCalendarItem, bool shouldThrow, bool canUpdatePrincipalCalendar)
		{
			if (!base.CheckPreConditions(originalCalendarItem, shouldThrow, canUpdatePrincipalCalendar))
			{
				return false;
			}
			bool flag = (originalCalendarItem != null) ? originalCalendarItem.IsOrganizer() : base.IsMailboxOwnerTheSender();
			if (!flag)
			{
				return true;
			}
			if (shouldThrow)
			{
				throw new InvalidOperationException(ServerStrings.ExOrganizerCannotCallUpdateCalendarItem);
			}
			return false;
		}

		protected override AppointmentStateFlags CalculatedAppointmentState()
		{
			this.CheckDisposed("CalculatedAppointmentState");
			return base.CalculatedAppointmentState() | AppointmentStateFlags.Cancelled;
		}

		private void CopyParticipantsToCalendarItem(CalendarItemBase calendarItem)
		{
			this.CheckDisposed("CopyParticipantsToCalendarItem");
			base.LocationIdentifierHelperInstance.SetLocationIdentifier(44533U, LastChangeAction.CopyParticipantsToCalendarItem);
			IAttendeeCollection attendeeCollection = calendarItem.AttendeeCollection;
			calendarItem.LocationIdentifierHelperInstance.SetLocationIdentifier(60917U);
			attendeeCollection.Clear();
			if (base.From != null)
			{
				calendarItem.LocationIdentifierHelperInstance.SetLocationIdentifier(36341U);
				attendeeCollection.Add(base.From, AttendeeType.Required, null, null, false).RecipientFlags = (RecipientFlags.Sendable | RecipientFlags.Organizer);
			}
			foreach (Recipient recipient in base.Recipients)
			{
				calendarItem.LocationIdentifierHelperInstance.SetLocationIdentifier(52725U);
				attendeeCollection.Add(recipient.Participant, Attendee.RecipientItemTypeToAttendeeType(recipient.RecipientItemType), null, null, false);
			}
		}

		private void Initialize(string itemClass)
		{
			base.Initialize();
			base.LocationIdentifierHelperInstance.SetLocationIdentifier(39029U);
			this[InternalSchema.ItemClass] = itemClass;
			this[InternalSchema.IconIndex] = IconIndex.AppointmentMeetCancel;
		}
	}
}
