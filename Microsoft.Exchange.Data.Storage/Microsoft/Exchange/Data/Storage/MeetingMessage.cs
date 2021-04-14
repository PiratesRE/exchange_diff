using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class MeetingMessage : MessageItem, IMeetingMessage, IMessageItem, IToDoItem, IItem, IStoreObject, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		public bool IsSeriesMessage
		{
			get
			{
				return ObjectClass.IsMeetingMessageSeries(this.ClassName);
			}
		}

		internal MeetingMessage(ICoreItem coreItem) : base(coreItem, false)
		{
		}

		public static bool IsDelegateOnlyFromSession(MailboxSession session)
		{
			DelegateRuleType? delegateRuleType;
			return MeetingMessage.TryGetDelegateRuleTypeFromSession(session, out delegateRuleType) && delegateRuleType == DelegateRuleType.ForwardAndDelete;
		}

		public override Schema Schema
		{
			get
			{
				this.CheckDisposed("Schema::get");
				return MeetingMessageSchema.Instance;
			}
		}

		public new static MeetingMessage Bind(StoreSession session, StoreId storeId)
		{
			return MeetingMessage.Bind(session, storeId, null);
		}

		public new static MeetingMessage Bind(StoreSession session, StoreId storeId, params PropertyDefinition[] propsToReturn)
		{
			return MeetingMessage.Bind(session, storeId, (ICollection<PropertyDefinition>)propsToReturn);
		}

		public new static MeetingMessage Bind(StoreSession session, StoreId storeId, ICollection<PropertyDefinition> propsToReturn)
		{
			return ItemBuilder.ItemBind<MeetingMessage>(session, storeId, MeetingMessageSchema.Instance, propsToReturn);
		}

		public static bool IsFromExternalParticipant(string routingType)
		{
			return !string.Equals("EX", routingType, StringComparison.OrdinalIgnoreCase);
		}

		public bool IsArchiveMigratedMessage
		{
			get
			{
				ExDateTime valueOrDefault = base.GetValueOrDefault<ExDateTime>(MeetingMessageSchema.EHAMigrationExpirationDate);
				return !(valueOrDefault == ExDateTime.MinValue);
			}
		}

		protected MailboxSession MailboxSession
		{
			get
			{
				return base.Session as MailboxSession;
			}
		}

		public bool IsExternalMessage
		{
			get
			{
				this.CheckDisposed("IsExternalMessage::get");
				string valueOrDefault = base.GetValueOrDefault<string>(ItemSchema.SentRepresentingType);
				return MeetingMessage.IsFromExternalParticipant(valueOrDefault);
			}
		}

		public bool IsDelegated()
		{
			MailboxSession mailboxSession = this.MailboxSession;
			if (mailboxSession == null)
			{
				return false;
			}
			Participant participant = new Participant(null, mailboxSession.MailboxOwnerLegacyDN, "EX");
			Participant valueOrDefault = base.GetValueOrDefault<Participant>(InternalSchema.ReceivedBy);
			return valueOrDefault != null && this.ReceivedRepresenting != null && Participant.HasSameEmail(valueOrDefault, participant, mailboxSession, true) && !Participant.HasSameEmail(valueOrDefault, this.ReceivedRepresenting, mailboxSession, true);
		}

		public Participant ReceivedRepresenting
		{
			get
			{
				this.CheckDisposed("ReceivedRepresenting::get");
				return base.GetValueOrDefault<Participant>(InternalSchema.ReceivedRepresenting);
			}
		}

		public string CalendarOriginatorId
		{
			get
			{
				this.CheckDisposed("CalendarOriginatorId::get");
				return base.GetValueOrDefault<string>(InternalSchema.CalendarOriginatorId);
			}
		}

		public string SeriesId
		{
			get
			{
				this.CheckDisposed("SeriesId::get");
				return base.GetValueOrDefault<string>(MeetingMessageSchema.SeriesId, string.Empty);
			}
			set
			{
				this.CheckDisposed("SeriesId::set");
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(65532U);
				this[InternalSchema.SeriesId] = value;
			}
		}

		public int SeriesSequenceNumber
		{
			get
			{
				this.CheckDisposed("SeriesSequenceNumber::get");
				return base.GetValueOrDefault<int>(MeetingMessageSchema.SeriesSequenceNumber, -1);
			}
			set
			{
				this.CheckDisposed("SeriesSequenceNumber::set");
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(40956U);
				this[MeetingMessageSchema.SeriesSequenceNumber] = value;
			}
		}

		public virtual CalendarItemBase GetCorrelatedItem()
		{
			throw new NotImplementedException();
		}

		public virtual CalendarItemBase GetCorrelatedItem(bool shouldDetectDuplicateIds, out IEnumerable<VersionedId> detectedDuplicatesIds)
		{
			throw new NotImplementedException();
		}

		public virtual CalendarItemBase GetCachedCorrelatedItem()
		{
			throw new NotImplementedException();
		}

		public virtual CalendarItemBase TryGetCorrelatedItemFromHintId(MailboxSession session, StoreObjectId hintId)
		{
			throw new NotImplementedException();
		}

		public virtual bool IsProcessed
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public bool IsRepairUpdateMessage
		{
			get
			{
				AppointmentAuxiliaryFlags valueOrDefault = base.GetValueOrDefault<AppointmentAuxiliaryFlags>(MeetingMessageSchema.AppointmentAuxiliaryFlags);
				return (valueOrDefault & AppointmentAuxiliaryFlags.RepairUpdateMessage) != (AppointmentAuxiliaryFlags)0;
			}
		}

		public bool CalendarProcessed
		{
			get
			{
				this.CheckDisposed("CalendarProcessed::get");
				return base.GetValueOrDefault<bool>(InternalSchema.CalendarProcessed);
			}
			set
			{
				this.CheckDisposed("CalendarProcessed::set");
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(52853U);
				this[InternalSchema.CalendarProcessed] = value;
			}
		}

		public virtual CalendarItemBase GetEmbeddedItem()
		{
			throw new NotImplementedException();
		}

		public virtual CalendarItemBase GetCachedEmbeddedItem()
		{
			throw new NotImplementedException();
		}

		public virtual bool TryUpdateCalendarItem(ref CalendarItemBase originalCalendarItem, bool canUpdatePrincipalCalendar)
		{
			throw new NotImplementedException();
		}

		public virtual CalendarItemBase UpdateCalendarItem(bool canUpdatePrincipalCalendar)
		{
			throw new NotImplementedException();
		}

		protected static bool TryGetDelegateRuleTypeFromSession(MailboxSession session, out DelegateRuleType? ruleType)
		{
			bool result = false;
			ruleType = null;
			if (session != null && session.Capabilities.CanHaveDelegateUsers)
			{
				try
				{
					DelegateUserCollection delegateUserCollection = new DelegateUserCollection(session);
					if (delegateUserCollection.Count > 0)
					{
						ruleType = new DelegateRuleType?(delegateUserCollection.DelegateRuleType);
					}
					result = true;
				}
				catch (DelegateUserNoFreeBusyFolderException)
				{
					ExTraceGlobals.MeetingMessageTracer.Information((long)session.GetHashCode(), "Storage.MeetingMessage.TryGetDelegateType: NoFreeBusyData Folder, failing to get delegate rule type.");
				}
				catch (ObjectNotFoundException)
				{
					ExTraceGlobals.MeetingMessageTracer.Information((long)session.GetHashCode(), "Storage.MeetingMessage.TryGetDelegateType: No delegates found, failing to get delegate rule type.");
				}
			}
			return result;
		}

		public bool IsOutOfDate()
		{
			this.CheckDisposed("IsOutOfDate");
			ExTraceGlobals.MeetingMessageTracer.Information((long)this.GetHashCode(), "Storage.MeetingMessage.IsOutOfDate");
			MeetingMessageType? valueAsNullable = base.GetValueAsNullable<MeetingMessageType>(InternalSchema.MeetingRequestType);
			if (valueAsNullable != null && valueAsNullable.Value == MeetingMessageType.Outdated)
			{
				return true;
			}
			CalendarItemBase correlatedItemInternal = this.GetCorrelatedItemInternal(true);
			return this.IsOutOfDate(correlatedItemInternal);
		}

		public bool IsOutOfDate(CalendarItemBase correlatedCalendarItem)
		{
			this.CheckDisposed("IsOutOfDate");
			return this.IsOutOfDateInternal(correlatedCalendarItem);
		}

		public virtual string GenerateWhen(CultureInfo preferedCulture, ExTimeZone preferredTimeZone)
		{
			throw new NotImplementedException();
		}

		public string GenerateWhen(CultureInfo preferedCulture)
		{
			return this.GenerateWhen(preferedCulture, null);
		}

		public string GenerateWhen()
		{
			return this.GenerateWhen(base.Session.InternalPreferedCulture);
		}

		public static void SendLocalOrRemote(MessageItem messageItem, bool copyToSentItems = true, bool ignoreSendAsRight = true)
		{
			MailboxSession mailboxSession = messageItem.Session as MailboxSession;
			if (mailboxSession != null && mailboxSession.MailboxOwner.MailboxInfo.IsAggregated && mailboxSession.ActivitySession != null)
			{
				messageItem.From = CalendarItemBase.GetAggregatedOwner(mailboxSession);
				if (messageItem.StoreObjectId == null)
				{
					messageItem.Save(SaveMode.NoConflictResolution);
					messageItem.Load();
				}
				messageItem.RemoteSend();
				return;
			}
			SubmitMessageFlags submitFlags = ignoreSendAsRight ? SubmitMessageFlags.IgnoreSendAsRight : SubmitMessageFlags.None;
			if (copyToSentItems)
			{
				messageItem.Send(submitFlags);
				return;
			}
			messageItem.SendWithoutSavingMessage(submitFlags);
		}

		public override void SetFlag(string flagRequest, ExDateTime? startDate, ExDateTime? dueDate)
		{
			this.CheckDisposed("SetFlag");
			base.LocationIdentifierHelperInstance.SetLocationIdentifier(40565U, LastChangeAction.SetFlag);
			base.SetFlagInternal(startDate, dueDate);
		}

		public override object this[PropertyDefinition propertyDefinition]
		{
			get
			{
				return base[propertyDefinition];
			}
			set
			{
				if (InternalSchema.FlagRequest.Equals(propertyDefinition))
				{
					ExTraceGlobals.MeetingMessageTracer.TraceError<string>((long)this.GetHashCode(), "MeetingMessage::Indexer. Property cannot be set for meeting requests. {0}", propertyDefinition.Name);
					return;
				}
				base[propertyDefinition] = value;
			}
		}

		public virtual bool IsRecurringMaster
		{
			get
			{
				return false;
			}
		}

		public bool IsOrganizer()
		{
			this.CheckDisposed("IsOrganizer");
			bool valueOrDefault = base.GetValueOrDefault<bool>(InternalSchema.IsOrganizer);
			ExTraceGlobals.MeetingMessageTracer.Information<GlobalObjectId, bool>((long)this.GetHashCode(), "Storage.MeetingMessage.IsOrganizer: GOID={0}. IsOrganizer={1}", this.GlobalObjectId, valueOrDefault);
			return valueOrDefault;
		}

		public virtual CalendarItemBase PreProcess(bool createNewItem, bool processExternal, int defaultReminderMinutes)
		{
			throw new NotImplementedException();
		}

		public void MarkAsOutOfDate()
		{
			this.CheckDisposed("MarkAsOutOfDate");
			ExTraceGlobals.MeetingMessageTracer.Information((long)this.GetHashCode(), "Storage.MeetingMessage.MarkAsOutOfDate");
			if (this.IsRepairUpdateMessage || this is MeetingRequest || this is MeetingCancellation)
			{
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(61045U);
				this[InternalSchema.MeetingRequestType] = MeetingMessageType.Outdated;
			}
		}

		public void MarkAsHijacked()
		{
			this.CheckDisposed("MarkAsHijacked");
			ExTraceGlobals.MeetingMessageTracer.Information((long)this.GetHashCode(), "Storage.MeetingMessage.MarkAsHijacked");
			this.MarkAsOutOfDate();
			if (this.IsRepairUpdateMessage || this is MeetingRequest || this is MeetingCancellation)
			{
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(46108U);
				this[InternalSchema.OriginalMeetingType] = MeetingMessageType.Outdated;
				this[InternalSchema.AppointmentStateInternal] = 0;
				this[InternalSchema.HijackedMeeting] = true;
			}
		}

		public bool VerifyCalendarOriginatorId(MailboxSession itemStore, CalendarItemBase calendarItem, string internetMessageId, out string participantMatchFailure)
		{
			participantMatchFailure = null;
			if (calendarItem == null)
			{
				return true;
			}
			if (!(this is MeetingRequest) && !(this is MeetingCancellation))
			{
				return true;
			}
			string valueOrDefault = base.GetValueOrDefault<string>(InternalSchema.CalendarOriginatorId);
			string valueOrDefault2 = calendarItem.GetValueOrDefault<string>(InternalSchema.CalendarOriginatorId);
			int? num = CalendarOriginatorIdProperty.Compare(valueOrDefault2, valueOrDefault);
			if (num != null)
			{
				if (num == 0)
				{
					return true;
				}
				if (num > 0)
				{
					ExTraceGlobals.MeetingMessageTracer.Information((long)this.GetHashCode(), "Calendar originator Id on the meeting request - {0} partially matches the originator Id on the calendar item - {1}. InternetMessageId = {2}, Mailbox = {3}.", new object[]
					{
						valueOrDefault,
						valueOrDefault2,
						internetMessageId,
						itemStore.MailboxOwnerLegacyDN
					});
					return true;
				}
				return false;
			}
			else
			{
				ExTraceGlobals.MeetingMessageTracer.Information((long)this.GetHashCode(), "Calendar originator Id on the meeting request - {0} does not match the originator Id on the calendar item - {1}. InternetMessageId = {2}, Mailbox = {3}. Verifying From.", new object[]
				{
					valueOrDefault,
					valueOrDefault2,
					internetMessageId,
					itemStore.MailboxOwnerLegacyDN
				});
				Participant organizer = calendarItem.Organizer;
				Participant from = base.From;
				if (!(organizer != null))
				{
					participantMatchFailure = "Organizer on the calendar item is not set.";
					return true;
				}
				if (Participant.HasSameEmail(organizer, from))
				{
					ExTraceGlobals.MeetingMessageTracer.Information((long)this.GetHashCode(), "From on the meeting request - {0} matches Organizer on the calendar item - {1}. InternetMessageId = {2}, Mailbox = {3}.", new object[]
					{
						from,
						organizer,
						internetMessageId,
						itemStore.MailboxOwnerLegacyDN
					});
					return true;
				}
				IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.PartiallyConsistent, Participant.BatchBuilder.GetADSessionSettings(itemStore), 870, "VerifyCalendarOriginatorId", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\Calendar\\MeetingMessage.cs");
				ADRecipient adrecipient = null;
				if (from.TryGetADRecipient(tenantOrRootOrgRecipientSession, out adrecipient) && adrecipient != null)
				{
					if (Participant.HasProxyAddress(adrecipient, organizer))
					{
						ExTraceGlobals.MeetingMessageTracer.Information((long)this.GetHashCode(), "HasProxyAddress succeeded - From: {0} on calendar item matches From on the meeting request - {1}. InternetMessageId = {2}, Mailbox = {3}.", new object[]
						{
							organizer,
							from,
							internetMessageId,
							itemStore.MailboxOwnerLegacyDN
						});
						return true;
					}
					participantMatchFailure = string.Format("Particpant match failure, Failed to match proxyAddresses. Calendar.From={0}, mtg.From={1}", organizer, from);
				}
				else
				{
					participantMatchFailure = string.Format("Particpant match failure, Failed to get the AD Recipient. Calendar.From={0}, mtg.From={1}", organizer, from);
				}
				return true;
			}
		}

		internal void CopySendableParticipantsToMessage(IList<Attendee> attendeeList)
		{
			foreach (Attendee attendee in attendeeList)
			{
				if (attendee.IsSendable())
				{
					base.Recipients.Add(attendee.Participant, Attendee.AttendeeTypeToRecipientItemType(attendee.AttendeeType));
				}
			}
		}

		internal virtual CalendarItemBase GetCorrelatedItemInternal(bool cache)
		{
			IEnumerable<VersionedId> enumerable;
			return this.GetCorrelatedItemInternal(cache, false, out enumerable);
		}

		internal virtual CalendarItemBase GetCorrelatedItemInternal(bool cache, bool shouldDetectDuplicateIds, out IEnumerable<VersionedId> detectedDuplicatesIds)
		{
			throw new NotImplementedException();
		}

		protected internal virtual CalendarItemOccurrence RecoverDeletedOccurrence()
		{
			throw new NotImplementedException();
		}

		protected static MailboxSession GetCalendarMailboxSession(MeetingMessage meetingMessage)
		{
			if (!(meetingMessage.Session is MailboxSession))
			{
				throw new NotSupportedException();
			}
			MailboxSession mailboxSession = (MailboxSession)meetingMessage.Session;
			if (meetingMessage.IsDelegated())
			{
				Participant valueOrDefault = meetingMessage.GetValueOrDefault<Participant>(InternalSchema.ReceivedRepresenting);
				ExchangePrincipal principal = MeetingMessage.InternalGetExchangePrincipal(valueOrDefault, mailboxSession);
				mailboxSession = mailboxSession.InternalGetDelegateSessionEntry(principal, OpenBy.Internal).MailboxSession;
			}
			return mailboxSession;
		}

		protected abstract void UpdateCalendarItemInternal(ref CalendarItemBase originalCalendarItem);

		protected virtual bool IsOutOfDateInternal(CalendarItemBase correlatedCalendarItem)
		{
			return this.CompareToCalendarItem(correlatedCalendarItem) < 0;
		}

		protected internal virtual int CompareToCalendarItem(CalendarItemBase correlatedCalendarItem)
		{
			throw new NotImplementedException();
		}

		protected virtual bool CheckPreConditions(CalendarItemBase originalCalendarItem, bool shouldThrow, bool canUpdatePrincipalCalendar)
		{
			throw new NotImplementedException();
		}

		protected CalendarItemBase GetCalendarItemToUpdate(CalendarItemBase correlatedCalendarItem)
		{
			CalendarItemBase calendarItemBase = correlatedCalendarItem;
			if (calendarItemBase == null)
			{
				MailboxSession calendarMailboxSession = MeetingMessage.GetCalendarMailboxSession(this);
				if ((calendarItemBase = this.RecoverDeletedOccurrence()) == null)
				{
					ExTraceGlobals.MeetingMessageTracer.Information<GlobalObjectId>((long)this.GetHashCode(), "Storage.MeetingMessage.GetCalendarItemToUpdate: GOID={0}. Creating calendar item", this.GlobalObjectId);
					StoreObjectId parentFolderId = calendarMailboxSession.SafeGetDefaultFolderId(DefaultFolderType.Calendar);
					calendarItemBase = (this.IsSeriesMessage ? CalendarItemSeries.CreateSeries(calendarMailboxSession, parentFolderId, false) : CalendarItem.CreateCalendarItem(calendarMailboxSession, parentFolderId, false));
				}
				else
				{
					ExTraceGlobals.MeetingMessageTracer.Information<GlobalObjectId>((long)this.GetHashCode(), "Storage.MeetingMessage.GetCalendarItemToUpdate: GOID={0}. Recovered deleted occurrence.", this.GlobalObjectId);
				}
			}
			return calendarItemBase;
		}

		internal Attendee FindAttendee(CalendarItemBase calendarItem)
		{
			this.CheckDisposed("FindAttendee");
			IAttendeeCollection attendeeCollection = calendarItem.AttendeeCollection;
			bool flag = false;
			Attendee attendee = null;
			bool flag2 = false;
			if (this.GetFromRecipient() == null)
			{
				if (base.From == null)
				{
					return null;
				}
				flag2 = true;
			}
			for (int i = 0; i < attendeeCollection.Count; i++)
			{
				attendee = attendeeCollection[i];
				bool flag3;
				if (flag2)
				{
					flag3 = base.From.AreAddressesEqual(attendee.Participant);
				}
				else
				{
					flag3 = Participant.HasProxyAddress(this.adFromRecipient, attendee.Participant);
				}
				if (flag3)
				{
					ExTraceGlobals.MeetingMessageTracer.Information((long)this.GetHashCode(), "User found in attendee list.");
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return null;
			}
			return attendee;
		}

		protected override Reminder CreateReminderObject()
		{
			return null;
		}

		internal void AdjustAppointmentStateFlagsForForward()
		{
			base.LocationIdentifierHelperInstance.SetLocationIdentifier(55925U);
			this.AdjustAppointmentState();
			int? num = base.GetValueAsNullable<int>(InternalSchema.AppointmentAuxiliaryFlags);
			if (num != null)
			{
				num |= 4;
			}
			else
			{
				num = new int?(4);
			}
			base.LocationIdentifierHelperInstance.SetLocationIdentifier(43637U);
			this[InternalSchema.AppointmentAuxiliaryFlags] = num;
		}

		public bool IsMailboxOwnerTheSender()
		{
			this.CheckDisposed("IsMailboxOwnerTheSender");
			if (!(base.Session is MailboxSession))
			{
				return false;
			}
			string valueOrDefault = base.GetValueOrDefault<string>(InternalSchema.SentRepresentingEmailAddress, string.Empty);
			bool flag;
			if (valueOrDefault.Length == 0)
			{
				flag = false;
				ExTraceGlobals.MeetingMessageTracer.Information<GlobalObjectId>((long)this.GetHashCode(), "Storage.MeetingMessage.IsMailboxOwnerTheSender: GOID={0}; no sent representing email address; returning false", this.GlobalObjectId);
			}
			else
			{
				MailboxSession mailboxSession = this.MailboxSession;
				string mailboxOwnerLegacyDN = mailboxSession.MailboxOwnerLegacyDN;
				flag = Participant.HasSameEmail(Participant.Parse(mailboxOwnerLegacyDN), Participant.Parse(valueOrDefault), mailboxSession, false);
				ExTraceGlobals.MeetingMessageTracer.Information<GlobalObjectId, bool>((long)this.GetHashCode(), "Storage.MeetingMessage.IsMailboxOwnerTheSender: GOID={0}; comparing representing email address; result={1}", this.GlobalObjectId, flag);
			}
			return flag;
		}

		public virtual GlobalObjectId GlobalObjectId
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		protected virtual AppointmentStateFlags CalculatedAppointmentState()
		{
			this.CheckDisposed("CalculatedAppointmentState");
			AppointmentStateFlags appointmentStateFlags = this.AppointmentState;
			appointmentStateFlags |= AppointmentStateFlags.Meeting;
			return appointmentStateFlags | AppointmentStateFlags.Received;
		}

		internal void AdjustAppointmentState()
		{
			this.CheckDisposed("AdjustAppointmentState");
			this.AppointmentState = this.CalculatedAppointmentState();
		}

		protected AppointmentStateFlags AppointmentState
		{
			get
			{
				this.CheckDisposed("AppointmentState::get");
				return base.GetValueOrDefault<AppointmentStateFlags>(InternalSchema.AppointmentState);
			}
			set
			{
				this.CheckDisposed("AppointmentState::set");
				this[InternalSchema.AppointmentState] = value;
			}
		}

		public bool SkipMessage(bool processExternal, CalendarItemBase existingCalendarItem)
		{
			this.CheckDisposed("SkipMessage");
			if (!this.IsExternalMessage)
			{
				return this.IsRepairUpdateMessage && this.ShouldBeSentFromOrganizer && this.InternalShouldSkipRUM(existingCalendarItem);
			}
			if (this.IsRepairUpdateMessage)
			{
				ExTraceGlobals.MeetingMessageTracer.Information<GlobalObjectId>((long)this.GetHashCode(), "Storage.MeetingMessage.SkipMessage: GOID={0}. Skipping external RUM.", this.GlobalObjectId);
				return true;
			}
			if (this is MeetingResponse)
			{
				return false;
			}
			if (processExternal)
			{
				return false;
			}
			ExTraceGlobals.MeetingMessageTracer.Information<GlobalObjectId>((long)this.GetHashCode(), "Storage.MeetingMessage.SkipMessage: GOID={0}. Skipping external message.", this.GlobalObjectId);
			return true;
		}

		private bool? IsInternalMessageOrganizerRogue(IRecipientSession adSession, CalendarItemBase existingCalendarItem)
		{
			if (existingCalendarItem == null)
			{
				return new bool?(false);
			}
			if (existingCalendarItem.IsOrganizerExternal)
			{
				return this.AreOrganizersDifferent(base.From, existingCalendarItem.Organizer, adSession);
			}
			Participant organizer = existingCalendarItem.Organizer;
			if (Participant.HasSameEmail(organizer, base.From, this.MailboxSession, true))
			{
				return new bool?(false);
			}
			return this.AreOrganizersDifferent(organizer, base.From, adSession);
		}

		private bool? AreOrganizersDifferent(Participant internalOrganizer, Participant otherOrganizer, IRecipientSession adSession)
		{
			ADRecipient adrecipient = null;
			if (!internalOrganizer.TryGetADRecipient(adSession, out adrecipient) || adrecipient == null)
			{
				ExTraceGlobals.MeetingMessageTracer.Information<GlobalObjectId, Participant>((long)this.GetHashCode(), "Storage.MeetingMessage.AreOrganizersDifferent: GOID={0}. Unable to get the ADRecipient for the organizer ({1}).", this.GlobalObjectId, internalOrganizer);
				return null;
			}
			Participant participant = new Participant(adrecipient);
			return new bool?(!Participant.HasSameEmail(participant, otherOrganizer, this.MailboxSession, true));
		}

		protected abstract bool ShouldBeSentFromOrganizer { get; }

		protected void CheckPreConditionForDelegatedMeeting(bool canUpdatePrincipalCalendar)
		{
			if (this.IsDelegated() && !canUpdatePrincipalCalendar)
			{
				throw new InvalidOperationException("Cannot process delegate meeting message if canUpdatePrincipalCalendar has been specified to false.");
			}
		}

		public virtual void SetCalendarProcessingSteps(CalendarProcessingSteps stepComplete)
		{
			throw new NotSupportedException();
		}

		protected override void OnBeforeSave()
		{
			base.OnBeforeSave();
			this.OnBeforeSaveUpdateLastChangeAction();
		}

		private void OnBeforeSaveUpdateLastChangeAction()
		{
			if (base.PropertyBag.IsDirty)
			{
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(51829U);
				this[InternalSchema.OutlookVersion] = StorageGlobals.ExchangeVersion;
				this[InternalSchema.OutlookInternalVersion] = (int)StorageGlobals.BuildVersion;
			}
		}

		private ADRecipient GetFromRecipient()
		{
			if (this.adFromRecipient == null)
			{
				Participant from = base.From;
				if (from == null)
				{
					ExTraceGlobals.MeetingMessageTracer.Information((long)this.GetHashCode(), "The meeting message response doesn't have a From");
					return null;
				}
				ADSessionSettings adsessionSettings = Participant.BatchBuilder.GetADSessionSettings(this.MailboxSession);
				IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.PartiallyConsistent, adsessionSettings, 1529, "GetFromRecipient", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\Calendar\\MeetingMessage.cs");
				if (!from.TryGetADRecipient(tenantOrRootOrgRecipientSession, out this.adFromRecipient) || this.adFromRecipient == null)
				{
					ExTraceGlobals.MeetingMessageTracer.Information<Participant>((long)this.GetHashCode(), "Failed to get the AD recipient for user {0}.", from);
					return null;
				}
			}
			return this.adFromRecipient;
		}

		internal virtual void Initialize()
		{
			this.CheckDisposed("Initialize");
			base.LocationIdentifierHelperInstance.SetLocationIdentifier(62069U, LastChangeAction.Create);
		}

		private static ExchangePrincipal InternalGetExchangePrincipal(Participant principal, MailboxSession calendarMailboxSession)
		{
			ExchangePrincipal result;
			if (string.Compare(principal.RoutingType, "EX", StringComparison.OrdinalIgnoreCase) == 0)
			{
				result = ExchangePrincipal.FromLegacyDN(calendarMailboxSession.GetADSessionSettings(), principal.EmailAddress, RemotingOptions.AllowCrossSite);
			}
			else
			{
				result = ExchangePrincipal.FromProxyAddress(calendarMailboxSession.GetADSessionSettings(), principal.EmailAddress, RemotingOptions.AllowCrossSite);
			}
			return result;
		}

		private bool InternalShouldSkipRUM(CalendarItemBase existingCalendarItem)
		{
			IRecipientSession adrecipientSession = base.Session.GetADRecipientSession(true, ConsistencyMode.PartiallyConsistent);
			if (adrecipientSession == null)
			{
				ExTraceGlobals.MeetingMessageTracer.TraceDebug<GlobalObjectId>((long)this.GetHashCode(), "Storage.MeetingMessage.SkipMessage: GOID={0}. Skipping the message since there's no session provided for the required AD lookup.", this.GlobalObjectId);
				return true;
			}
			bool? flag = this.IsInternalMessageOrganizerRogue(adrecipientSession, existingCalendarItem);
			return flag == null || flag.Value;
		}

		internal static readonly StorePropertyDefinition[] PreservableProperties = PreservableMeetingMessageProperty.PreservablePropertyDefinitions.ToArray<StorePropertyDefinition>();

		internal static readonly StorePropertyDefinition[] MeetingMessageProperties = CalendarItemProperties.NonPreservableMeetingMessageProperties.Concat(MeetingMessage.PreservableProperties).ToArray<StorePropertyDefinition>();

		internal static readonly StorePropertyDefinition[] DisplayTimeZoneProperties = new StorePropertyDefinition[]
		{
			InternalSchema.TimeZoneDefinitionStart,
			InternalSchema.TimeZoneDefinitionEnd,
			InternalSchema.TimeZoneDefinitionRecurring
		};

		internal static readonly StorePropertyDefinition[] WriteOnCreateProperties = new StorePropertyDefinition[]
		{
			InternalSchema.MapiSensitivity,
			InternalSchema.Categories,
			InternalSchema.ReminderMinutesBeforeStartInternal,
			InternalSchema.ReminderIsSetInternal,
			InternalSchema.ReminderNextTime,
			InternalSchema.AcceptLanguage
		};

		internal static readonly StorePropertyDefinition[] WriteOnCreateSeriesProperties = new StorePropertyDefinition[]
		{
			InternalSchema.SeriesReminderIsSet
		};

		private ADRecipient adFromRecipient;
	}
}
