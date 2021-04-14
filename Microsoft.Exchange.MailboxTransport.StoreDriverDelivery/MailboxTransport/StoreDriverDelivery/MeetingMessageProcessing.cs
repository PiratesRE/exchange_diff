using System;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.StoreDriverDelivery;
using Microsoft.Exchange.Entities.Calendaring.MessageProcessing;
using Microsoft.Exchange.MailboxTransport.StoreDriver.Configuration;
using Microsoft.Exchange.MailboxTransport.StoreDriverCommon;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Categorizer;
using Microsoft.Exchange.Transport.Logging.MessageTracking;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal static class MeetingMessageProcessing
	{
		public static void ProcessMessage(MessageItem messageItem, MailRecipient recipient, StoreObjectId calendarItemIdHint = null, StoreId deliveryFolderId = null)
		{
			MailboxItem mailboxItem = RecipientItem.Create(recipient) as MailboxItem;
			if (!MeetingMessageProcessing.ParametersValidation(mailboxItem))
			{
				return;
			}
			string text = (string)MeetingMessageProcessing.SafeGetProperty(messageItem, ItemSchema.InternetMessageId, "<null>");
			MailboxSession session = messageItem.Session as MailboxSession;
			MeetingMessageProcessing.diag.TraceDebug<string>(0L, "About to process {0}", text);
			MeetingMessage meetingMessage = null;
			try
			{
				Item item = Item.ConvertFrom(messageItem, session);
				meetingMessage = (item as MeetingMessage);
				if (meetingMessage == null)
				{
					MeetingMessageProcessing.diag.TraceDebug<string>(0L, "Bad format: the message has the right messageclass, but is not a meeting message. Stopping processing. InternetMessageId = {0}.", text);
					if (item != null)
					{
						Item.SafeDisposeConvertedItem(messageItem, item);
					}
				}
				else
				{
					MeetingMessageProcessing.ProcessMessageInternal(messageItem, recipient, session, meetingMessage, mailboxItem, text, calendarItemIdHint, deliveryFolderId);
				}
			}
			finally
			{
				if (meetingMessage != null)
				{
					Item.SafeDisposeConvertedItem(messageItem, meetingMessage);
				}
			}
		}

		private static void RemoveMungageDataIfNeeded(MeetingMessage messageItem)
		{
			using (MessageItem messageItem2 = MessageItem.CreateInMemory(StoreObjectSchema.ContentConversionProperties))
			{
				Body.CopyBody(messageItem.Body, messageItem2.Body, messageItem.Session.PreferedCulture, true, false);
				Body.CopyBody(messageItem2.Body, messageItem.Body, messageItem.Session.PreferedCulture, false, false);
			}
		}

		private static void ProcessMessageInternal(MessageItem messageItem, MailRecipient recipient, MailboxSession session, MeetingMessage mtgMessage, MailboxItem mailboxItem, string internetMessageId, StoreObjectId calendarItemIdHint = null, StoreId deliveryFolderId = null)
		{
			CalendarItemBase calendarItemBase = null;
			string legacyExchangeDN = mailboxItem.LegacyExchangeDN;
			Guid mailboxGuid = mailboxItem.MailboxGuid;
			string serverName = mailboxItem.ServerName;
			CalendarSettings calendarSettings = new CalendarSettings(CalendarFlags.CalAssistantDefaults, 15);
			MeetingMessageProcessing.GetCalSettings(session, serverName, calendarSettings);
			if (calendarSettings.Flags == CalendarFlags.None)
			{
				MeetingMessageProcessing.diag.TraceDebug<string, string, string>(0L, "Unable to get calendar settings for user: {0} on server: {1}. InternetMessageId = {2}.", legacyExchangeDN, serverName, internetMessageId);
				return;
			}
			MeetingMessageProcessing.diag.TracePfd<int, string, string>(0L, "PFD IWC {0} Store Driver Start processing meeting message {1} for mailbox {2}", 20119, legacyExchangeDN, internetMessageId);
			int num = 2;
			MeetingMessageProcessingTrackingInfo meetingMessageProcessingTrackingInfo = new MeetingMessageProcessingTrackingInfo(legacyExchangeDN, mailboxGuid);
			bool flag = true;
			for (int i = 0; i < num; i++)
			{
				try
				{
					mtgMessage.SetCalendarProcessingSteps(CalendarProcessingSteps.None);
					MeetingMessageProcessing.RemoveMungageDataIfNeeded(mtgMessage);
					if (mtgMessage.IsDelegated())
					{
						meetingMessageProcessingTrackingInfo.Stage = MeetingMessageProcessStages.DelegateMessageFound;
						break;
					}
					if (!MeetingMessageProcessing.GetCalendarItem(session, mtgMessage, internetMessageId, legacyExchangeDN, meetingMessageProcessingTrackingInfo, out calendarItemBase, calendarItemIdHint))
					{
						flag = false;
						meetingMessageProcessingTrackingInfo.Stage = MeetingMessageProcessStages.ErrorObtainingCalendarItem;
						break;
					}
					if (!MeetingMessageProcessing.ShouldContinueProcessing(legacyExchangeDN, mtgMessage, calendarItemBase, calendarSettings, meetingMessageProcessingTrackingInfo, session, deliveryFolderId, internetMessageId))
					{
						break;
					}
					bool flag2 = true;
					bool flag3;
					if (mailboxItem.Recipient.ExtendedProperties.TryGetValue<bool>("Microsoft.Exchange.Transport.DirectoryData.IsResource", out flag3))
					{
						flag2 = flag3;
					}
					if (flag2 && mtgMessage.SkipMessage(calendarSettings.CalendarAssistantProcessExternal, calendarItemBase))
					{
						MeetingMessageProcessing.diag.TraceDebug<string, bool>(0L, "Stopping processing because the message is an external message. Mailbox = {0}. IsRepairUpdateMessage = {1}.", legacyExchangeDN, mtgMessage.IsRepairUpdateMessage);
						meetingMessageProcessingTrackingInfo.Stage = MeetingMessageProcessStages.ExternalMsgProcessingDisabled;
						break;
					}
					if (calendarItemBase == null)
					{
						MeetingMessageProcessing.diag.TraceDebug<string, string>(0L, "Original CalendarItem is null for message {0}. Mailbox = {1}", internetMessageId, legacyExchangeDN);
					}
					flag = false;
					MeetingMessageProcessing.ProcessMeetingMessage(session, mtgMessage, ref calendarItemBase, internetMessageId, calendarSettings, meetingMessageProcessingTrackingInfo);
					flag = true;
					break;
				}
				catch (OccurrenceCrossingBoundaryException ex)
				{
					MeetingMessageProcessing.diag.TraceDebug<string, string>(0L, "Found an overlapping occurrence while processing message ID={0}, in mailbox={1}. Cleaning things up and retrying", internetMessageId, legacyExchangeDN);
					StringBuilder stringBuilder = new StringBuilder(1024);
					stringBuilder.AppendFormat("Found an overlapping occurrence while processing message ID={0}.", internetMessageId);
					if (ex.OccurrenceInfo == null)
					{
						MeetingMessageProcessing.diag.TraceDebug<string>(0L, "Unexpected: Handling OccurrenceCrossingBoundaryException, the OccurrenceInfo is null.  Mailbox = {0}", legacyExchangeDN);
						stringBuilder.Append("Unexpected: Handling OccurrenceCrossingBoundaryException, the OccurrenceInfo is null");
						break;
					}
					VersionedId versionedId = ex.OccurrenceInfo.VersionedId;
					OperationResult operationResult = mtgMessage.Session.Delete(DeleteItemFlags.MoveToDeletedItems, new StoreId[]
					{
						versionedId
					}).OperationResult;
					MeetingMessageProcessing.diag.TraceDebug<string, OperationResult>(0L, "Deleting the occurrence when handling an OccurrenceCrossingBoundaryException in Mailbox = {0}, returned:{1}", legacyExchangeDN, operationResult);
					stringBuilder.AppendFormat("Deleting the occurrence when handling an OccurrenceCrossingBoundaryException returned:{0}", operationResult);
					MeetingMessageProcessingLog.LogEntry(session, ex, false, stringBuilder.ToString(), new object[0]);
					meetingMessageProcessingTrackingInfo.AddLogMessage(stringBuilder.ToString());
					meetingMessageProcessingTrackingInfo.SaveExceptionInfo(ex);
				}
				catch (CorruptDataException ex2)
				{
					MeetingMessageProcessing.diag.TraceDebug<string, CorruptDataException>(0L, "Got a corrupt message in Mailbox = {0}. Exception: {1}", legacyExchangeDN, ex2);
					MeetingMessageProcessingLog.LogEntry(session, ex2, false, "Got a corrupt message id = {0}", new object[]
					{
						internetMessageId
					});
					meetingMessageProcessingTrackingInfo.SaveExceptionInfo(ex2);
				}
				catch (VirusDetectedException ex3)
				{
					MeetingMessageProcessing.diag.TraceDebug<VersionedId, string, VirusDetectedException>(0L, "A virus was detected while processing this message {0}. Mailbox = {1}. This message will be skipped. Exception: {2}", mtgMessage.Id, legacyExchangeDN, ex3);
					MeetingMessageProcessingLog.LogEntry(session, ex3, false, "A virus was detected while processing message {0}. This message will be skipped.", new object[]
					{
						internetMessageId
					});
					meetingMessageProcessingTrackingInfo.SaveExceptionInfo(ex3);
				}
				catch (VirusMessageDeletedException ex4)
				{
					MeetingMessageProcessing.diag.TraceDebug<VersionedId, string, VirusMessageDeletedException>(0L, "A virus was detected while processing this message {0}. Mailbox = {1}. This message will be skipped. Exception: {2}", mtgMessage.Id, legacyExchangeDN, ex4);
					MeetingMessageProcessingLog.LogEntry(session, ex4, false, "A virus was detected while processing message {0}. This message will be skipped.", new object[]
					{
						internetMessageId
					});
					meetingMessageProcessingTrackingInfo.SaveExceptionInfo(ex4);
				}
				catch (DataValidationException ex5)
				{
					MeetingMessageProcessing.diag.TraceDebug<VersionedId, string, DataValidationException>(0L, "The directory entry for one of the recipients was found to be corrupt while processing this message {0}. Mailbox = {1}. This message will be skipped. Exception: {2}", mtgMessage.Id, legacyExchangeDN, ex5);
					MeetingMessageProcessingLog.LogEntry(session, ex5, false, "The directory entry for one of the recipients was found to be corrupt while processing this message {0}. This message will be skipped.", new object[]
					{
						internetMessageId
					});
					meetingMessageProcessingTrackingInfo.SaveExceptionInfo(ex5);
				}
				catch (RecurrenceException ex6)
				{
					MeetingMessageProcessing.diag.TraceDebug<VersionedId, string, RecurrenceException>(0L, "There was a problem reading the recurrence blob on this message {0}. Mailbox = {1}. This message will be skipped. Exception: {2}", mtgMessage.Id, legacyExchangeDN, ex6);
					MeetingMessageProcessingLog.LogEntry(session, ex6, false, "There was a problem reading the recurrence blob on this message {0}. This message will be skipped.", new object[]
					{
						internetMessageId
					});
					meetingMessageProcessingTrackingInfo.SaveExceptionInfo(ex6);
				}
				catch (SaveConflictException ex7)
				{
					MeetingMessageProcessing.diag.TraceDebug<VersionedId, string, SaveConflictException>(0L, "Hit a save conflict on this message {0}. Mailbox = {1}. This message will be skipped. Exception: {2}", mtgMessage.Id, legacyExchangeDN, ex7);
					MeetingMessageProcessingLog.LogEntry(session, ex7, false, "Hit a save conflict on this message {0}. This message will be skipped.", new object[]
					{
						internetMessageId
					});
					meetingMessageProcessingTrackingInfo.SaveExceptionInfo(ex7);
					i = num;
				}
				finally
				{
					object obj = mtgMessage.TryGetProperty(CalendarItemBaseSchema.MeetingRequestType);
					object obj2 = mtgMessage.TryGetProperty(MeetingMessageInstanceSchema.CalendarProcessingSteps);
					CalendarProcessingSteps calendarProcessingSteps = CalendarProcessingSteps.None;
					if (obj2 is int)
					{
						calendarProcessingSteps = (CalendarProcessingSteps)obj2;
					}
					if (calendarProcessingSteps == CalendarProcessingSteps.None && obj is int && (MeetingMessageType)obj == MeetingMessageType.PrincipalWantsCopy)
					{
						mtgMessage[CalendarItemBaseSchema.MeetingRequestType] = MeetingMessageType.FullUpdate;
					}
					if (calendarItemBase != null)
					{
						calendarItemBase.Dispose();
						calendarItemBase = null;
					}
				}
				meetingMessageProcessingTrackingInfo.MeetingMessageProcessingAttempts++;
			}
			MeetingMessageProcessing.diag.TraceDebug<string, string>(0L, "Finished processing message. Mailbox = {0}, InternetMessgeID = {1}", legacyExchangeDN, internetMessageId);
			MeetingMessageProcessing.diag.TracePfd<int, string, string>(0L, "PFD IWC {0} Store Driver Finished processing meeting message for Mailbox = {1}, InternetMessgeID = {2}", 28311, legacyExchangeDN, internetMessageId);
			meetingMessageProcessingTrackingInfo.ProcessingSucceeded = flag;
			MSExchangeStoreDriver.TotalMeetingMessages.Increment();
			if (!flag)
			{
				MSExchangeStoreDriver.TotalMeetingFailures.Increment();
				if (mtgMessage.IsRepairUpdateMessage)
				{
					MeetingMessageProcessing.diag.TraceDebug<string, string, GlobalObjectId>(0L, "A RUM was not processed: will abort the delivery for Mailbox = {0}, InternetMessgeID = {1}, GOID = {2}", legacyExchangeDN, internetMessageId, mtgMessage.GlobalObjectId);
					MeetingMessageProcessingLog.LogEntry(session, "A RUM with InternetMessageID = {0} and GOID = {1} was not processed and the delivery will be aborted.", new object[]
					{
						internetMessageId,
						mtgMessage.GlobalObjectId
					});
					meetingMessageProcessingTrackingInfo.Stage = MeetingMessageProcessStages.RUMAbortDelivery;
					MessageTrackingLog.TrackMeetingMessage(internetMessageId, session.ClientMachineName, session.OrganizationId, meetingMessageProcessingTrackingInfo.GetExtraEventData());
					MeetingMessageProcessing.AbortDelivery(recipient);
				}
			}
			MessageTrackingLog.TrackMeetingMessage(internetMessageId, session.ClientMachineName, session.OrganizationId, meetingMessageProcessingTrackingInfo.GetExtraEventData());
		}

		internal static void TraceExceptionAndDispose(LocalizedException e, MailboxSession itemStore, MeetingMessage mtgMessage, string internetMessageId, int retries, MeetingMessageProcessingTrackingInfo processingTracker)
		{
			object obj = string.Empty;
			if (e is SaveConflictException)
			{
				obj = new ACRTraceString(((SaveConflictException)e).ConflictResolutionResult);
			}
			MeetingMessageProcessing.diag.TraceError(0L, "Exception thrown when processing item: {0}, Attempt = {1}, Mailbox = {2}, exception = {3}, extraData={4}", new object[]
			{
				internetMessageId,
				4 - retries,
				itemStore.MailboxOwnerLegacyDN,
				e,
				obj
			});
			MeetingMessageProcessingLog.LogEntry(itemStore, e, false, "Exception thrown when processing item: {0}, Attempt = {1}, extraData={2}", new object[]
			{
				internetMessageId,
				4 - retries,
				obj
			});
			processingTracker.SaveExceptionInfo(e);
			processingTracker.AddLogMessage(obj.ToString());
		}

		private static void AbortDelivery(MailRecipient recipient)
		{
			recipient.DsnRequested = DsnRequestedFlags.Never;
			throw new SmtpResponseException(MeetingMessageProcessing.UnprocessedRumResponse, "MeetingMessageProcessingAgent");
		}

		private static bool GetCalendarItem(MailboxSession session, MeetingMessage mtgMessage, string internetMessageId, string legacyDN, MeetingMessageProcessingTrackingInfo processingTracker, out CalendarItemBase originalCalItem, StoreObjectId calendarItemIdHint = null)
		{
			originalCalItem = null;
			try
			{
				if (session.GetDefaultFolderId(DefaultFolderType.Calendar) == null)
				{
					MeetingMessageProcessing.diag.TraceDebug<string>(0L, "Calendar folder does not exist. Skipping processing. InternetMessageId = {0}.", internetMessageId);
					MeetingMessageProcessingLog.LogEntry(session, "Calendar folder does not exist. Skipping processing. InternetMessageId = {0}.", new object[]
					{
						internetMessageId
					});
					return false;
				}
				originalCalItem = mtgMessage.TryGetCorrelatedItemFromHintId(session, calendarItemIdHint);
				if (originalCalItem == null)
				{
					originalCalItem = mtgMessage.GetCorrelatedItem();
				}
			}
			catch (ObjectNotFoundException ex)
			{
				MeetingMessageProcessingLog.LogEntry(session, ex, false, "Failed to get the default folder id.", new object[0]);
				processingTracker.SaveExceptionInfo(ex);
				return false;
			}
			catch (CorruptDataException ex2)
			{
				MeetingMessageProcessing.diag.TraceDebug<VersionedId, string>(0L, "There was an error opening the original CalendarItem associated with this message {0}. Mailbox = {1}. This message will be skipped.", mtgMessage.Id, legacyDN);
				MeetingMessageProcessingLog.LogEntry(session, ex2, false, "There was an error opening the original CalendarItem associated with this message {0}", new object[]
				{
					mtgMessage.Id
				});
				processingTracker.SaveExceptionInfo(ex2);
				return false;
			}
			catch (CorrelationFailedException ex3)
			{
				MeetingMessageProcessing.diag.TraceDebug<VersionedId, string>(0L, "The original CalendarItem associated with this message {0} is a master, but this message is a single occurrence. Mailbox = {1}. This message will be skipped.", mtgMessage.Id, legacyDN);
				MeetingMessageProcessingLog.LogEntry(session, ex3, false, "The original CalendarItem associated with this message {0} is a master, but this message is a single occurrence. This message will be skipped.", new object[]
				{
					mtgMessage.Id
				});
				processingTracker.SaveExceptionInfo(ex3);
				return false;
			}
			catch (VirusDetectedException ex4)
			{
				MeetingMessageProcessing.diag.TraceDebug<VersionedId, string>(0L, "A virus was detected in the CalendarItem associated with this message {0}. Mailbox = {1}. This message will be skipped.", mtgMessage.Id, legacyDN);
				MeetingMessageProcessingLog.LogEntry(session, ex4, false, "A virus was detected in the CalendarItem associated with this message {0}. This message will be skipped.", new object[]
				{
					mtgMessage.Id
				});
				processingTracker.SaveExceptionInfo(ex4);
				return false;
			}
			catch (VirusMessageDeletedException ex5)
			{
				MeetingMessageProcessing.diag.TraceDebug<VersionedId, string>(0L, "This message was deleted because it had a virus. Message {0}. Mailbox = {1}. This message will be skipped.", mtgMessage.Id, legacyDN);
				MeetingMessageProcessingLog.LogEntry(session, ex5, false, "This message was deleted because it had a virus. Message {0}. This message will be skipped.", new object[]
				{
					mtgMessage.Id
				});
				processingTracker.SaveExceptionInfo(ex5);
				return false;
			}
			catch (RecurrenceException ex6)
			{
				MeetingMessageProcessing.diag.TraceDebug<VersionedId, string, RecurrenceException>(0L, "This message is being skipped because there is a problem reading the recurrence blob. Message {0}. Mailbox = {1}. This message will be skipped. Exception {2}", mtgMessage.Id, legacyDN, ex6);
				MeetingMessageProcessingLog.LogEntry(session, ex6, false, "This message is being skipped because there is a problem reading the recurrence blob. Message {0}. This message will be skipped.", new object[]
				{
					mtgMessage.Id
				});
				processingTracker.SaveExceptionInfo(ex6);
				return false;
			}
			return true;
		}

		private static bool ParametersValidation(MailboxItem mailboxItem)
		{
			if (mailboxItem == null)
			{
				MeetingMessageProcessing.diag.TraceDebug(0L, "Could not get the guid or server name properties from the recipient. Skipping processing.");
				return false;
			}
			if (mailboxItem.RecipientType == RecipientType.PublicFolder)
			{
				MeetingMessageProcessing.diag.TraceDebug(0L, "Skipping calendar processing for items for public folders.");
				return false;
			}
			if (mailboxItem.RecipientType == RecipientType.SystemAttendantMailbox)
			{
				MeetingMessageProcessing.diag.TraceDebug(0L, "Skipping the System Attendant mailbox.");
				return false;
			}
			return true;
		}

		private static void GetCalSettings(MailboxSession session, string serverName, CalendarSettings settings)
		{
			int num = 0;
			settings.Flags = CalendarFlags.None;
			settings.DefaultReminderTime = 15;
			CalendarConfiguration calendarConfiguration;
			using (CalendarConfigurationDataProvider calendarConfigurationDataProvider = new CalendarConfigurationDataProvider(session))
			{
				calendarConfiguration = (CalendarConfiguration)calendarConfigurationDataProvider.Read<CalendarConfiguration>(null);
			}
			if (calendarConfiguration == null)
			{
				MeetingMessageProcessing.diag.TraceDebug(0L, "Could not open the calendar config, even when trying to recreate it, using the defaults.");
				MeetingMessageProcessingLog.LogEntry(session, "Could not open the calendar config, even when trying to recreate it, using the defaults.", new object[0]);
				calendarConfiguration = new CalendarConfiguration();
			}
			num |= ((calendarConfiguration.AutomateProcessing == CalendarProcessingFlags.AutoAccept) ? 1 : 0);
			num |= ((calendarConfiguration.AutomateProcessing == CalendarProcessingFlags.AutoUpdate) ? 2 : 0);
			num |= (calendarConfiguration.RemoveOldMeetingMessages ? 4 : 0);
			num |= (calendarConfiguration.AddNewRequestsTentatively ? 8 : 0);
			num |= (calendarConfiguration.ProcessExternalMeetingMessages ? 16 : 0);
			num |= (calendarConfiguration.SkipProcessing ? 32 : 0);
			settings.Flags = (CalendarFlags)num;
			settings.DefaultReminderTime = calendarConfiguration.DefaultReminderTime;
		}

		internal static bool ShouldContinueProcessing(string mailboxName, MeetingMessage mtgMessage, CalendarItemBase originalCalItem, CalendarSettings settings, MeetingMessageProcessingTrackingInfo processingTracker, MailboxSession session, StoreId deliveryFolderId, string internetMessageId)
		{
			if (settings.AutomaticBooking)
			{
				MeetingMessageProcessing.diag.TraceDebug<string>(0L, "This is a resource mailbox. Mailbox = {0}", mailboxName);
				processingTracker.Stage = MeetingMessageProcessStages.ResourceMailboxFound;
				processingTracker.AddLogMessage("This is a resource mailbox.");
				return false;
			}
			if (settings.SkipProcessing)
			{
				MeetingMessageProcessing.diag.TraceDebug<string>(0L, "SkipProcessing is set. Mailbox = {0}", mailboxName);
				processingTracker.Stage = MeetingMessageProcessStages.CalendarAssistantActiveFalse;
				processingTracker.AddLogMessage("CalAssistant is configured to not run.");
				return false;
			}
			if (originalCalItem == null && MeetingMessageProcessing.IsJunkMeetingRequest(mtgMessage, deliveryFolderId, session, internetMessageId))
			{
				MeetingMessageProcessing.diag.TraceDebug<string>(0L, "Message {0} is new junk meeting request message", internetMessageId);
				MeetingMessageProcessingLog.LogEntry(session, "Message {0} is new junk meeting request", new object[]
				{
					internetMessageId
				});
				processingTracker.Stage = MeetingMessageProcessStages.JunkNewMeetingRequestFound;
				processingTracker.AddLogMessage(string.Format("Message {0} is new junk meeting request.", internetMessageId));
				return false;
			}
			return true;
		}

		internal static bool IsJunkMeetingRequest(MeetingMessage mtgMessage, StoreId deliveryFolderId, MailboxSession mailBoxSession, string internetMessageId)
		{
			bool flag = false;
			try
			{
				ExTraceGlobals.FaultInjectionTracer.TraceTest<bool>(2575707453U, ref flag);
			}
			catch (Exception)
			{
				MeetingMessageProcessingLog.LogEntry(mailBoxSession, "Exception thrown during getting the value of the isJunk fault injection.", new object[0]);
			}
			if (flag)
			{
				MeetingMessageProcessingLog.LogEntry(mailBoxSession, "Using isJunk fault injection, return IsJunkMeetingRequest = true for message : {0}", new object[]
				{
					internetMessageId
				});
				return true;
			}
			MeetingMessageProcessingLog.LogEntry(mailBoxSession, "Not using isJunk fault injection for message : {0}", new object[]
			{
				internetMessageId
			});
			bool flag2 = mtgMessage is MeetingRequest;
			if (flag2 && deliveryFolderId != null)
			{
				StoreObjectId storeObjectId = StoreId.GetStoreObjectId(deliveryFolderId);
				StoreObjectId defaultFolderId = mailBoxSession.GetDefaultFolderId(DefaultFolderType.JunkEmail);
				return storeObjectId.Equals(defaultFolderId);
			}
			return false;
		}

		internal static bool IsSentToSelf(MeetingRequest mtgMessage, MailboxSession session)
		{
			if (mtgMessage == null || mtgMessage.IsExternalMessage || session == null)
			{
				return false;
			}
			Participant sender = mtgMessage.Sender;
			if (sender == null)
			{
				return false;
			}
			IExchangePrincipal mailboxOwner = session.MailboxOwner;
			bool flag = false;
			Participant participant = Participant.TryConvertTo(sender, "SMTP", session);
			if (participant != null)
			{
				flag = string.Equals(mailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString(), participant.EmailAddress, StringComparison.OrdinalIgnoreCase);
			}
			else if (string.Equals(sender.RoutingType, "EX", StringComparison.OrdinalIgnoreCase))
			{
				flag = string.Equals(sender.EmailAddress, mailboxOwner.LegacyDn, StringComparison.OrdinalIgnoreCase);
			}
			if (flag)
			{
				string value = MeetingMessageProcessing.SafeGetProperty(mtgMessage, MessageItemSchema.ReceivedRepresentingEmailAddress, string.Empty) as string;
				flag = string.IsNullOrEmpty(value);
			}
			return flag;
		}

		private static void ProcessMeetingMessage(MailboxSession itemStore, MeetingMessage mtgMessage, ref CalendarItemBase originalCalItem, string internetMessageId, CalendarSettings settings, MeetingMessageProcessingTrackingInfo processingTracker)
		{
			int num = settings.DefaultReminderTime;
			if (num < 0 || num > 2629800)
			{
				num = MeetingMessageProcessing.DefaultReminderMinutesBeforeStart;
			}
			bool flag = false;
			int num2 = 3;
			int i = num2;
			VersionedId versionedId = null;
			bool processed = false;
			while (i > 0)
			{
				try
				{
					MeetingMessageProcessing.diag.TraceDebug(0L, "About to DoProcessingLogic");
					if (versionedId == null && originalCalItem != null)
					{
						versionedId = originalCalItem.Id;
					}
					flag = MeetingMessageProcessing.DoProcessingLogic(itemStore, mtgMessage, ref originalCalItem, internetMessageId, processed, num, processingTracker, ref i);
					MeetingMessageProcessing.diag.TraceDebug<bool>(0L, "completedProcessing = {0}", flag);
					if (flag)
					{
						break;
					}
				}
				catch (ObjectExistedException e)
				{
					MeetingMessageProcessing.TraceExceptionAndDispose(e, itemStore, mtgMessage, internetMessageId, i, processingTracker);
					i--;
				}
				catch (SaveConflictException e2)
				{
					MeetingMessageProcessing.TraceExceptionAndDispose(e2, itemStore, mtgMessage, internetMessageId, i, processingTracker);
					if (originalCalItem != null)
					{
						originalCalItem.Dispose();
						originalCalItem = null;
					}
					if (!MeetingMessageProcessing.GetCalendarItem(itemStore, mtgMessage, internetMessageId, itemStore.MailboxOwnerLegacyDN, processingTracker, out originalCalItem, null))
					{
						i = 0;
						break;
					}
					if (originalCalItem != null)
					{
						originalCalItem.OpenAsReadWrite();
					}
					i--;
				}
				catch (QuotaExceededException e3)
				{
					MeetingMessageProcessing.TraceExceptionAndDispose(e3, itemStore, mtgMessage, internetMessageId, i, processingTracker);
					i = 0;
				}
				catch (ObjectNotFoundException e4)
				{
					MeetingMessageProcessing.TraceExceptionAndDispose(e4, itemStore, mtgMessage, internetMessageId, i, processingTracker);
					i = 0;
				}
				processingTracker.CalendarUpdateXsoCodeAttempts++;
			}
			if (!flag && i == 0)
			{
				MeetingMessageProcessing.diag.TraceDebug<string, string>(0L, "Attempting a third time to process message, but without the catch blocks: {0}, Mailbox = {1}", internetMessageId, itemStore.MailboxOwnerLegacyDN);
				StoreDriverDeliveryDiagnostics.LogEvent(MailboxTransportEventLogConstants.Tuple_ProcessingMeetingMessageFailure, null, new object[]
				{
					mtgMessage.Subject,
					(itemStore != null && itemStore.MailboxOwner != null) ? itemStore.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString() : "NULL",
					"NULL"
				});
				flag = MeetingMessageProcessing.DoProcessingLogic(itemStore, mtgMessage, ref originalCalItem, internetMessageId, processed, num, processingTracker, ref i);
				processingTracker.CalendarUpdateXsoCodeAttempts++;
			}
		}

		private static bool DoProcessingLogic(MailboxSession itemStore, MeetingMessage mtgMessage, ref CalendarItemBase originalCalItem, string internetMessageId, bool processed, int defaultReminderInMinutes, MeetingMessageProcessingTrackingInfo processingTracker, ref int retries)
		{
			bool flag = false;
			mtgMessage.SetCalendarProcessingSteps(CalendarProcessingSteps.None);
			try
			{
				ExTraceGlobals.FaultInjectionTracer.TraceTest<bool>(52880U, ref flag);
			}
			catch (Exception)
			{
				flag = false;
				MeetingMessageProcessing.diag.TraceDebug<string, GlobalObjectId, string>(0L, "Failed during fault injection. Ignoring message wtih InternetMessageId: {0}, Goid = {1}, Mailbox = {2}", internetMessageId, mtgMessage.GlobalObjectId, itemStore.MailboxOwnerLegacyDN);
				MeetingMessageProcessingLog.LogEntry(itemStore, "Failed during fault injection. Ignoring message with InternetMessageId = {0}, Goid = {1}", new object[]
				{
					internetMessageId,
					mtgMessage.GlobalObjectId
				});
			}
			if (flag)
			{
				MeetingMessageProcessing.diag.TraceDebug<string, string>(0L, "Using fault injection to simulate a failure to process message: {0}, Mailbox = {1}", internetMessageId, itemStore.MailboxOwnerLegacyDN);
				MeetingMessageProcessingLog.LogEntry(itemStore, "Using fault injection to simulate a failure to process message with InternetMessageId = {0}.", new object[]
				{
					internetMessageId
				});
				throw new SaveConflictException(new LocalizedString("Testing E14:237591"));
			}
			bool flag2 = true;
			if (StoreDriverConfig.Instance.MeetingHijackPreventionEnabled)
			{
				string text = null;
				if (!mtgMessage.VerifyCalendarOriginatorId(itemStore, originalCalItem, internetMessageId, out text))
				{
					MeetingMessageProcessing.diag.TraceError<string, string>(0L, "Failed to validate calendar originator on MeetingRequest: {0}, Mailbox = {1}", internetMessageId, itemStore.MailboxOwnerLegacyDN);
					bool isMeeting = originalCalItem.IsMeeting;
					string text2 = mtgMessage.GlobalObjectId.ToString();
					string idForEventLog = ((IOrganizationIdForEventLog)itemStore.OrganizationId).IdForEventLog;
					MeetingMessageProcessingLog.LogEntry(itemStore, "Calendar originator Ids don't match: MR - {0} Calendar - {1}. Calendar item is {2}, Goid - {3}, InternetMessageId = {4}", new object[]
					{
						mtgMessage.CalendarOriginatorId,
						originalCalItem.CalendarOriginatorId,
						isMeeting ? "a meeting" : "an appointment",
						text2,
						internetMessageId
					});
					processingTracker.OrgId = idForEventLog;
					processingTracker.Goid = text2;
					processingTracker.Stage = (isMeeting ? MeetingMessageProcessStages.HijackedMeetingFound : MeetingMessageProcessStages.HijackedAppointmentFound);
					CalendarProcessingSteps calendarProcessingSteps = CalendarProcessingSteps.PropsCheck | CalendarProcessingSteps.LookedForOutOfDate | CalendarProcessingSteps.ChangedMtgType | CalendarProcessingSteps.UpdatedCalItem;
					mtgMessage.MarkAsHijacked();
					mtgMessage.SetCalendarProcessingSteps(calendarProcessingSteps);
					mtgMessage.CalendarProcessed = true;
					mtgMessage.IsProcessed = true;
					MeetingMessageProcessingLog.LogEntry(itemStore, "Marked the meeting request as Hijacked. InternetMessageId = {0}, Mailbox = {1}, OrganizationId = {2}", new object[]
					{
						internetMessageId,
						itemStore.MailboxOwnerLegacyDN,
						idForEventLog
					});
					return true;
				}
				if (!string.IsNullOrEmpty(text))
				{
					MeetingMessageProcessingLog.LogEntry(itemStore, "{0} InternetMessageId = {2}, Mailbox = {3}", new object[]
					{
						text,
						internetMessageId,
						itemStore.MailboxOwnerLegacyDN
					});
					processingTracker.Stage = MeetingMessageProcessStages.ParticipantMatchFailure;
				}
			}
			MeetingMessageProcessing.diag.TraceDebug(0L, "About to call OrganizerOrDelegateCheck");
			if (!MeetingMessageProcessing.OrganizerOrDelegateCheck(mtgMessage, originalCalItem, out flag2))
			{
				MeetingMessageProcessing.diag.TraceDebug(0L, "Not Organizer or delegate");
				if (mtgMessage is MeetingRequest)
				{
					MeetingMessageProcessing.diag.TraceDebug<string, string>(0L, "Processing MeetingRequest: {0}, Mailbox = {1}", internetMessageId, itemStore.MailboxOwnerLegacyDN);
					flag2 = MeetingMessageProcessing.ProcessMeetingRequest(itemStore, (MeetingRequest)mtgMessage, ref originalCalItem, internetMessageId, defaultReminderInMinutes);
					processingTracker.Stage = MeetingMessageProcessStages.ProcessMeetingRequest;
					int num = (int)MeetingMessageProcessing.SafeGetProperty(mtgMessage, CalendarItemBaseSchema.AppointmentSequenceNumber, -1);
					if (processed && num == 0)
					{
						((MeetingRequest)mtgMessage).MeetingRequestType = MeetingMessageType.NewMeetingRequest;
					}
				}
				else if (mtgMessage is MeetingResponse)
				{
					MeetingMessageProcessing.diag.TraceDebug<string, string>(0L, "Processing MeetingResponse: {0}, Mailbox = {1}", internetMessageId, itemStore.MailboxOwnerLegacyDN);
					flag2 = MeetingMessageProcessing.ProcessMeetingResponse(itemStore, (MeetingResponse)mtgMessage, ref originalCalItem, internetMessageId);
					processingTracker.Stage = MeetingMessageProcessStages.ProcessMeetingResponse;
				}
				else if (mtgMessage is MeetingCancellation)
				{
					MeetingMessageProcessing.diag.TraceDebug<string, string>(0L, "Processing MeetingCancellation: {0}, Mailbox = {1}", internetMessageId, itemStore.MailboxOwnerLegacyDN);
					flag2 = MeetingMessageProcessing.ProcessMeetingCancellation(itemStore, (MeetingCancellation)mtgMessage, ref originalCalItem, internetMessageId);
					processingTracker.Stage = MeetingMessageProcessStages.ProcessMeetingCancellation;
				}
				else if (mtgMessage is MeetingForwardNotification)
				{
					MeetingMessageProcessing.diag.TraceDebug<string, string>(0L, "Processing MeetingForwardNotification: {0}, Mailbox = {1}", internetMessageId, itemStore.MailboxOwnerLegacyDN);
					flag2 = MeetingMessageProcessing.ProcessMeetingForwardNotification(itemStore, (MeetingForwardNotification)mtgMessage, ref originalCalItem, internetMessageId);
					processingTracker.Stage = MeetingMessageProcessStages.ProcessMFN;
				}
			}
			else
			{
				object obj = mtgMessage.TryGetProperty(MeetingMessageInstanceSchema.OriginalMeetingType);
				if (obj is int)
				{
					mtgMessage[CalendarItemBaseSchema.MeetingRequestType] = (MeetingMessageType)obj;
				}
				processingTracker.Stage = MeetingMessageProcessStages.MessageReceipientIsOrganizer;
			}
			if (originalCalItem == null)
			{
				MeetingMessageProcessing.diag.TraceDebug<string, string>(0L, "Null cal item resulted from processing meeting: {0}, Mailbox = {1}", internetMessageId, itemStore.MailboxOwnerLegacyDN);
			}
			if (flag2)
			{
				mtgMessage.CalendarProcessed = true;
			}
			return true;
		}

		private static bool OrganizerOrDelegateCheck(MeetingMessage mtgMessage, CalendarItemBase originalCalItem, out bool markAsProcessed)
		{
			bool result = false;
			bool flag = true;
			string text = (string)MeetingMessageProcessing.SafeGetProperty(mtgMessage, MessageItemSchema.ReceivedRepresentingEmailAddress, string.Empty);
			string text2 = (string)MeetingMessageProcessing.SafeGetProperty(mtgMessage, CalendarItemBaseSchema.OrganizerEmailAddress, string.Empty);
			string text3 = string.Empty;
			MailboxSession mailboxSession = mtgMessage.Session as MailboxSession;
			if (originalCalItem != null)
			{
				text3 = (string)MeetingMessageProcessing.SafeGetProperty(originalCalItem, CalendarItemBaseSchema.OrganizerEmailAddress, string.Empty);
			}
			if (string.IsNullOrEmpty(text))
			{
				text = mailboxSession.MailboxOwnerLegacyDN;
			}
			MeetingMessageProcessing.diag.TraceDebug(0L, "OrganizerOrDelegateCheck(): ReceivedRepresenting={0} SentRepresenting={1} CalItemSentRepresenting={2} Mailbox LegDN={3}", new object[]
			{
				text,
				text2,
				text3,
				mailboxSession.MailboxOwner.LegacyDn
			});
			if (string.Compare(text3, text2, true) != 0)
			{
				MeetingMessageProcessing.diag.TraceDebug<string, string>(0L, "OrganizerOrDelegateCheck(): MReq SentRepresenting and CalItem SentRepresenting do NOT match. {0} != {1}", text2, text3);
			}
			if (!(mtgMessage is MeetingForwardNotification))
			{
				if (string.Compare(text, text2, true) == 0)
				{
					result = true;
				}
				if (string.Compare(text, mailboxSession.MailboxOwner.LegacyDn, true) != 0)
				{
					result = true;
				}
			}
			if (originalCalItem != null)
			{
				if (originalCalItem.IsOrganizer())
				{
					if (mtgMessage is MeetingRequest || mtgMessage is MeetingCancellation)
					{
						result = true;
					}
				}
				else if (mtgMessage is MeetingForwardNotification || mtgMessage is MeetingResponse)
				{
					result = true;
				}
			}
			else if (mtgMessage is MeetingRequest)
			{
				if (string.Compare(mailboxSession.MailboxOwner.LegacyDn, text2, true) == 0)
				{
					result = true;
				}
			}
			else
			{
				result = true;
				if (mtgMessage is MeetingResponse)
				{
					flag = false;
				}
			}
			markAsProcessed = flag;
			return result;
		}

		private static bool ProcessMeetingRequest(MailboxSession itemStore, MeetingRequest mtgMessage, ref CalendarItemBase originalCalItem, string internetMessageId, int defaultReminderInMinutes)
		{
			bool result = false;
			try
			{
				if (mtgMessage.TryUpdateCalendarItem(ref originalCalItem, false))
				{
					MeetingMessageType meetingRequestType = mtgMessage.MeetingRequestType;
					if (originalCalItem != null)
					{
						if (originalCalItem.Id == null && MeetingMessageType.NewMeetingRequest == meetingRequestType)
						{
							int num = (int)MeetingMessageProcessing.SafeGetProperty(mtgMessage, ItemSchema.ReminderMinutesBeforeStart, defaultReminderInMinutes);
							if (num == 1525252321)
							{
								num = defaultReminderInMinutes;
								originalCalItem[ItemSchema.ReminderMinutesBeforeStart] = num;
							}
							if (num < 0 || num > 2629800)
							{
								originalCalItem[ItemSchema.ReminderMinutesBeforeStart] = defaultReminderInMinutes;
							}
							if (StoreDriverConfig.Instance.AlwaysSetReminderOnAppointment && !originalCalItem.Reminder.IsSet)
							{
								originalCalItem.Reminder.MinutesBeforeStart = defaultReminderInMinutes;
								originalCalItem.Reminder.IsSet = true;
							}
						}
						if (mtgMessage.IsSeriesMessage)
						{
							MeetingMessageProcessor.ProcessXsoMeetingMessage(itemStore, mtgMessage, originalCalItem, null);
						}
						if (StoreDriverConfig.Instance.IsAutoAcceptForGroupAndSelfForwardedEventEnabled)
						{
							originalCalItem = MeetingMessageProcessing.PerformAutoAcceptForGroupAndSelfForwardedEvents(itemStore, mtgMessage, originalCalItem);
						}
						originalCalItem.Validate();
						ConflictResolutionResult conflictResolutionResult = originalCalItem.Save(SaveMode.ResolveConflicts);
						if (conflictResolutionResult.SaveStatus == SaveResult.IrresolvableConflict)
						{
							originalCalItem.Load();
							throw new SaveConflictException(ServerStrings.ExSaveFailedBecauseOfConflicts(originalCalItem.Id), conflictResolutionResult);
						}
					}
				}
				result = true;
				MeetingMessageProcessing.diag.TracePfd<int, string, string>(0L, "PFD IWC {0} Store Driver Completed Processing MeetingRequest for : {1}, Mailbox = {2}", 24215, internetMessageId, itemStore.MailboxOwnerLegacyDN);
			}
			catch (ObjectNotFoundException)
			{
			}
			return result;
		}

		private static CalendarItemBase PerformAutoAcceptForGroupAndSelfForwardedEvents(MailboxSession itemStore, MeetingRequest mtgMessage, CalendarItemBase originalCalItem)
		{
			try
			{
				if (itemStore.IsGroupMailbox())
				{
					MeetingMessageProcessing.diag.TraceDebug<IExchangePrincipal>(0L, "Processing meeting request for group mailbox {0}", itemStore.MailboxOwner);
					mtgMessage.ProcessMeetingRequestForGroupMailbox(ref originalCalItem);
					originalCalItem.Load();
				}
				else if (MeetingMessageProcessing.IsSentToSelf(mtgMessage, itemStore))
				{
					MeetingMessageProcessing.diag.TraceDebug<IExchangePrincipal>(0L, "Processing sent to self meeting request. Mailbox owner: {0}", itemStore.MailboxOwner);
					MeetingMessageProcessing.ProcessSelfForwardedEvents(originalCalItem);
				}
			}
			catch (StorageTransientException arg)
			{
				MeetingMessageProcessing.diag.TraceError<IExchangePrincipal, StorageTransientException>(0L, "Exception occured while performing the group mailbox processing for mailbox owner:{0}, Exception: {1}", itemStore.MailboxOwner, arg);
			}
			catch (StoragePermanentException arg2)
			{
				MeetingMessageProcessing.diag.TraceError<IExchangePrincipal, StoragePermanentException>(0L, "Exception occured while performing the group mailbox processing for mailbox owner:{0}, Exception: {1}", itemStore.MailboxOwner, arg2);
			}
			return originalCalItem;
		}

		private static void ProcessSelfForwardedEvents(CalendarItemBase originalCalItem)
		{
			if (originalCalItem != null && !originalCalItem.IsCancelled)
			{
				bool responseRequested = originalCalItem.ResponseRequested;
				using (MeetingResponse meetingResponse = originalCalItem.RespondToMeetingRequest(ResponseType.Accept, true, originalCalItem.ResponseRequested, null, null))
				{
					if (responseRequested)
					{
						meetingResponse.Send();
					}
				}
				originalCalItem.Load();
			}
		}

		private static bool ProcessMeetingResponse(MailboxSession itemStore, MeetingResponse mtgMessage, ref CalendarItemBase originalCalItem, string internetMessageId)
		{
			bool result = false;
			try
			{
				if (!mtgMessage.IsRepairUpdateMessage && originalCalItem != null && !originalCalItem.ResponseRequested)
				{
					MeetingMessageProcessing.diag.TraceDebug<string, string>(0L, "The organizer has not requested a response - not updating the calendar item for MeetingResponse: {0}, Mailbox: {1}", internetMessageId, itemStore.MailboxOwnerLegacyDN);
					CalendarProcessingSteps calendarProcessingSteps = CalendarProcessingSteps.PropsCheck | CalendarProcessingSteps.LookedForOutOfDate | CalendarProcessingSteps.UpdatedCalItem;
					mtgMessage.SetCalendarProcessingSteps(calendarProcessingSteps);
				}
				else if (mtgMessage.TryUpdateCalendarItem(ref originalCalItem, false))
				{
					try
					{
						itemStore.COWSettings.TemporaryDisableHold = true;
						ConflictResolutionResult conflictResolutionResult = originalCalItem.Save(SaveMode.ResolveConflicts);
						originalCalItem.Load();
						if (conflictResolutionResult.SaveStatus == SaveResult.IrresolvableConflict)
						{
							throw new SaveConflictException(ServerStrings.ExSaveFailedBecauseOfConflicts(originalCalItem.Id), conflictResolutionResult);
						}
					}
					finally
					{
						itemStore.COWSettings.TemporaryDisableHold = false;
					}
				}
				result = true;
				MeetingMessageProcessing.diag.TracePfd<int, string, string>(0L, "PFD IWC {0} Store Driver completed Processing MeetingResponse for : {1}, Mailbox = {2}", 32407, internetMessageId, itemStore.MailboxOwnerLegacyDN);
			}
			catch (ObjectNotFoundException)
			{
			}
			return result;
		}

		private static bool ProcessMeetingCancellation(MailboxSession itemStore, MeetingCancellation mtgMessage, ref CalendarItemBase originalCalItem, string internetMessageId)
		{
			bool result = false;
			try
			{
				mtgMessage.TryUpdateCalendarItem(ref originalCalItem, false);
				ConflictResolutionResult conflictResolutionResult = originalCalItem.Save(SaveMode.ResolveConflicts);
				originalCalItem.Load();
				if (conflictResolutionResult.SaveStatus == SaveResult.IrresolvableConflict)
				{
					throw new SaveConflictException(ServerStrings.ExSaveFailedBecauseOfConflicts(originalCalItem.Id), conflictResolutionResult);
				}
				result = true;
				MeetingMessageProcessing.diag.TracePfd<int, string, string>(0L, "PFD IWC {0} Store Driver completed Processing MeetingCancellation for : {1}, Mailbox = {2}", 18071, internetMessageId, itemStore.MailboxOwnerLegacyDN);
			}
			catch (ObjectNotFoundException)
			{
			}
			return result;
		}

		private static bool ProcessMeetingForwardNotification(MailboxSession itemStore, MeetingForwardNotification mfnMessage, ref CalendarItemBase originalCalItem, string internetMessageId)
		{
			bool result = false;
			if (mfnMessage == null)
			{
				return result;
			}
			try
			{
				if (!mfnMessage.TryUpdateCalendarItem(ref originalCalItem, false))
				{
					return result;
				}
				ConflictResolutionResult conflictResolutionResult = originalCalItem.Save(SaveMode.ResolveConflicts);
				originalCalItem.Load();
				if (conflictResolutionResult.SaveStatus == SaveResult.IrresolvableConflict)
				{
					throw new SaveConflictException(ServerStrings.ExSaveFailedBecauseOfConflicts(originalCalItem.Id), conflictResolutionResult);
				}
				result = true;
				MeetingMessageProcessing.diag.TracePfd<int, string, string>(0L, "PFD IWC {0} Store Driver completed Processing MeetingForwardNotification for : {1}, Mailbox = {2}", 18071, internetMessageId, itemStore.MailboxOwnerLegacyDN);
			}
			catch (ObjectNotFoundException)
			{
			}
			return result;
		}

		private static object SafeGetProperty(StoreObject message, PropertyDefinition propertyDefinition, object defaultValue)
		{
			if (message == null)
			{
				return defaultValue;
			}
			object obj = message.TryGetProperty(propertyDefinition);
			if (obj == null || obj is PropertyError)
			{
				return defaultValue;
			}
			return obj;
		}

		public const int MarkerForDefaultReminder = 1525252321;

		public static DateTime MaxOutlookDateUtc = DateTime.SpecifyKind(new DateTime(4501, 1, 1), DateTimeKind.Utc);

		public static int DefaultReminderMinutesBeforeStart = 15;

		private static readonly Trace diag = ExTraceGlobals.CalendarProcessingTracer;

		private static readonly SmtpResponse UnprocessedRumResponse = new SmtpResponse("250", "2.1.6", new string[]
		{
			"MeetingMessageProcessingAgent; Suppressing delivery of RUM message"
		});
	}
}
