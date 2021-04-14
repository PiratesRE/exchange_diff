using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage.CalendarDiagnostics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MeetingInquiryMessage : MessageItem
	{
		private static void CheckSession(StoreSession session)
		{
		}

		public static MeetingInquiryMessage Create(MailboxSession session, StoreId parentFolderId, RumInfo rumInfo)
		{
			MeetingInquiryMessage meetingInquiryMessage = null;
			bool flag = false;
			try
			{
				meetingInquiryMessage = ItemBuilder.CreateNewItem<MeetingInquiryMessage>(session, parentFolderId, ItemCreateInfo.MeetingInquiryInfo);
				meetingInquiryMessage.LocationIdentifierHelperInstance.SetLocationIdentifier(39191U, LastChangeAction.Create);
				meetingInquiryMessage[StoreObjectSchema.ItemClass] = "IPM.Schedule.Inquiry";
				RumDecorator rumDecorator = RumDecorator.CreateInstance(rumInfo);
				rumDecorator.AdjustRumMessage(meetingInquiryMessage.MailboxSession, meetingInquiryMessage, rumInfo, null, false);
				flag = true;
			}
			finally
			{
				if (!flag && meetingInquiryMessage != null)
				{
					meetingInquiryMessage.Dispose();
				}
			}
			return meetingInquiryMessage;
		}

		public new static MeetingInquiryMessage Bind(StoreSession session, StoreId id)
		{
			return MeetingInquiryMessage.Bind(session, id, null);
		}

		public new static MeetingInquiryMessage Bind(StoreSession session, StoreId id, ICollection<PropertyDefinition> propsToReturn)
		{
			MeetingInquiryMessage meetingInquiryMessage = ItemBuilder.ItemBind<MeetingInquiryMessage>(session, id, MeetingInquiryMessageSchema.Instance, propsToReturn);
			meetingInquiryMessage.LocationIdentifierHelperInstance.SetLocationIdentifier(55575U, LastChangeAction.Bind);
			return meetingInquiryMessage;
		}

		internal MeetingInquiryMessage(ICoreItem coreItem) : base(coreItem, false)
		{
		}

		public GlobalObjectId GlobalObjectId
		{
			get
			{
				this.CheckDisposed("GlobalObjectId::get");
				if (this.cachedGlobalObjectId != null)
				{
					return this.cachedGlobalObjectId;
				}
				byte[] valueOrDefault = base.GetValueOrDefault<byte[]>(MeetingInquiryMessageSchema.GlobalObjectId);
				this.cachedGlobalObjectId = new GlobalObjectId(valueOrDefault);
				return this.cachedGlobalObjectId;
			}
			set
			{
				this.CheckDisposed("GlobalObjectId::set");
				this.cachedGlobalObjectId = value;
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(63767U);
				this[MeetingInquiryMessageSchema.GlobalObjectId] = value.Bytes;
			}
		}

		public bool IsProcessed
		{
			get
			{
				this.CheckDisposed("IsProcessed::get");
				return base.GetValueOrDefault<bool>(MeetingInquiryMessageSchema.IsProcessed);
			}
			set
			{
				this.CheckDisposed("IsProcessed::set");
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(34071U);
				this[MeetingInquiryMessageSchema.IsProcessed] = value;
			}
		}

		public bool CalendarProcessed
		{
			get
			{
				this.CheckDisposed("CalendarProcessed::get");
				return base.GetValueOrDefault<bool>(MeetingInquiryMessageSchema.CalendarProcessed);
			}
			set
			{
				this.CheckDisposed("CalendarProcessed::set");
				base.LocationIdentifierHelperInstance.SetLocationIdentifier(50455U);
				this[MeetingInquiryMessageSchema.CalendarProcessed] = value;
			}
		}

		public override Schema Schema
		{
			get
			{
				this.CheckDisposed("Schema::get");
				return MeetingInquiryMessageSchema.Instance;
			}
		}

		private MailboxSession MailboxSession
		{
			get
			{
				return (MailboxSession)base.Session;
			}
		}

		public static bool WouldTryToRepairIfMissing(CalendarVersionStoreGateway cvsGateway, GlobalObjectId globalObjectId, MailboxSession session, byte[] calendarFolderId, out MeetingInquiryAction predictedAction)
		{
			return MeetingInquiryMessage.WouldTryToRepairIfMissing(cvsGateway, globalObjectId, session, null, false, calendarFolderId, out predictedAction);
		}

		private static bool WouldTryToRepairIfMissing(CalendarVersionStoreGateway cvsGateway, GlobalObjectId globalObjectId, MailboxSession session, MeetingInquiryMessage message, bool performRepair, byte[] calendarFolderId, out MeetingInquiryAction predictedAction)
		{
			VersionedId versionedId;
			bool result;
			if (MeetingInquiryMessage.TryGetCancelledVersionIdFromCvs(cvsGateway, globalObjectId, session, calendarFolderId, out versionedId))
			{
				if (performRepair)
				{
					message.LocationIdentifierHelperInstance.SetLocationIdentifier(47383U, LastChangeAction.SendMeetingCancellations);
					message.SendCancellationBasedOnFoundIntent(versionedId);
				}
				predictedAction = MeetingInquiryAction.SendCancellation;
				result = true;
			}
			else
			{
				if (versionedId != null)
				{
					using (CalendarItemBase calendarItemBase = CalendarItemBase.Bind(session, versionedId))
					{
						predictedAction = MeetingInquiryMessage.GetReviveAction(cvsGateway, session, calendarItemBase);
						if (predictedAction == MeetingInquiryAction.ReviveMeeting)
						{
							if (performRepair)
							{
								predictedAction = message.TryReviveMeeting(calendarItemBase);
							}
							result = true;
						}
						else
						{
							result = false;
						}
						return result;
					}
				}
				ExTraceGlobals.MeetingMessageTracer.TraceDebug<string, GlobalObjectId>((long)session.GetHashCode(), "Storage.MeetingInquiryMessage::Process. Couldn't find deletion on mailbox {0}. Skipping GOID {1}", session.DisplayName, globalObjectId);
				predictedAction = MeetingInquiryAction.DeletedVersionNotFound;
				result = false;
			}
			return result;
		}

		private static bool TryGetCancelledVersionIdFromCvs(CalendarVersionStoreGateway cvsGateway, GlobalObjectId globalObjectId, MailboxSession session, byte[] calendarFolderId, out VersionedId deletedVersionId)
		{
			ClientIntentFlags clientIntentFlags;
			ClientIntentQuery clientIntentQuery;
			if (globalObjectId.IsCleanGlobalObjectId)
			{
				clientIntentFlags = ClientIntentFlags.MeetingCanceled;
				ICalendarItemStateDefinition deletedFromFolderStateDefinition = CompositeCalendarItemStateDefinition.GetDeletedFromFolderStateDefinition(calendarFolderId);
				clientIntentQuery = new ItemDeletionClientIntentQuery(globalObjectId, deletedFromFolderStateDefinition);
			}
			else
			{
				clientIntentFlags = ClientIntentFlags.MeetingExceptionCanceled;
				ICalendarItemStateDefinition targetState = new DeletedOccurrenceCalendarItemStateDefinition(globalObjectId.Date, false);
				clientIntentQuery = new NonTransitionalClientIntentQuery(globalObjectId, targetState);
			}
			ClientIntentQuery.QueryResult queryResult = clientIntentQuery.Execute(session, cvsGateway);
			deletedVersionId = queryResult.SourceVersionId;
			return ClientIntentQuery.CheckDesiredClientIntent(queryResult.Intent, new ClientIntentFlags[]
			{
				clientIntentFlags
			});
		}

		private static MeetingInquiryAction GetReviveAction(CalendarVersionStoreGateway cvsGateway, MailboxSession session, CalendarItemBase deletedVersion)
		{
			MeetingInquiryAction result;
			if (deletedVersion == null)
			{
				result = MeetingInquiryAction.DeletedVersionNotFound;
			}
			else
			{
				bool flag = false;
				try
				{
					if (session.Capabilities.CanHaveDelegateUsers)
					{
						DelegateUserCollection delegateUserCollection = new DelegateUserCollection(session);
						flag = (delegateUserCollection.Count > 0);
					}
				}
				catch (DelegateUserNoFreeBusyFolderException)
				{
					ExTraceGlobals.MeetingMessageTracer.Information((long)session.GetHashCode(), "Storage.MeetingInquiryMessage.GetReviveAction: NoFreeBusyData Folder, failing to get delegate rule type.");
				}
				catch (ObjectNotFoundException)
				{
					ExTraceGlobals.MeetingMessageTracer.Information((long)session.GetHashCode(), "Storage.MeetingInquiryMessage.GetReviveAction: No delegates found, failing to get delegate rule type.");
				}
				if (!flag)
				{
					result = (MeetingInquiryMessage.PairWithCancellation(cvsGateway, deletedVersion, session) ? MeetingInquiryAction.PairedCancellationFound : MeetingInquiryAction.ReviveMeeting);
				}
				else
				{
					ExTraceGlobals.MeetingMessageTracer.TraceDebug<string, GlobalObjectId>((long)session.GetHashCode(), "Storage.MeetingInquiryMessage::GetReviveAction. Mailbox {0} has delegates so we can't get the cancellation from version store. Skipping GOID {1}", session.DisplayName, deletedVersion.GlobalObjectId);
					result = MeetingInquiryAction.HasDelegates;
				}
			}
			return result;
		}

		private static bool PairWithCancellation(CalendarVersionStoreGateway cvsGateway, CalendarItemBase deletedVersion, MailboxSession session)
		{
			bool pairedWithCancellation = false;
			int deletedItemSequenceNumber = deletedVersion.AppointmentSequenceNumber;
			cvsGateway.QueryByGlobalObjectId(session, deletedVersion.GlobalObjectId, "{B7DF6DD0-6F29-42b4-A1AE-44A0733782AA}", MeetingInquiryMessage.cancellationQueryProperties, delegate(PropertyBag propertyBag)
			{
				int valueOrDefault = propertyBag.GetValueOrDefault<int>(CalendarItemBaseSchema.AppointmentSequenceNumber);
				if (valueOrDefault >= deletedItemSequenceNumber)
				{
					pairedWithCancellation = true;
				}
				return false;
			}, true, MeetingInquiryMessage.cancellationClassArray, null, null);
			return pairedWithCancellation;
		}

		private List<Attendee> GetRumAttendees(CalendarItemBase calendarItem)
		{
			return new List<Attendee>(1)
			{
				calendarItem.AttendeeCollection.Add(base.From, AttendeeType.Required, null, null, false)
			};
		}

		private void SendCancellationBasedOnFoundIntent(VersionedId cancelledVersionId)
		{
			using (CalendarItemBase calendarItemBase = CalendarItemBase.Bind(base.Session, cancelledVersionId))
			{
				IList<Attendee> rumAttendees = this.GetRumAttendees(calendarItemBase);
				CancellationRumInfo info;
				if (this.GlobalObjectId.IsCleanGlobalObjectId)
				{
					info = CancellationRumInfo.CreateMasterInstance(rumAttendees);
				}
				else
				{
					info = CancellationRumInfo.CreateOccurrenceInstance(this.GlobalObjectId.Date, rumAttendees);
				}
				Dictionary<GlobalObjectId, List<Attendee>> dictionary = new Dictionary<GlobalObjectId, List<Attendee>>(1);
				RumAgent.Instance.SendRums(info, false, calendarItemBase, ref dictionary);
			}
		}

		private MeetingInquiryAction TryReviveMeeting(CalendarItemBase deletedVersion)
		{
			bool flag = false;
			if (this.GlobalObjectId.IsCleanGlobalObjectId)
			{
				using (CalendarItemBase calendarItemBase = (CalendarItemBase)Microsoft.Exchange.Data.Storage.Item.CloneItem(base.Session, this.MailboxSession.GetDefaultFolderId(DefaultFolderType.Calendar), deletedVersion, false, true, null))
				{
					calendarItemBase.LocationIdentifierHelperInstance.SetLocationIdentifier(43287U, LastChangeAction.Resurrect);
					calendarItemBase.Save(SaveMode.NoConflictResolution);
					flag = true;
					goto IL_207;
				}
			}
			using (CalendarFolder calendarFolder = CalendarFolder.Bind(this.MailboxSession, DefaultFolderType.Calendar))
			{
				StoreId calendarItemId = calendarFolder.GetCalendarItemId(this.GlobalObjectId.CleanGlobalObjectIdBytes);
				if (calendarItemId != null)
				{
					using (CalendarItem calendarItem = CalendarItem.Bind(this.MailboxSession, calendarItemId))
					{
						if (calendarItem.CalendarItemType == CalendarItemType.RecurringMaster)
						{
							try
							{
								calendarItem.RecoverDeletedOccurrenceByDateId(this.GlobalObjectId.Date);
								calendarItem.LocationIdentifierHelperInstance.SetLocationIdentifier(59671U, LastChangeAction.Resurrect);
								ConflictResolutionResult conflictResolutionResult = calendarItem.Save(SaveMode.ResolveConflicts);
								if (conflictResolutionResult.SaveStatus == SaveResult.IrresolvableConflict)
								{
									ExTraceGlobals.MeetingMessageTracer.TraceError<string, GlobalObjectId>((long)this.GetHashCode(), "Storage.MeetingInquiryMessage::ReviveDeletedVersionIfNoCancellation for Mailbox {0} and GOID {1} hit a save conflict.", this.MailboxSession.DisplayName, this.GlobalObjectId);
									throw new SaveConflictException(ServerStrings.ExSaveFailedBecauseOfConflicts(base.InternalObjectId), conflictResolutionResult);
								}
								flag = true;
							}
							catch (OccurrenceNotFoundException)
							{
								ExTraceGlobals.MeetingMessageTracer.TraceError<string, GlobalObjectId, ExDateTime>((long)this.GetHashCode(), "Storage.MeetingInquiryMessage::ReviveDeletedVersionIfNoCancellation for Mailbox {0} and GOID {1}. There's no occurrence on {2}", this.MailboxSession.DisplayName, this.GlobalObjectId, this.GlobalObjectId.Date);
							}
							catch (ObjectValidationException ex)
							{
								if (ex.Errors.Count != 1 || (!(ex.Errors[0].Constraint is OrganizerPropertiesConstraint) && !(ex.Errors[0].Constraint is CalendarOriginatorIdConstraint)))
								{
									throw;
								}
								ExTraceGlobals.MeetingMessageTracer.TraceError<string, GlobalObjectId, string>((long)this.GetHashCode(), "Storage.MeetingInquiryMessage::ReviveDeletedVersionIfNoCancellation for Mailbox {0} and GOID {1}. Object validation failed : {2}", this.MailboxSession.DisplayName, this.GlobalObjectId, ex.Message);
							}
						}
						goto IL_1FB;
					}
				}
				ExTraceGlobals.MeetingMessageTracer.TraceError<string, byte[]>((long)this.GetHashCode(), "Storage.MeetingInquiryMessage::ReviveDeletedVersionIfNoCancellation for Mailbox {0}. Couldn't find calendar item with CGOID {1}", this.MailboxSession.DisplayName, this.GlobalObjectId.CleanGlobalObjectIdBytes);
				IL_1FB:;
			}
			IL_207:
			if (!flag)
			{
				return MeetingInquiryAction.FailedToRevive;
			}
			return MeetingInquiryAction.ReviveMeeting;
		}

		private MeetingInquiryMessage.OccurrenceInquiryError CheckOccurrence(CalendarItem masterItem)
		{
			MeetingInquiryMessage.OccurrenceInquiryError result = MeetingInquiryMessage.OccurrenceInquiryError.None;
			if (masterItem != null)
			{
				ExDateTime date = this.GlobalObjectId.Date;
				if (masterItem.Recurrence == null || !masterItem.Recurrence.IsValidOccurrenceId(date))
				{
					ExTraceGlobals.MeetingMessageTracer.TraceDebug<GlobalObjectId, string>((long)this.GetHashCode(), "Storage.MeetingInquiryMessage::CheckOccurrence. The inquired occurrence (GOID: {0}) is invalid (user: {1})", this.GlobalObjectId, this.MailboxSession.DisplayName);
					result = MeetingInquiryMessage.OccurrenceInquiryError.InvalidOccurrence;
				}
				else if (!masterItem.Recurrence.IsOccurrenceDeleted(date))
				{
					ExTraceGlobals.MeetingMessageTracer.TraceDebug<GlobalObjectId, string>((long)this.GetHashCode(), "Storage.MeetingInquiryMessage::CheckOccurrence. The inquired occurrence (GOID: {0}) already exists in the calendar folder of the user ({1})", this.GlobalObjectId, this.MailboxSession.DisplayName);
					result = MeetingInquiryMessage.OccurrenceInquiryError.ExistingOccurrence;
				}
			}
			return result;
		}

		private MeetingInquiryMessage.ExistingCalendarItemError CheckExistingCalendarItem(CalendarFolder calFolder, out VersionedId existingCalendarItemId)
		{
			MeetingInquiryMessage.ExistingCalendarItemError result = MeetingInquiryMessage.ExistingCalendarItemError.None;
			existingCalendarItemId = calFolder.GetCalendarItemId(this.GlobalObjectId.CleanGlobalObjectIdBytes);
			if (existingCalendarItemId != null)
			{
				if (this.GlobalObjectId.IsCleanGlobalObjectId)
				{
					ExTraceGlobals.MeetingMessageTracer.TraceDebug<GlobalObjectId, string>((long)this.GetHashCode(), "Storage.MeetingInquiryMessage::CheckExistingCalendarItem. The inquired meeting (GOID: {0}) already exists in the calendar folder of the user ({1})", this.GlobalObjectId, this.MailboxSession.DisplayName);
					result = MeetingInquiryMessage.ExistingCalendarItemError.MeetingAlreadyExists;
				}
				else
				{
					result = MeetingInquiryMessage.ExistingCalendarItemError.OccurrenceInquiry;
				}
			}
			return result;
		}

		private MeetingInquiryAction TryRepairMissingItem(CalendarVersionStoreGateway cvsGateway, byte[] calendarFolderId)
		{
			MeetingInquiryAction result;
			MeetingInquiryMessage.WouldTryToRepairIfMissing(cvsGateway, this.GlobalObjectId, this.MailboxSession, this, true, calendarFolderId, out result);
			return result;
		}

		public MeetingInquiryAction Process(CalendarVersionStoreGateway cvsGateway)
		{
			this.CheckDisposed("Process");
			VersionedId id;
			MeetingInquiryMessage.ExistingCalendarItemError existingCalendarItemError;
			byte[] providerLevelItemId;
			using (CalendarFolder calendarFolder = CalendarFolder.Bind(this.MailboxSession, DefaultFolderType.Calendar))
			{
				existingCalendarItemError = this.CheckExistingCalendarItem(calendarFolder, out id);
				providerLevelItemId = calendarFolder.Id.ObjectId.ProviderLevelItemId;
			}
			switch (existingCalendarItemError)
			{
			case MeetingInquiryMessage.ExistingCalendarItemError.None:
				return this.TryRepairMissingItem(cvsGateway, providerLevelItemId);
			case MeetingInquiryMessage.ExistingCalendarItemError.MeetingAlreadyExists:
				return MeetingInquiryAction.MeetingAlreadyExists;
			case MeetingInquiryMessage.ExistingCalendarItemError.OccurrenceInquiry:
				using (CalendarItem calendarItem = CalendarItem.Bind(base.Session, id))
				{
					MeetingInquiryMessage.OccurrenceInquiryError occurrenceInquiryError = this.CheckOccurrence(calendarItem);
					switch (occurrenceInquiryError)
					{
					case MeetingInquiryMessage.OccurrenceInquiryError.None:
						return this.TryRepairMissingItem(cvsGateway, providerLevelItemId);
					case MeetingInquiryMessage.OccurrenceInquiryError.InvalidOccurrence:
					{
						UpdateRumInfo rumInfo = UpdateRumInfo.CreateMasterInstance(this.GetRumAttendees(calendarItem), CalendarInconsistencyFlag.RecurrenceBlob);
						calendarItem.SendUpdateRums(rumInfo, false);
						return MeetingInquiryAction.SendUpdateForMaster;
					}
					case MeetingInquiryMessage.OccurrenceInquiryError.ExistingOccurrence:
						return MeetingInquiryAction.ExistingOccurrence;
					default:
						throw new InvalidOperationException(string.Format("An invalid error code is returned: {0}.", occurrenceInquiryError));
					}
				}
				break;
			}
			throw new InvalidOperationException(string.Format("An invalid error code is returned: {0}.", existingCalendarItemError));
		}

		private const string CvsQuerySchemaKey = "{B7DF6DD0-6F29-42b4-A1AE-44A0733782AA}";

		private static readonly StorePropertyDefinition[] cancellationQueryProperties = new StorePropertyDefinition[]
		{
			StoreObjectSchema.ItemClass,
			CalendarItemBaseSchema.AppointmentSequenceNumber
		};

		private static readonly string[] cancellationClassArray = new string[]
		{
			"IPM.Schedule.Meeting.Canceled"
		};

		private GlobalObjectId cachedGlobalObjectId;

		private enum ExistingCalendarItemError
		{
			None,
			MeetingAlreadyExists,
			OccurrenceInquiry
		}

		private enum OccurrenceInquiryError
		{
			None,
			InvalidOccurrence,
			ExistingOccurrence
		}
	}
}
