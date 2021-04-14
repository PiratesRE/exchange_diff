using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MeetingResponse : MeetingMessageInstance, IMeetingResponse, IMeetingMessageInstance, IMeetingMessage, IMessageItem, IToDoItem, IItem, IStoreObject, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		internal MeetingResponse(ICoreItem coreItem) : base(coreItem)
		{
		}

		public override Schema Schema
		{
			get
			{
				this.CheckDisposed("Schema::get");
				return MeetingResponseSchema.Instance;
			}
		}

		public ResponseType ResponseType
		{
			get
			{
				this.CheckDisposed("ResponseType::get");
				return (ResponseType)this[MeetingResponseSchema.ResponseType];
			}
		}

		public StoreObjectId AssociatedMeetingRequestId
		{
			get
			{
				this.CheckDisposed("AssociatedMeetingRequestId");
				if (this.AssociatedItemId != null)
				{
					return this.AssociatedItemId.ObjectId;
				}
				return null;
			}
		}

		public ExDateTime AttendeeCriticalChangeTime
		{
			get
			{
				this.CheckDisposed("AttendeeCriticalChangeTime::get");
				return base.GetValueOrDefault<ExDateTime>(CalendarItemBaseSchema.AttendeeCriticalChangeTime, ExDateTime.MinValue);
			}
		}

		public string Location
		{
			get
			{
				this.CheckDisposed("Location::get");
				return (string)this[InternalSchema.Location];
			}
			set
			{
				this.CheckDisposed("Location::set");
				this[InternalSchema.Location] = value;
			}
		}

		public ExDateTime ProposedStart
		{
			get
			{
				this.CheckDisposed("ProposedStart::get");
				return base.GetValueOrDefault<ExDateTime>(InternalSchema.AppointmentCounterStartWhole, CalendarItemBase.OutlookRtmNone);
			}
			private set
			{
				this.CheckDisposed("ProposedStart::set");
				this[InternalSchema.AppointmentCounterStartWhole] = value;
			}
		}

		public ExDateTime ProposedEnd
		{
			get
			{
				this.CheckDisposed("ProposedEnd::get");
				return base.GetValueOrDefault<ExDateTime>(InternalSchema.AppointmentCounterEndWhole, CalendarItemBase.OutlookRtmNone);
			}
			private set
			{
				this.CheckDisposed("ProposedEnd::set");
				this[InternalSchema.AppointmentCounterEndWhole] = value;
			}
		}

		public bool IsCounterProposal
		{
			get
			{
				return this.ProposedStart != CalendarItemBase.OutlookRtmNone && this.ProposedEnd != CalendarItemBase.OutlookRtmNone;
			}
		}

		public bool IsSilent
		{
			get
			{
				return base.GetValueOrDefault<bool>(InternalSchema.IsSilent, false);
			}
		}

		internal void SetTimeProposal(ExDateTime proposedStart, ExDateTime proposedEnd)
		{
			this.CheckDisposed("SetTimeProposal");
			ArgumentValidator.ThrowIfNull("proposedStart", proposedStart);
			ArgumentValidator.ThrowIfNull("proposedEnd", proposedEnd);
			this.ProposedStart = proposedStart;
			this.ProposedEnd = proposedEnd;
			this[InternalSchema.AppointmentCounterProposal] = true;
			this[InternalSchema.AppointmentProposedDuration] = (int)(proposedEnd - proposedStart).TotalMinutes;
		}

		public new static MeetingResponse Bind(StoreSession session, StoreId storeId)
		{
			return MeetingResponse.Bind(session, storeId, null);
		}

		public new static MeetingResponse Bind(StoreSession session, StoreId storeId, ICollection<PropertyDefinition> propsToReturn)
		{
			return ItemBuilder.ItemBind<MeetingResponse>(session, storeId, MeetingResponseSchema.Instance, propsToReturn);
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<MeetingResponse>(this);
		}

		internal static void CoreObjectUpdateIsSilent(CoreItem coreItem)
		{
			if (((ICoreItem)coreItem).AreOptionalAutoloadPropertiesLoaded)
			{
				coreItem.PropertyBag[InternalSchema.IsSilent] = (coreItem.AttachmentCollection.Count == 0 && coreItem.Body.PreviewText.Trim().Length == 0);
			}
		}

		public static MeetingResponse CreateMeetingResponse(MailboxSession mailboxSession, ResponseType responseType)
		{
			MeetingResponse meetingResponse = ItemBuilder.CreateNewItem<MeetingResponse>(mailboxSession, CalendarItemBase.GetDraftsFolderIdOrThrow(mailboxSession), ItemCreateInfo.MeetingResponseInfo);
			meetingResponse.Initialize("IPM.Schedule.Meeting.Resp", responseType);
			return meetingResponse;
		}

		public static MeetingResponse CreateMeetingResponseSeries(MailboxSession mailboxSession, ResponseType responseType)
		{
			MeetingResponse meetingResponse = ItemBuilder.CreateNewItem<MeetingResponse>(mailboxSession, CalendarItemBase.GetDraftsFolderIdOrThrow(mailboxSession), ItemCreateInfo.MeetingResponseSeriesInfo);
			meetingResponse.Initialize("IPM.MeetingMessageSeries.Resp", responseType);
			return meetingResponse;
		}

		internal void Initialize(string itemClassPrefix, ResponseType responseType)
		{
			base.Initialize();
			EnumValidator.AssertValid<ResponseType>(responseType);
			base.LocationIdentifierHelperInstance.SetLocationIdentifier(55413U);
			this[InternalSchema.ItemClass] = MeetingResponse.ItemClassFromResponseType(itemClassPrefix, responseType);
			this[InternalSchema.IconIndex] = IconIndex.AppointmentMeet;
		}

		protected internal override int CompareToCalendarItem(CalendarItemBase correlatedCalendarItem)
		{
			int num = 0;
			Attendee attendee = null;
			bool flag = false;
			if (correlatedCalendarItem != null && base.IsRepairUpdateMessage)
			{
				attendee = this.FindOrAddAttendee(correlatedCalendarItem, out flag);
			}
			if ((!base.IsRepairUpdateMessage || !flag) && correlatedCalendarItem != null && correlatedCalendarItem.Id != null)
			{
				int? valueAsNullable = base.PropertyBag.GetValueAsNullable<int>(InternalSchema.AppointmentSequenceNumber);
				int? valueAsNullable2 = correlatedCalendarItem.PropertyBag.GetValueAsNullable<int>(InternalSchema.AppointmentSequenceNumber);
				if (valueAsNullable != null && valueAsNullable2 != null && valueAsNullable.Value != valueAsNullable2.Value)
				{
					num = valueAsNullable.Value - valueAsNullable2.Value;
				}
				else
				{
					if (!base.IsRepairUpdateMessage)
					{
						attendee = this.FindOrAddAttendee(correlatedCalendarItem, out flag);
					}
					if (attendee != null)
					{
						ExDateTime valueOrDefault = base.GetValueOrDefault<ExDateTime>(InternalSchema.AttendeeCriticalChangeTime, base.SentTime);
						num = ExDateTime.Compare(valueOrDefault, attendee.ReplyTime, Util.DateTimeComparisonRange);
					}
				}
			}
			if (num < 0)
			{
				ExTraceGlobals.MeetingMessageTracer.Information<GlobalObjectId>((long)this.GetHashCode(), "Storage.MeetingMessage.IsOutOfDate: GOID={0}; isOutOfDate=true", this.GlobalObjectId);
			}
			return num;
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
			ExTraceGlobals.MeetingMessageTracer.Information<GlobalObjectId>((long)this.GetHashCode(), "Storage.MeetingResponse.UpdateCalendarItemInternal: GOID={0}", this.GlobalObjectId);
			CalendarItemBase calendarItemBase = originalCalendarItem;
			bool flag;
			Attendee attendee = this.FindOrAddAttendee(calendarItemBase, out flag);
			if (attendee == null)
			{
				return;
			}
			if (this.IsAttendeeResponseOutOfDate(attendee))
			{
				ExTraceGlobals.MeetingMessageTracer.Information<GlobalObjectId>((long)this.GetHashCode(), "Storage.MeetingResponse.UpdateCalendarItemInternal: GOID={0}. Old Response, doing nothing", this.GlobalObjectId);
				return;
			}
			attendee.ResponseType = this.ResponseType;
			ExDateTime exDateTime = base.GetValueOrDefault<ExDateTime>(InternalSchema.AttendeeCriticalChangeTime, ExDateTime.MinValue);
			if (exDateTime == ExDateTime.MinValue)
			{
				exDateTime = base.SentTime;
			}
			if (exDateTime != ExDateTime.MinValue)
			{
				attendee.ReplyTime = exDateTime;
			}
			if (!base.IsSeriesMessage)
			{
				CalendarItemType calendarItemType = calendarItemBase.CalendarItemType;
				if (calendarItemType == CalendarItemType.Exception || calendarItemType == CalendarItemType.Occurrence)
				{
					attendee.RecipientFlags |= RecipientFlags.ExceptionalResponse;
				}
				this.ProcessCounterProposal(calendarItemBase, attendee);
			}
		}

		protected override bool CheckPreConditions(CalendarItemBase originalCalendarItem, bool shouldThrow, bool canUpdatePrincipalCalendar)
		{
			if (!base.CheckPreConditions(originalCalendarItem, shouldThrow, canUpdatePrincipalCalendar))
			{
				return false;
			}
			if (originalCalendarItem == null)
			{
				ExTraceGlobals.MeetingMessageTracer.Information<GlobalObjectId>((long)this.GetHashCode(), "Storage.MeetingResponse.CheckPreConditions: GOID={0}; there is no calendar item; returning false.", this.GlobalObjectId);
				return false;
			}
			if (originalCalendarItem.IsOrganizer())
			{
				return true;
			}
			if (shouldThrow)
			{
				throw new InvalidOperationException(ServerStrings.ExCannotUpdateResponses);
			}
			ExTraceGlobals.MeetingMessageTracer.Information<GlobalObjectId>((long)this.GetHashCode(), "Storage.MeetingResponse.CheckPreConditions: GOID={0}; not organizer; returning false.", this.GlobalObjectId);
			return false;
		}

		private static string ItemClassFromResponseType(string itemClassPrefix, ResponseType responseType)
		{
			bool flag = ObjectClass.IsMeetingResponseSeries(itemClassPrefix);
			string result;
			switch (responseType)
			{
			case ResponseType.Tentative:
				result = (flag ? "IPM.MeetingMessageSeries.Resp.Tent" : "IPM.Schedule.Meeting.Resp.Tent");
				break;
			case ResponseType.Accept:
				result = (flag ? "IPM.MeetingMessageSeries.Resp.Pos" : "IPM.Schedule.Meeting.Resp.Pos");
				break;
			case ResponseType.Decline:
				result = (flag ? "IPM.MeetingMessageSeries.Resp.Neg" : "IPM.Schedule.Meeting.Resp.Neg");
				break;
			default:
				throw new ArgumentException(ServerStrings.ExUnknownResponseType, "responseType");
			}
			return result;
		}

		private void ProcessCounterProposal(CalendarItemBase calendarItem, Attendee attendee)
		{
			base.LocationIdentifierHelperInstance.SetLocationIdentifier(57973U, LastChangeAction.ProcessCounterProposal);
			bool valueOrDefault = base.GetValueOrDefault<bool>(InternalSchema.AppointmentCounterProposal);
			int num = calendarItem.GetValueOrDefault<int>(InternalSchema.AppointmentCounterProposalCount);
			bool valueOrDefault2 = attendee.GetValueOrDefault<bool>(InternalSchema.RecipientProposed);
			if (valueOrDefault)
			{
				attendee[InternalSchema.RecipientProposed] = true;
				attendee[InternalSchema.RecipientProposedStartTime] = this.ProposedStart;
				attendee[InternalSchema.RecipientProposedEndTime] = this.ProposedEnd;
				if (!valueOrDefault2)
				{
					ExTraceGlobals.MeetingMessageTracer.Information<GlobalObjectId, string>((long)this.GetHashCode(), "Storage.MeetingResponse.ProcessCounterProposal: GOID={0}; new counter proposal from {1}.", this.GlobalObjectId, attendee.Participant.DisplayName);
					num++;
				}
				else
				{
					ExTraceGlobals.MeetingMessageTracer.Information<GlobalObjectId, string>((long)this.GetHashCode(), "Storage.MeetingResponse.ProcessCounterProposal: GOID={0}; updated counter proposal from {1}.", this.GlobalObjectId, attendee.Participant.DisplayName);
				}
				calendarItem[InternalSchema.AppointmentCounterProposal] = true;
				calendarItem[InternalSchema.AppointmentCounterProposalCount] = num;
			}
			else if (valueOrDefault2)
			{
				ExTraceGlobals.MeetingMessageTracer.Information<GlobalObjectId, string>((long)this.GetHashCode(), "Storage.MeetingResponse.ProcessCounterProposal: GOID={0}; reseting counter proposal from {1}.", this.GlobalObjectId, attendee.Participant.DisplayName);
				attendee[InternalSchema.RecipientProposed] = false;
				attendee[InternalSchema.RecipientProposedStartTime] = CalendarItemBase.OutlookRtmNone;
				attendee[InternalSchema.RecipientProposedEndTime] = CalendarItemBase.OutlookRtmNone;
				if (num > 0)
				{
					ExTraceGlobals.MeetingMessageTracer.Information<GlobalObjectId>((long)this.GetHashCode(), "Storage.MeetingResponse.ProcessCounterProposal: GOID={0}; no more counter proposals.", this.GlobalObjectId);
					num--;
				}
				calendarItem[InternalSchema.AppointmentCounterProposalCount] = num;
				if (num == 0)
				{
					calendarItem[InternalSchema.AppointmentCounterProposal] = false;
				}
			}
			this.SetCalendarProcessingSteps(CalendarProcessingSteps.CounterProposalSet);
		}

		private bool IsAttendeeResponseOutOfDate(Attendee attendee)
		{
			ExDateTime valueOrDefault = base.GetValueOrDefault<ExDateTime>(InternalSchema.AttendeeCriticalChangeTime, base.SentTime);
			bool flag = ExDateTime.Compare(valueOrDefault, attendee.ReplyTime, Util.DateTimeComparisonRange) < 0;
			if (flag)
			{
				ExTraceGlobals.MeetingMessageTracer.Information<GlobalObjectId>((long)this.GetHashCode(), "Storage.MeetingResponse.IsAttendeeResponseOutOfDate: GOID={0}; response is out of date.", this.GlobalObjectId);
			}
			return flag;
		}

		private Attendee FindOrAddAttendee(CalendarItemBase calendarItem, out bool newAttendee)
		{
			IAttendeeCollection attendeeCollection = calendarItem.AttendeeCollection;
			Attendee attendee = base.FindAttendee(calendarItem);
			if (attendee == null && attendeeCollection != null && base.From != null)
			{
				ExTraceGlobals.MeetingMessageTracer.Information((long)this.GetHashCode(), "User not found, adding to the list.");
				attendee = attendeeCollection.Add(base.From, AttendeeType.Optional, null, null, false);
				newAttendee = true;
			}
			else
			{
				newAttendee = false;
			}
			return attendee;
		}

		internal const string YouCannotUpdateResponses = "Cannot update calendar item with responses when you are not the organizer.";
	}
}
