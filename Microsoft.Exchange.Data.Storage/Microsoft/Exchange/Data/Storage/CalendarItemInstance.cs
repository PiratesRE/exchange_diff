using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class CalendarItemInstance : CalendarItemBase, ICalendarItemInstance, ICalendarItemBase, IItem, IStoreObject, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		internal CalendarItemInstance(ICoreItem coreItem) : base(coreItem)
		{
		}

		public new static CalendarItemInstance Bind(StoreSession session, StoreId storeId)
		{
			return CalendarItemInstance.Bind(session, storeId, null);
		}

		public new static CalendarItemInstance Bind(StoreSession session, StoreId storeId, params PropertyDefinition[] propsToReturn)
		{
			return CalendarItemInstance.Bind(session, storeId, (ICollection<PropertyDefinition>)propsToReturn);
		}

		public new static CalendarItemInstance Bind(StoreSession session, StoreId storeId, ICollection<PropertyDefinition> propsToReturn)
		{
			return ItemBuilder.ItemBind<CalendarItemInstance>(session, storeId, CalendarItemInstanceSchema.Instance, propsToReturn);
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<CalendarItemInstance>(this);
		}

		public override Schema Schema
		{
			get
			{
				this.CheckDisposed("Schema::get");
				return CalendarItemInstanceSchema.Instance;
			}
		}

		public override ExDateTime StartTime
		{
			get
			{
				this.CheckDisposed("StartTime::get");
				object obj = base.TryGetProperty(InternalSchema.StartTime);
				if (obj is ExDateTime)
				{
					return (ExDateTime)obj;
				}
				throw new CorruptDataException(ServerStrings.ExStartTimeNotSet);
			}
			set
			{
				this.CheckDisposed("StartTime::set");
				if (!base.IsInMemoryObject)
				{
					value.CheckExpectedTimeZone(base.Session.ExTimeZone, ExTimeZoneHelperForMigrationOnly.ValidationLevel.VeryHigh);
				}
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(48501U);
				this[InternalSchema.StartTime] = value;
			}
		}

		public override ExDateTime StartWallClock
		{
			get
			{
				this.CheckDisposed("StartWallClock::get");
				return base.GetValueOrDefault<ExDateTime>(CalendarItemInstanceSchema.StartWallClock);
			}
		}

		public override ExDateTime EndWallClock
		{
			get
			{
				this.CheckDisposed("EndWallClock::get");
				return base.GetValueOrDefault<ExDateTime>(CalendarItemInstanceSchema.EndWallClock);
			}
		}

		public override ExDateTime EndTime
		{
			get
			{
				this.CheckDisposed("EndTime::get");
				object obj = base.TryGetProperty(InternalSchema.EndTime);
				if (obj is ExDateTime)
				{
					return (ExDateTime)obj;
				}
				throw new CorruptDataException(ServerStrings.ExEndTimeNotSet);
			}
			set
			{
				this.CheckDisposed("EndTime::set");
				if (!base.IsInMemoryObject)
				{
					value.CheckExpectedTimeZone(base.Session.ExTimeZone, ExTimeZoneHelperForMigrationOnly.ValidationLevel.VeryHigh);
				}
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(64885U);
				this[InternalSchema.EndTime] = value;
			}
		}

		internal override void Initialize(bool newItem)
		{
			base.Initialize(newItem);
			this[InternalSchema.ItemClass] = "IPM.Appointment";
			ExDateTime nextHalfHour = this.GetNextHalfHour();
			this[InternalSchema.StartTime] = nextHalfHour;
			this[InternalSchema.EndTime] = nextHalfHour.AddMinutes(30.0);
			this[InternalSchema.AppointmentRecurring] = false;
			this[InternalSchema.IsRecurring] = false;
			this[InternalSchema.IsException] = false;
			this[InternalSchema.ConversationIndexTracking] = true;
			this[InternalSchema.SideEffects] = (SideEffects.OpenToDelete | SideEffects.CoerceToInbox | SideEffects.OpenToCopy | SideEffects.OpenToMove | SideEffects.OpenForCtxMenu);
		}

		public override void MoveToFolder(MailboxSession destinationSession, StoreObjectId destinationFolderId)
		{
			this.CheckDisposed("MoveToFolder");
			this.CopyMoveToFolder(new CalendarItemInstance.CopyMoveOperation(base.Session.Move), destinationSession, destinationFolderId, false);
		}

		public override void CopyToFolder(MailboxSession destinationSession, StoreObjectId destinationFolderId)
		{
			this.CheckDisposed("CopyToFolder");
			this.CopyMoveToFolder(new CalendarItemInstance.CopyMoveOperation(base.Session.Copy), destinationSession, destinationFolderId, true);
		}

		protected override MeetingRequest CreateNewMeetingRequest(MailboxSession mailboxSession)
		{
			return MeetingRequest.CreateMeetingRequest(mailboxSession);
		}

		protected override MeetingCancellation CreateNewMeetingCancelation(MailboxSession mailboxSession)
		{
			return MeetingCancellation.CreateMeetingCancellation(mailboxSession);
		}

		protected override MeetingResponse CreateNewMeetingResponse(MailboxSession mailboxSession, ResponseType responseType)
		{
			return MeetingResponse.CreateMeetingResponse(mailboxSession, responseType);
		}

		protected override void SendMeetingCancellations(MailboxSession mailboxSession, bool isToAllAttendees, IList<Attendee> removedAttendeeList, bool copyToSentItems, bool ignoreSendAsRight, CancellationRumInfo rumInfo)
		{
			if (removedAttendeeList.Count == 0)
			{
				return;
			}
			ExTraceGlobals.MeetingMessageTracer.Information<GlobalObjectId, int>((long)this.GetHashCode(), "Storage.CalendarItemBase.SendMeetingCancellations: GOID={0}; users={1}", base.GlobalObjectId, removedAttendeeList.Count);
			using (MeetingCancellation meetingCancellation = (rumInfo != null) ? this.CreateCancellationRum(mailboxSession, rumInfo) : base.CreateMeetingCancellation(mailboxSession, isToAllAttendees, null, null))
			{
				meetingCancellation.CopySendableParticipantsToMessage(removedAttendeeList);
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(33141U, LastChangeAction.SendMeetingCancellations);
				base.SendMessage(mailboxSession, meetingCancellation, copyToSentItems, ignoreSendAsRight);
			}
		}

		protected override void SetSequencingPropertiesForForward(MeetingRequest meetingRequest)
		{
			int appointmentLastSequenceNumber = this.AppointmentLastSequenceNumber;
			int valueOrDefault = meetingRequest.GetValueOrDefault<int>(InternalSchema.AppointmentSequenceNumber, appointmentLastSequenceNumber);
			if (valueOrDefault < appointmentLastSequenceNumber)
			{
				meetingRequest[InternalSchema.AppointmentSequenceNumber] = appointmentLastSequenceNumber;
			}
		}

		protected override void InitializeMeetingRequest(Action<MeetingRequest> setBodyAndAdjustFlags, MeetingRequest meetingRequest)
		{
			base.InitializeMeetingRequest(setBodyAndAdjustFlags, meetingRequest);
			Microsoft.Exchange.Data.Storage.Item.CopyCustomPublicStrings(this, meetingRequest);
			this.ClearCounterProposal();
			if (base.CalendarItemType == CalendarItemType.RecurringMaster)
			{
				CalendarItemBase.CopyPropertiesTo(this, meetingRequest, new PropertyDefinition[]
				{
					InternalSchema.AppointmentRecurrenceBlob
				});
			}
		}

		protected override void InternalUpdateSequencingProperties(bool isToAllAttendees, MeetingMessage message, int minSequenceNumber, int? seriesSequenceNumber = null)
		{
			ExDateTime now = ExDateTime.GetNow(base.PropertyBag.ExTimeZone);
			base.OwnerCriticalChangeTime = now;
			int appointmentSequenceNumber = base.AppointmentSequenceNumber;
			int appointmentLastSequenceNumber = this.AppointmentLastSequenceNumber;
			int valueOrDefault = base.GetValueOrDefault<int>(InternalSchema.CdoSequenceNumber);
			int num = Math.Max(minSequenceNumber, Math.Max(appointmentSequenceNumber, Math.Max(appointmentLastSequenceNumber, valueOrDefault)));
			if (base.MeetingRequestWasSent)
			{
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(40959U);
				num++;
			}
			if (isToAllAttendees)
			{
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(51573U);
				this[InternalSchema.AppointmentSequenceTime] = now;
				base.AppointmentSequenceNumber = num;
			}
			this.AppointmentLastSequenceNumber = num;
			if (message != null)
			{
				message[InternalSchema.OwnerCriticalChangeTime] = now;
				message[InternalSchema.AppointmentSequenceNumber] = num;
				ExDateTime? valueAsNullable = base.GetValueAsNullable<ExDateTime>(InternalSchema.AppointmentSequenceTime);
				message[InternalSchema.AppointmentSequenceTime] = ((valueAsNullable != null) ? valueAsNullable.Value : now);
			}
			base.LocationIdentifierHelperInstance.SetLocationIdentifier(37237U, LastChangeAction.UpdateSequenceNumber);
		}

		protected override void InitializeMeetingResponse(MeetingResponse meetingResponse, ResponseType responseType, bool isCalendarDelegateAccess, ExDateTime? proposedStart, ExDateTime? proposedEnd)
		{
			base.InitializeMeetingResponse(meetingResponse, responseType, isCalendarDelegateAccess, proposedStart, proposedEnd);
			if (proposedStart != null && proposedEnd != null)
			{
				this.ValidateTimeProposal(proposedStart.Value, proposedEnd.Value);
				meetingResponse.SetTimeProposal(proposedStart.Value, proposedEnd.Value);
			}
		}

		protected override ClientIntentFlags CalculateClientIntentBasedOnModifiedProperties()
		{
			ClientIntentFlags clientIntentFlags = base.CalculateClientIntentBasedOnModifiedProperties();
			if (base.PropertyBag.IsPropertyDirty(InternalSchema.MapiIsAllDayEvent))
			{
				clientIntentFlags |= ClientIntentFlags.ModifiedTime;
			}
			else
			{
				if (base.PropertyBag.IsPropertyDirty(CalendarItemInstanceSchema.StartTime))
				{
					clientIntentFlags |= ClientIntentFlags.ModifiedStartTime;
				}
				if (base.PropertyBag.IsPropertyDirty(CalendarItemInstanceSchema.EndTime))
				{
					clientIntentFlags |= ClientIntentFlags.ModifiedEndTime;
				}
			}
			return clientIntentFlags;
		}

		protected override void OnBeforeSave()
		{
			base.OnBeforeSave();
			if (!base.IsInMemoryObject)
			{
				this.OnBeforeSaveUpdateStartTimeEndTime();
			}
		}

		private MeetingCancellation CreateCancellationRum(MailboxSession mailboxSession, CancellationRumInfo rumInfo)
		{
			Action<MeetingCancellation> setBodyAndAdjustFlags = delegate(MeetingCancellation cancellation)
			{
				this.AdjustRumMessage(mailboxSession, cancellation, rumInfo, false);
			};
			return base.CreateMeetingCancellation(mailboxSession, false, rumInfo.AttendeeRequiredSequenceNumber, setBodyAndAdjustFlags, false, null, null);
		}

		private void ClearCounterProposal()
		{
			int num = base.GetValueOrDefault<int>(InternalSchema.AppointmentCounterProposalCount);
			ExTraceGlobals.MeetingMessageTracer.Information<GlobalObjectId, int>((long)this.GetHashCode(), "Storage.CalendarItemBase.ClearCounterProposal: GOID={0}; count={1}", base.GlobalObjectId, num);
			base.LocationIdentifierHelperInstance.SetLocationIdentifier(41333U);
			this[InternalSchema.AppointmentCounterProposal] = false;
			base.LocationIdentifierHelperInstance.SetLocationIdentifier(57717U);
			this[InternalSchema.AppointmentCounterProposalCount] = 0;
			if (num > 0)
			{
				foreach (Attendee attendee in base.AttendeeCollection)
				{
					bool valueOrDefault = attendee.GetValueOrDefault<bool>(InternalSchema.RecipientProposed);
					if (valueOrDefault)
					{
						attendee[InternalSchema.RecipientProposed] = false;
						attendee[InternalSchema.RecipientProposedStartTime] = CalendarItemBase.OutlookRtmNone;
						attendee[InternalSchema.RecipientProposedEndTime] = CalendarItemBase.OutlookRtmNone;
						if (--num == 0)
						{
							break;
						}
					}
				}
			}
		}

		private ExDateTime GetNextHalfHour()
		{
			ExDateTime d = ExDateTime.GetNow(base.PropertyBag.ExTimeZone);
			if (d.Minute < 30)
			{
				d = d.AddMinutes((double)(30 - d.Minute));
			}
			else
			{
				d = d.AddMinutes((double)(60 - d.Minute));
			}
			return d - TimeSpan.FromMilliseconds((double)(d.Second * 1000 + d.Millisecond));
		}

		private void CopyMoveToFolder(CalendarItemInstance.CopyMoveOperation operation, MailboxSession destinationSession, StoreObjectId destinationFolderId, bool isCopy)
		{
			this.ThrowIfCopyMovePrereqsFail(destinationSession, destinationFolderId, isCopy);
			operation(destinationSession, destinationFolderId, new StoreId[]
			{
				base.Id
			});
		}

		private void ThrowIfCopyMovePrereqsFail(MailboxSession destinationSession, StoreObjectId destinationFolderId, bool isCopy)
		{
			Util.ThrowOnNullArgument(destinationFolderId, "destinationFolderId");
			if (destinationFolderId.ObjectType != StoreObjectType.CalendarFolder)
			{
				throw new ArgumentException("Destination folder must be a calendar folder", "destinationFolderId");
			}
			if (!(base.Session is MailboxSession))
			{
				throw new InvalidOperationException("Only mailbox sessions are supported");
			}
			if (base.ParentId.Equals(destinationFolderId))
			{
				throw new ArgumentException("The destination folder must be different from the source folder.", "destinationFolderId");
			}
			if (!this.IsInThePast)
			{
				throw new FutureMeetingException("Only meetings in the past can be copied or moved");
			}
			if (isCopy)
			{
				StoreObjectId defaultFolderId = ((MailboxSession)base.Session).GetDefaultFolderId(DefaultFolderType.Calendar);
				if (base.ParentId.Equals(defaultFolderId) || destinationFolderId.Equals(defaultFolderId))
				{
					throw new PrimaryCalendarFolderException("Copy is not allowed to/from the primary calendar");
				}
			}
			using (CalendarFolder calendarFolder = CalendarFolder.Bind(destinationSession, destinationFolderId))
			{
				IList list = CalendarCorrelationMatch.FindMatches(calendarFolder, base.GlobalObjectId, null);
				if (list.Count > 0)
				{
					throw new CalendarItemExistsException("There is already a calendar item with this GOID in the destination folder");
				}
			}
		}

		private void OnBeforeSaveUpdateStartTimeEndTime()
		{
			if (base.IsAllDayEventCache == true)
			{
				ExDateTime? valueAsNullable = base.GetValueAsNullable<ExDateTime>(CalendarItemInstanceSchema.StartTime);
				ExDateTime? valueAsNullable2 = base.GetValueAsNullable<ExDateTime>(CalendarItemInstanceSchema.EndTime);
				if (valueAsNullable != null && valueAsNullable2 != null)
				{
					if (!object.Equals(valueAsNullable.Value, base.TryGetProperty(InternalSchema.MapiStartTime)))
					{
						base.LocationIdentifierHelperInstance.SetLocationIdentifier(35317U);
						this[CalendarItemInstanceSchema.StartTime] = valueAsNullable.Value;
					}
					if (!object.Equals(valueAsNullable2.Value, base.TryGetProperty(InternalSchema.MapiEndTime)))
					{
						base.LocationIdentifierHelperInstance.SetLocationIdentifier(51701U);
						this[CalendarItemInstanceSchema.EndTime] = valueAsNullable2.Value;
					}
				}
			}
		}

		private void ValidateTimeProposal(ExDateTime proposedStart, ExDateTime proposedEnd)
		{
			if (base.CalendarItemType == CalendarItemType.RecurringMaster)
			{
				throw new InvalidTimeProposalException(ServerStrings.ErrorTimeProposalInvalidOnRecurringMaster);
			}
			if (!base.AllowNewTimeProposal)
			{
				throw new InvalidTimeProposalException(ServerStrings.ErrorTimeProposalInvalidWhenNotAllowedByOrganizer);
			}
			if (proposedStart.CompareTo(proposedEnd) > 0)
			{
				throw new InvalidTimeProposalException(ServerStrings.ErrorTimeProposalEndTimeBeforeStartTime);
			}
			TimeSpan timeSpan = proposedEnd - proposedStart;
			if (timeSpan.TotalDays > 1825.0)
			{
				throw new InvalidTimeProposalException(ServerStrings.ErrorTimeProposalInvalidDuration((int)timeSpan.TotalDays));
			}
		}

		private delegate AggregateOperationResult CopyMoveOperation(StoreSession destinationSession, StoreId destinationFolderId, params StoreId[] ids);
	}
}
