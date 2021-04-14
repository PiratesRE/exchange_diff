using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCommon.MessageContent;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class EmailManager : SendMessageManager
	{
		public bool MessageListIsNull
		{
			get
			{
				return this.messageItemList == null;
			}
		}

		internal NameOrNumberOfCaller SpecifiedCallerDetails { get; private set; }

		internal override bool LargeGrammarsNeeded
		{
			get
			{
				return true;
			}
		}

		internal EmailManager(ActivityManager manager, EmailManager.ConfigClass config) : base(manager, config)
		{
			this.hiddenThreads = new Dictionary<EmailManager.Conversation, object>();
		}

		internal bool IsRecurringMeetingRequest
		{
			get
			{
				return this.isRecurringMeetingRequest;
			}
			set
			{
				this.isRecurringMeetingRequest = value;
			}
		}

		internal bool IsSenderRoutable
		{
			get
			{
				return this.isSenderRoutable;
			}
			set
			{
				this.isSenderRoutable = value;
			}
		}

		protected ContactInfo SenderInfo
		{
			get
			{
				return this.senderInfo;
			}
			set
			{
				this.senderInfo = value;
			}
		}

		private StoreObjectId InboxId
		{
			get
			{
				byte[] entryId = Convert.FromBase64String(this.user.ConfigFolder.TelephoneAccessFolderEmail);
				return StoreObjectId.FromProviderSpecificId(entryId);
			}
		}

		internal static bool CanReadMessageClassWithTui(string itemClass)
		{
			bool flag = true;
			flag &= (!XsoUtil.IsPureVoice(itemClass) && !XsoUtil.IsProtectedVoicemail(itemClass));
			if (!flag)
			{
				return flag;
			}
			return flag & (itemClass.StartsWith("IPM.Note", true, CultureInfo.InvariantCulture) || itemClass.StartsWith("IPM.Schedule.Meeting", true, CultureInfo.InvariantCulture));
		}

		internal override void Start(BaseUMCallSession vo, string refInfo)
		{
			vo.IncrementCounter(SubscriberAccessCounters.EmailMessageQueueAccessed);
			this.user = vo.CurrentCallContext.CallerInfo;
			this.folderId = this.InboxId;
			this.messageItemList = new MessageItemList(this.user, this.folderId, MessageItemListSortType.Email, EmailManager.viewProperties);
			this.SpecifiedCallerDetails = new NameOrNumberOfCaller(NameOrNumberOfCaller.TypeOfVoiceCall.MissedCall);
			base.Start(vo, refInfo);
		}

		internal override TransitionBase ExecuteAction(string action, BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "EmailManager::ExecuteAction action={0}.", new object[]
			{
				action
			});
			string input = null;
			if (string.Equals(action, "nextUnreadMessage", StringComparison.OrdinalIgnoreCase))
			{
				input = this.NextUnreadMessage();
			}
			else if (string.Equals(action, "previousMessage", StringComparison.OrdinalIgnoreCase))
			{
				input = this.PreviousMessage();
			}
			else if (string.Equals(action, "acceptMeeting", StringComparison.OrdinalIgnoreCase))
			{
				base.SendMsg = new EmailManager.MeetingResponse(vo, this.messageItemList.CurrentStoreObjectId, ResponseType.Accept, this.user, this);
				base.SendMsg.DoSubmit(Importance.Normal);
				base.SendMsg = null;
			}
			else if (string.Equals(action, "acceptMeetingTentative", StringComparison.OrdinalIgnoreCase))
			{
				base.SendMsg = new EmailManager.MeetingResponse(vo, this.messageItemList.CurrentStoreObjectId, ResponseType.Tentative, this.user, this);
				base.SendMsg.DoSubmit(Importance.Normal);
				base.SendMsg = null;
			}
			else if (string.Equals(action, "declineMeeting", StringComparison.OrdinalIgnoreCase))
			{
				base.SendMsg = new EmailManager.MeetingResponse(vo, this.messageItemList.CurrentStoreObjectId, ResponseType.Decline, this.user, this);
				base.WriteReplyIntroType(IntroType.Decline);
			}
			else if (string.Equals(action, "reply", StringComparison.OrdinalIgnoreCase))
			{
				base.SendMsg = new EmailManager.EmailReply(vo, this.messageItemList.CurrentStoreObjectId, this.user, this);
				base.WriteReplyIntroType(IntroType.Reply);
			}
			else if (string.Equals(action, "replyAll", StringComparison.OrdinalIgnoreCase))
			{
				base.SendMsg = new EmailManager.EmailReplyAll(vo, this.messageItemList.CurrentStoreObjectId, this.user, this);
				base.WriteReplyIntroType(IntroType.ReplyAll);
			}
			else if (string.Equals(action, "forward", StringComparison.OrdinalIgnoreCase))
			{
				base.SendMsg = new EmailManager.EmailForward(vo, this.messageItemList.CurrentStoreObjectId, this.user, this);
				base.WriteReplyIntroType(IntroType.Forward);
			}
			else if (string.Equals(action, "deleteMessage", StringComparison.OrdinalIgnoreCase))
			{
				this.DeleteCurrentMessage(true);
			}
			else if (string.Equals(action, "undeleteMessage", StringComparison.OrdinalIgnoreCase))
			{
				this.Undelete();
			}
			else if (string.Equals(action, "flagMessage", StringComparison.OrdinalIgnoreCase))
			{
				this.FlagMessage();
			}
			else if (string.Equals(action, "saveMessage", StringComparison.OrdinalIgnoreCase))
			{
				this.MarkRead();
			}
			else if (string.Equals(action, "markUnread", StringComparison.OrdinalIgnoreCase))
			{
				this.MarkUnread();
			}
			else if (string.Equals(action, "hideThread", StringComparison.OrdinalIgnoreCase))
			{
				this.HideThread();
			}
			else if (string.Equals(action, "deleteThread", StringComparison.OrdinalIgnoreCase))
			{
				this.DeleteThread();
			}
			else if (string.Equals(action, "findByName", StringComparison.OrdinalIgnoreCase))
			{
				this.FindByName();
			}
			else
			{
				if (string.Equals(action, "commitPendingDeletions", StringComparison.OrdinalIgnoreCase))
				{
					if (this.pendingDeletion == null)
					{
						goto IL_301;
					}
					using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.user.CreateSessionLock())
					{
						this.pendingDeletion.Commit(mailboxSessionLock.Session);
						this.pendingDeletion = null;
						goto IL_301;
					}
				}
				if (string.Equals(action, "selectLanguage", StringComparison.OrdinalIgnoreCase))
				{
					input = base.SelectLanguage();
				}
				else
				{
					if (!string.Equals(action, "nextLanguage", StringComparison.OrdinalIgnoreCase))
					{
						return base.ExecuteAction(action, vo);
					}
					input = base.NextLanguage(vo);
				}
			}
			IL_301:
			return base.CurrentActivity.GetTransition(input);
		}

		internal StoreObjectId GetCurrentItemId()
		{
			return this.messageItemList.CurrentStoreObjectId;
		}

		internal string NextMessage(BaseUMCallSession vo)
		{
			return this.NextMessage(false);
		}

		internal void DeleteCurrentMessage(bool allowUndelete)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "DeleteCurrentMessage with allowUndelete={0}.", new object[]
			{
				allowUndelete
			});
			using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.user.CreateSessionLock())
			{
				StoreObjectId currentStoreObjectId = this.messageItemList.CurrentStoreObjectId;
				string itemClass = this.messageItemList.SafeGetProperty<string>(StoreObjectSchema.ItemClass, string.Empty);
				if (ObjectClass.IsMeetingCancellation(itemClass))
				{
					XsoUtil.RemoveFromCalendar(this.messageItemList.CurrentStoreObjectId, mailboxSessionLock.Session);
				}
				if (this.pendingDeletion != null)
				{
					this.pendingDeletion.Commit(mailboxSessionLock.Session);
					this.pendingDeletion = null;
				}
				if (!allowUndelete)
				{
					mailboxSessionLock.Session.Delete(DeleteItemFlags.MoveToDeletedItems, new StoreId[]
					{
						this.messageItemList.CurrentStoreObjectId
					});
				}
				else
				{
					this.pendingDeletion = new EmailManager.SingleItemPendingDeletion(base.CallSession, this.messageItemList);
					if (this.numDeletions < 2U && (this.numDeletions += 1U) >= 2U)
					{
						base.WriteVariable("playedUndelete", true);
					}
				}
				base.WriteVariable("canUndelete", this.pendingDeletion != null);
				this.messageItemList.Ignore(currentStoreObjectId);
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "EmailManager::Dispose.", new object[0]);
					if (this.findByNameResults != null)
					{
						this.findByNameResults.Dispose();
					}
					if (this.pendingDeletion != null && this.user != null)
					{
						using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.user.CreateSessionLock())
						{
							if (mailboxSessionLock.Session != null)
							{
								this.pendingDeletion.Commit(mailboxSessionLock.Session);
								this.pendingDeletion = null;
							}
						}
					}
				}
			}
			finally
			{
				base.InternalDispose(disposing);
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<EmailManager>(this);
		}

		private static void GetToAndCCFields(MessageItem emailMsg, out string addressToLine, out string addressCcLine)
		{
			XsoUtil.BuildParticipantStrings(emailMsg.Recipients, out addressToLine, out addressCcLine);
			addressToLine = ((!string.IsNullOrEmpty(addressToLine)) ? addressToLine : null);
			addressCcLine = ((!string.IsNullOrEmpty(addressCcLine)) ? addressCcLine : null);
		}

		private static List<StoreObjectId> GetConversations(MailboxSession session, StoreObjectId folderId, EmailManager.Conversation targetConversation)
		{
			List<StoreObjectId> result;
			using (Folder folder = Folder.Bind(session, folderId))
			{
				using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.None, null, null, new PropertyDefinition[]
				{
					ItemSchema.Id,
					ItemSchema.ConversationIndex
				}))
				{
					List<StoreObjectId> list = new List<StoreObjectId>();
					object[][] rows = queryResult.GetRows(MessageItemList.PageSize);
					while (rows != null && 0 < rows.Length)
					{
						foreach (object[] array2 in rows)
						{
							StoreObjectId objectId = ((VersionedId)array2[0]).ObjectId;
							EmailManager.Conversation other = EmailManager.Conversation.FromConversationIndex(array2[1] as byte[]);
							if (targetConversation.Equals(other))
							{
								list.Add(objectId);
							}
						}
						rows = queryResult.GetRows(MessageItemList.PageSize);
					}
					result = list;
				}
			}
			return result;
		}

		private static ExDateTime CalculateNextOccurenceEndWindow(ExDateTime startTime, Recurrence r)
		{
			ExDateTime result;
			try
			{
				if (r.Pattern is DailyRecurrencePattern)
				{
					DailyRecurrencePattern dailyRecurrencePattern = r.Pattern as DailyRecurrencePattern;
					result = startTime.AddDays((double)(2 * dailyRecurrencePattern.RecurrenceInterval));
				}
				else if (r.Pattern is WeeklyRecurrencePattern)
				{
					WeeklyRecurrencePattern weeklyRecurrencePattern = r.Pattern as WeeklyRecurrencePattern;
					result = startTime.AddDays((double)(14 * weeklyRecurrencePattern.RecurrenceInterval));
				}
				else if (r.Pattern is MonthlyRecurrencePattern || r.Pattern is MonthlyThRecurrencePattern)
				{
					MonthlyRecurrencePattern monthlyRecurrencePattern = r.Pattern as MonthlyRecurrencePattern;
					MonthlyThRecurrencePattern monthlyThRecurrencePattern = r.Pattern as MonthlyThRecurrencePattern;
					int num = (monthlyRecurrencePattern != null) ? monthlyRecurrencePattern.RecurrenceInterval : monthlyThRecurrencePattern.RecurrenceInterval;
					result = startTime.AddMinutes((double)(2 * num));
				}
				else if (r.Pattern is YearlyRecurrencePattern || r.Pattern is YearlyThRecurrencePattern)
				{
					result = startTime.AddYears(2);
				}
				else
				{
					result = ExDateTime.MaxValue;
				}
			}
			catch (ArgumentException)
			{
				result = ExDateTime.MaxValue;
			}
			return result;
		}

		private static CalendarItemBase GetCalendarItem(MeetingRequest msgReq)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, null, "EmailManager::GetCorrelatedItem.", new object[0]);
			CalendarItemBase calendarItemBase = msgReq.IsOrganizer() ? msgReq.GetCorrelatedItem() : msgReq.UpdateCalendarItem(false);
			if (calendarItemBase == null)
			{
				throw new ObjectNotFoundException(LocalizedString.Empty);
			}
			return calendarItemBase;
		}

		private static bool ValidateMessage(MessageItem emailMsg)
		{
			if (null == emailMsg.From || string.IsNullOrEmpty(emailMsg.From.EmailAddress))
			{
				if (XsoUtil.IsMissedCall(emailMsg.ClassName))
				{
					return true;
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, null, "Skipping message, FROM field is missing or there is no email address", new object[0]);
				return false;
			}
			else
			{
				MeetingRequest meetingRequest = emailMsg as MeetingRequest;
				if (meetingRequest != null && meetingRequest.IsOutOfDate())
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, null, "Skipping out of date meeting request.", new object[0]);
					return false;
				}
				return true;
			}
		}

		private string NextUnreadMessage()
		{
			return this.NextMessage(true);
		}

		private string SeekToMessage(StoreObjectId targetObjectId)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "EmailManager::SeekToMessage({0}).", new object[]
			{
				targetObjectId
			});
			int currentOffset = this.messageItemList.CurrentOffset;
			this.messageItemList.Seek(targetObjectId);
			string result;
			using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.user.CreateSessionLock())
			{
				MessageItem messageItem = null;
				try
				{
					if (!this.FetchMessage(mailboxSessionLock.Session, out messageItem))
					{
						throw new InvalidOperationException();
					}
					result = this.InitializeMessage(true, currentOffset, messageItem);
				}
				finally
				{
					if (messageItem != null)
					{
						messageItem.Dispose();
					}
				}
			}
			return result;
		}

		private string NextMessage(bool unreadOnly)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "EmailManager::NextMessage", new object[0]);
			int currentOffset = this.messageItemList.CurrentOffset;
			string result;
			using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.user.CreateSessionLock())
			{
				MessageItem messageItem = null;
				try
				{
					while (this.messageItemList.Next(unreadOnly) && !this.FetchMessage(mailboxSessionLock.Session, unreadOnly, out messageItem))
					{
					}
					result = this.InitializeMessage(true, currentOffset, messageItem);
				}
				finally
				{
					if (messageItem != null)
					{
						messageItem.Dispose();
					}
				}
			}
			return result;
		}

		private string PreviousMessage()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "EmailManager::PreviousMessage", new object[0]);
			int currentOffset = this.messageItemList.CurrentOffset;
			string result;
			using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.user.CreateSessionLock())
			{
				MessageItem messageItem = null;
				try
				{
					while (this.messageItemList.Previous() && !this.FetchMessage(mailboxSessionLock.Session, out messageItem))
					{
					}
					result = this.InitializeMessage(false, currentOffset, messageItem);
				}
				finally
				{
					if (messageItem != null)
					{
						messageItem.Dispose();
					}
				}
			}
			return result;
		}

		private string InitializeMessage(bool movingForward, int currentOffset, MessageItem emailMsg)
		{
			string result = null;
			CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "EmailManager::InitializeMessage.", new object[0]);
			if (emailMsg == null)
			{
				this.messageItemList.Seek(currentOffset);
				CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "No more messages in the inbox.", new object[0]);
				result = (movingForward ? "endOfMessages" : "noPreviousMessages");
			}
			else
			{
				ContactInfo contactInfo = null;
				if (emailMsg.From != null && emailMsg.From.EmailAddress != null)
				{
					contactInfo = ContactInfo.FindByParticipant(this.user, emailMsg.From);
				}
				this.SenderInfo = ((contactInfo != null) ? contactInfo : new DefaultContactInfo());
				this.WriteMessageVariables(emailMsg);
				base.MessagePlayerContext.Reset(emailMsg.Id.ObjectId);
				base.CallSession.IncrementCounter(SubscriberAccessCounters.EmailMessagesHeard);
			}
			return result;
		}

		private bool FetchMessage(MailboxSession session, out MessageItem emailMsg)
		{
			return this.FetchMessage(session, false, out emailMsg);
		}

		private bool FetchMessage(MailboxSession session, bool unreadOnly, out MessageItem emailMsg)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "EmailManager::FetchMessage.", new object[0]);
			emailMsg = null;
			string text = this.messageItemList.SafeGetProperty<string>(StoreObjectSchema.ItemClass, string.Empty);
			byte[] array = this.messageItemList.SafeGetProperty<byte[]>(ItemSchema.ConversationIndex, null);
			bool flag = this.messageItemList.SafeGetProperty<bool>(MessageItemSchema.IsRead, false);
			if (text == null || !EmailManager.CanReadMessageClassWithTui(text))
			{
				return false;
			}
			if (array != null && this.hiddenThreads.ContainsKey(EmailManager.Conversation.FromConversationIndex(array)))
			{
				return false;
			}
			if (unreadOnly && flag)
			{
				return false;
			}
			StoreObjectId currentStoreObjectId = this.messageItemList.CurrentStoreObjectId;
			try
			{
				emailMsg = MessageItem.Bind(session, currentStoreObjectId, new PropertyDefinition[]
				{
					ItemSchema.ReceivedTime,
					ItemSchema.Subject,
					MessageItemSchema.IsRead,
					ItemSchema.Importance,
					CalendarItemInstanceSchema.StartTime,
					MeetingMessageInstanceSchema.IsProcessed,
					ItemSchema.NormalizedSubject,
					ItemSchema.FlagStatus,
					ItemSchema.FlagRequest,
					MessageItemSchema.VoiceMessageSenderName,
					MessageItemSchema.SenderTelephoneNumber
				});
				if (!EmailManager.ValidateMessage(emailMsg))
				{
					emailMsg.Dispose();
					emailMsg = null;
					return false;
				}
				emailMsg.OpenAsReadWrite();
			}
			catch (ObjectNotFoundException)
			{
				CallIdTracer.TraceWarning(ExTraceGlobals.EmailTracer, this, "EmailManager caught ObjectNotFoundException.", new object[0]);
				emailMsg = null;
				this.messageItemList.Ignore(currentStoreObjectId);
				return false;
			}
			if (this.firstMessageId == null && emailMsg != null)
			{
				this.firstMessageId = emailMsg.Id.ObjectId;
			}
			return emailMsg != null;
		}

		private void WriteMessageVariables(MessageItem emailMsg)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "EmailManager::WriteMessageVariables.", new object[0]);
			this.WriteMeetingRequestVariables(emailMsg);
			this.WriteMeetingCancellationVariables(emailMsg);
			this.WriteCommonMessageVariables(emailMsg);
		}

		private void WriteCommonMessageVariables(MessageItem emailMsg)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "EmailManager::WriteCommonMessageVariables.", new object[0]);
			base.WriteVariable("emailReceivedTime", emailMsg.ReceivedTime);
			base.WriteVariable("emailSubject", emailMsg.Subject);
			if (ObjectClass.IsOfClass(emailMsg.ClassName, "IPM.Schedule.Meeting.Resp") || ObjectClass.IsOfClass(emailMsg.ClassName, "IPM.Note.Rules.OofTemplate.Microsoft") || !string.IsNullOrEmpty(emailMsg.VotingInfo.Response))
			{
				base.WriteVariable("normalizedSubject", Utils.TrimSpaces(emailMsg.Subject));
			}
			else
			{
				base.WriteVariable("normalizedSubject", Utils.TrimSpaces((string)XsoUtil.SafeGetProperty(emailMsg, ItemSchema.NormalizedSubject, string.Empty)));
			}
			base.WriteReplyIntroType(IntroType.None);
			base.WriteVariable("read", emailMsg.IsRead);
			bool flag = emailMsg.IsRestricted || ObjectClass.IsOfClass(emailMsg.ClassName, "IPM.Note.Secure") || ObjectClass.IsOfClass(emailMsg.ClassName, "IPM.Note.SMIME");
			base.WriteVariable("drm", flag);
			base.WriteVariable("urgent", Importance.High == emailMsg.Importance);
			base.WriteVariable("attachments", 0 != emailMsg.AttachmentCollection.Count);
			base.WriteVariable("receivedDayOfWeek", (int)emailMsg.ReceivedTime.DayOfWeek);
			base.WriteVariable("receivedOffset", (int)(this.user.Now.Date - emailMsg.ReceivedTime.Date).TotalDays);
			base.WriteVariable("firstMessage", emailMsg.Id.ObjectId.Equals(this.firstMessageId));
			string varValue = null;
			string varValue2 = null;
			EmailManager.GetToAndCCFields(emailMsg, out varValue, out varValue2);
			base.WriteVariable("emailToField", varValue);
			base.WriteVariable("emailCCField", varValue2);
			base.WriteVariable("isRecorded", XsoUtil.IsMixedVoice(emailMsg.ClassName));
			PhoneNumber senderTelephoneNumber = XsoUtil.GetSenderTelephoneNumber(emailMsg);
			PhoneNumber phoneNumber = RetrieveVoicemailManager.ApplyDialingRules(this.user, senderTelephoneNumber, this.SenderInfo.DialPlan);
			base.TargetPhoneNumber = (phoneNumber ?? Util.GetNumberToDial(this.user, this.SenderInfo));
			if (XsoUtil.IsMissedCall(emailMsg.ClassName))
			{
				this.SpecifiedCallerDetails.ClearProperties();
				base.WriteVariable("isMissedCall", true);
				base.WriteVariable("senderCallerID", senderTelephoneNumber);
				this.SpecifiedCallerDetails.CallerId = senderTelephoneNumber;
				string senderName;
				if (this.SenderInfo is DefaultContactInfo)
				{
					senderName = (string)XsoUtil.SafeGetProperty(emailMsg, MessageItemSchema.VoiceMessageSenderName, string.Empty);
				}
				else
				{
					senderName = this.SenderInfo.DisplayName;
				}
				this.SetSenderName(this.SenderInfo.ADOrgPerson, senderName);
				this.SpecifiedCallerDetails.EmailSender = this.ReadVariable("emailSender");
			}
			else
			{
				base.WriteVariable("isMissedCall", false);
				base.WriteVariable("senderCallerID", null);
				this.SetSenderName(emailMsg.From);
			}
			ReplyForwardType replyForwardType = GlobCfg.SubjectToReplyForwardType(emailMsg.Subject);
			base.WriteVariable("isReply", replyForwardType == ReplyForwardType.Reply);
			base.WriteVariable("isForward", replyForwardType == ReplyForwardType.Forward);
			base.WriteVariable("messageLanguage", null);
			base.WriteVariable("languageDetected", null);
			base.WriteVariable("isHighPriority", Importance.High == emailMsg.Importance);
			this.IsSenderRoutable = (((null == emailMsg.From) ? new bool?(false) : emailMsg.From.IsRoutable(emailMsg.Session)) ?? false);
		}

		private void WriteMeetingRequestVariables(MessageItem emailMsg)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "EmailManager::WriteMeetingRequestVariables.", new object[0]);
			MeetingRequest meetingRequest = emailMsg as MeetingRequest;
			bool flag = meetingRequest != null && !meetingRequest.IsDelegated();
			base.WriteVariable("meetingRequest", flag);
			if (!flag)
			{
				base.WriteVariable("outOfDate", null);
				base.WriteVariable("meetingOver", null);
				base.WriteVariable("alreadyAccepted", null);
				base.WriteVariable("emailReplyTime", null);
				base.WriteVariable("emailRequestTime", null);
				base.WriteVariable("emailRequestTimeRange", null);
				base.WriteVariable("owner", null);
				base.WriteVariable("location", null);
				base.WriteVariable("calendarStatus", string.Empty);
				base.WriteVariable("meetingDayOfWeek", 0);
				base.WriteVariable("meetingOffset", 0);
				return;
			}
			using (CalendarItemBase calendarItem = EmailManager.GetCalendarItem(meetingRequest))
			{
				using (CalendarItemBase firstFutureInstance = this.GetFirstFutureInstance(calendarItem))
				{
					this.SetConflictStatus(firstFutureInstance);
					base.WriteVariable("outOfDate", meetingRequest.IsOutOfDate());
					base.WriteVariable("meetingOver", firstFutureInstance.EndTime < this.user.Now);
					base.WriteVariable("emailRequestTime", firstFutureInstance.StartTime);
					base.WriteVariable("emailRequestTimeRange", new TimeRange(firstFutureInstance.StartTime, firstFutureInstance.EndTime));
					base.WriteVariable("owner", calendarItem.IsOrganizer());
					base.WriteVariable("location", firstFutureInstance.Location);
					base.WriteVariable("meetingDayOfWeek", (int)firstFutureInstance.StartTime.DayOfWeek);
					base.WriteVariable("meetingOffset", (int)(firstFutureInstance.StartTime.Date - this.user.Now.Date).TotalDays);
					CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "MeetingRequestType={0}.", new object[]
					{
						meetingRequest.MeetingRequestType
					});
					if (MeetingMessageType.InformationalUpdate == meetingRequest.MeetingRequestType)
					{
						object obj = (calendarItem == null) ? null : XsoUtil.SafeGetProperty(calendarItem, CalendarItemBaseSchema.AppointmentReplyTime, null);
						CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "ReplyTime={0}.", new object[]
						{
							obj
						});
						if (obj == null || !(obj is ExDateTime))
						{
							base.WriteVariable("alreadyAccepted", null);
							base.WriteVariable("emailReplyTime", null);
						}
						else
						{
							base.WriteVariable("alreadyAccepted", true);
							base.WriteVariable("emailReplyTime", obj);
						}
					}
				}
			}
		}

		private CalendarItemBase GetFirstFutureInstance(CalendarItemBase potentialMaster)
		{
			CalendarItemBase result = potentialMaster;
			CalendarItem calendarItem = potentialMaster as CalendarItem;
			if (calendarItem == null || calendarItem.Recurrence == null)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "EmailManager::GetFirstFutureInstance was passed a non-recurring meeting", new object[0]);
				this.isRecurringMeetingRequest = false;
			}
			else
			{
				this.isRecurringMeetingRequest = true;
				CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "EmailManager::GetFirstFutureInstance was passed a recurring meeting", new object[0]);
				ExDateTime now = this.user.Now;
				ExDateTime endView = EmailManager.CalculateNextOccurenceEndWindow(this.user.Now, calendarItem.Recurrence);
				IList<OccurrenceInfo> occurrenceInfoList = calendarItem.Recurrence.GetOccurrenceInfoList(now, endView);
				using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.user.CreateSessionLock())
				{
					foreach (OccurrenceInfo occurrenceInfo in occurrenceInfoList)
					{
						if (occurrenceInfo != null && occurrenceInfo.VersionedId != null && occurrenceInfo.StartTime >= this.user.Now)
						{
							result = CalendarItemBase.Bind(mailboxSessionLock.Session, occurrenceInfo.VersionedId);
							break;
						}
					}
				}
			}
			return result;
		}

		private void WriteMeetingCancellationVariables(MessageItem emailMsg)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "EmailManager::WriteMeetingCancellationVariables.", new object[0]);
			MeetingCancellation meetingCancellation = emailMsg as MeetingCancellation;
			bool flag = null != meetingCancellation;
			base.WriteVariable("meetingCancellation", flag);
			if (!flag)
			{
				return;
			}
			base.WriteVariable("emailRequestTime", emailMsg[CalendarItemInstanceSchema.StartTime]);
		}

		private void SetConflictStatus(CalendarItemBase calendarItem)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "EmailManager::SetConflictStatus.", new object[0]);
			BusyType busyType = BusyType.Free;
			base.WriteVariable("calendarStatus", "free");
			using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.user.CreateSessionLock())
			{
				using (CalendarFolder calendarFolder = CalendarFolder.Bind(mailboxSessionLock.Session, mailboxSessionLock.Session.GetDefaultFolderId(DefaultFolderType.Calendar)))
				{
					AdjacencyOrConflictInfo[] adjacentOrConflictingItems = calendarFolder.GetAdjacentOrConflictingItems(calendarItem);
					foreach (AdjacencyOrConflictInfo adjacencyOrConflictInfo in adjacentOrConflictingItems)
					{
						if ((adjacencyOrConflictInfo.AdjacencyOrConflictType & AdjacencyOrConflictType.Conflicts) != (AdjacencyOrConflictType)0 && adjacencyOrConflictInfo.FreeBusyStatus != BusyType.Free)
						{
							if (adjacencyOrConflictInfo.FreeBusyStatus == BusyType.Tentative && busyType == BusyType.Free)
							{
								busyType = BusyType.Tentative;
								base.WriteVariable("calendarStatus", "tentative");
							}
							else if (adjacencyOrConflictInfo.FreeBusyStatus == BusyType.Busy && busyType != BusyType.OOF)
							{
								busyType = BusyType.Busy;
								base.WriteVariable("calendarStatus", "busy");
							}
							else if (adjacencyOrConflictInfo.FreeBusyStatus == BusyType.OOF)
							{
								busyType = BusyType.OOF;
								base.WriteVariable("calendarStatus", "oof");
								break;
							}
						}
					}
					CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "SetConflictStatus found mostBusyType={0}.", new object[]
					{
						busyType
					});
					base.WriteVariable("free", busyType == BusyType.Free);
				}
			}
		}

		private void SetSenderName(Participant p)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "EmailManager::SetSenderName.", new object[0]);
			IADRecipientLookup iadrecipientLookup = ADRecipientLookupFactory.CreateFromUmUser(this.user);
			ADRecipient recipient = iadrecipientLookup.LookupByParticipant(p);
			this.SetSenderName(recipient, p.DisplayName);
		}

		private void SetSenderName(IADRecipient recipient, string senderName)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "EmailManager::SetSenderName.", new object[0]);
			if (recipient == null)
			{
				base.WriteVariable("emailSender", senderName);
				return;
			}
			base.SetRecordedName("emailSender", recipient);
		}

		private void Undelete()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "EmailManager::Undelete with pendingDelete={0}.", new object[]
			{
				this.pendingDeletion
			});
			if (this.pendingDeletion != null)
			{
				base.WriteVariable("undeletedAConversation", this.pendingDeletion is EmailManager.ConversationPendingDeletion);
				this.pendingDeletion.Revert();
				this.SeekToMessage(this.pendingDeletion.UndeleteRefId);
				this.pendingDeletion = null;
			}
			base.WriteVariable("canUndelete", false);
		}

		private void FlagMessage()
		{
			using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.user.CreateSessionLock())
			{
				using (MessageItem messageItem = Item.BindAsMessage(mailboxSessionLock.Session, this.messageItemList.CurrentStoreObjectId))
				{
					messageItem.OpenAsReadWrite();
					messageItem.SetFlag(Strings.FollowUp.ToString(this.user.TelephonyCulture), null, null);
					ConflictResolutionResult conflictResolutionResult = messageItem.Save(SaveMode.ResolveConflicts);
					if (SaveResult.IrresolvableConflict == conflictResolutionResult.SaveStatus)
					{
						throw new SaveConflictException(ServerStrings.ExSaveFailedBecauseOfConflicts(this.messageItemList.CurrentStoreObjectId), conflictResolutionResult);
					}
				}
			}
		}

		private void MarkRead()
		{
			using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.user.CreateSessionLock())
			{
				using (MessageItem messageItem = Item.BindAsMessage(mailboxSessionLock.Session, this.messageItemList.CurrentStoreObjectId))
				{
					if (!messageItem.IsRead)
					{
						messageItem.OpenAsReadWrite();
						messageItem.MarkAsRead(false, true);
						ConflictResolutionResult conflictResolutionResult = messageItem.Save(SaveMode.ResolveConflicts);
						if (SaveResult.IrresolvableConflict == conflictResolutionResult.SaveStatus)
						{
							throw new SaveConflictException(ServerStrings.ExSaveFailedBecauseOfConflicts(this.messageItemList.CurrentStoreObjectId), conflictResolutionResult);
						}
					}
				}
			}
		}

		private void MarkUnread()
		{
			using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.user.CreateSessionLock())
			{
				using (MessageItem messageItem = Item.BindAsMessage(mailboxSessionLock.Session, this.messageItemList.CurrentStoreObjectId))
				{
					if (messageItem.IsRead)
					{
						messageItem.OpenAsReadWrite();
						messageItem.MarkAsUnread(true);
						ConflictResolutionResult conflictResolutionResult = messageItem.Save(SaveMode.ResolveConflicts);
						if (SaveResult.IrresolvableConflict == conflictResolutionResult.SaveStatus)
						{
							throw new SaveConflictException(ServerStrings.ExSaveFailedBecauseOfConflicts(this.messageItemList.CurrentStoreObjectId), conflictResolutionResult);
						}
					}
				}
			}
		}

		private void HideThread()
		{
			byte[] indexBytes = this.messageItemList.SafeGetProperty<byte[]>(ItemSchema.ConversationIndex, null);
			EmailManager.Conversation conversation = EmailManager.Conversation.FromConversationIndex(indexBytes);
			this.hiddenThreads.Add(conversation, null);
			CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "EmailManager::HideThread has hidden all messages with conversation idx={0}.", new object[]
			{
				conversation.GetHashCode()
			});
		}

		private void DeleteThread()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "EmailManager::DeleteThread.", new object[0]);
			if (this.pendingDeletion != null)
			{
				using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.user.CreateSessionLock())
				{
					this.pendingDeletion.Commit(mailboxSessionLock.Session);
					this.pendingDeletion = null;
				}
			}
			byte[] array = this.messageItemList.SafeGetProperty<byte[]>(ItemSchema.ConversationIndex, null);
			string text = this.messageItemList.SafeGetProperty<string>(ItemSchema.ConversationTopic, null);
			if (array == null || array.Length < 22 || string.IsNullOrEmpty(text))
			{
				this.pendingDeletion = new EmailManager.SingleItemPendingDeletion(base.CallSession, this.messageItemList);
			}
			else
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "EmailManager::DeleteThread finding all messages with topic={0}.", new object[]
				{
					text
				});
				QueryFilter filter = new TextFilter(ItemSchema.ConversationTopic, text, MatchOptions.ExactPhrase, MatchFlags.Default);
				List<StoreObjectId> conversations;
				using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock2 = this.user.CreateSessionLock())
				{
					using (OneTimeSearch oneTimeSearch = OneTimeSearch.Execute(this.user, mailboxSessionLock2.Session, this.InboxId, filter))
					{
						conversations = EmailManager.GetConversations(mailboxSessionLock2.Session, oneTimeSearch.FolderId, EmailManager.Conversation.FromConversationIndex(array));
					}
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "EmailManager::DeleteThread found c={0} such messages.", new object[]
				{
					(conversations == null) ? 0 : conversations.Count
				});
				EmailManager.Conversation conversation = EmailManager.Conversation.FromConversationIndex(array);
				this.hiddenThreads.Add(conversation, null);
				this.pendingDeletion = new EmailManager.ConversationPendingDeletion(base.CallSession, this.messageItemList.CurrentStoreObjectId, this.hiddenThreads, conversation, conversations.ToArray());
			}
			if (this.numDeletions < 2U && (this.numDeletions += 1U) >= 2U)
			{
				base.WriteVariable("playedUndelete", true);
			}
			base.WriteVariable("canUndelete", this.pendingDeletion != null);
		}

		private void FindByName()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "EmailManager::FindByName.", new object[0]);
			OneTimeSearch oneTimeSearch = null;
			QueryResult queryResult = null;
			UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = null;
			Folder folder = null;
			try
			{
				ContactSearchItem contactSearchItem = (ContactSearchItem)this.ReadVariable("directorySearchResult");
				if (contactSearchItem == null)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "EmailManager::FindByName received a null search result!", new object[0]);
					base.WriteVariable("numMessagesFromName", 0);
					base.WriteVariable("findByName", string.Empty);
				}
				else
				{
					if (contactSearchItem.Recipient == null)
					{
						base.WriteVariable("findByName", contactSearchItem.FullName);
					}
					else if (!string.IsNullOrEmpty(contactSearchItem.Recipient.LegacyExchangeDN))
					{
						base.SetRecordedName("findByName", contactSearchItem.Recipient);
					}
					QueryFilter queryFilter = Filters.CreateFindByNameFilter(contactSearchItem);
					if (queryFilter != null)
					{
						mailboxSessionLock = this.user.CreateSessionLock();
						oneTimeSearch = OneTimeSearch.Execute(this.user, mailboxSessionLock.Session, this.InboxId, queryFilter);
						folder = Folder.Bind(mailboxSessionLock.Session, oneTimeSearch.FolderId);
						queryResult = folder.ItemQuery(ItemQueryType.None, null, null, EmailManager.viewProperties);
						int findByNameCount = this.GetFindByNameCount(queryResult);
						base.WriteVariable("numMessagesFromName", findByNameCount);
						if (findByNameCount > 0)
						{
							if (this.findByNameResults != null)
							{
								this.findByNameResults.Dispose();
								this.findByNameResults = null;
							}
							if (this.pendingDeletion != null)
							{
								this.pendingDeletion.Commit(mailboxSessionLock.Session);
								this.pendingDeletion = null;
								base.WriteVariable("canUndelete", false);
							}
							this.folderId = oneTimeSearch.FolderId;
							this.findByNameResults = oneTimeSearch;
							oneTimeSearch = null;
							this.firstMessageId = null;
							this.messageItemList = new MessageItemList(this.user, this.folderId, MessageItemListSortType.Email, EmailManager.viewProperties);
							base.WriteVariable("inFindMode", true);
						}
					}
					else
					{
						PIIMessage data = PIIMessage.Create(PIIType._User, this.user.ADRecipient.DistinguishedName);
						CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, data, "Unable to generate the search filter for Contact or Gal user with id {0} because neither displayname nor email properties are populated. Caller _User. ", new object[]
						{
							contactSearchItem.Id ?? "not available"
						});
						base.WriteVariable("numMessagesFromName", 0);
					}
				}
			}
			finally
			{
				if (queryResult != null)
				{
					queryResult.Dispose();
					queryResult = null;
				}
				if (folder != null)
				{
					folder.Dispose();
					folder = null;
				}
				if (mailboxSessionLock != null)
				{
					mailboxSessionLock.Dispose();
					mailboxSessionLock = null;
				}
				if (oneTimeSearch != null)
				{
					oneTimeSearch.Dispose();
					oneTimeSearch = null;
				}
			}
		}

		private int GetFindByNameCount(QueryResult query)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "EmailManager::GetFindByNameCount.", new object[0]);
			int num = 0;
			int currentRow = query.CurrentRow;
			object[][] rows = query.GetRows(MessageItemList.PageSize);
			while (rows != null && 0 < rows.Length)
			{
				foreach (object[] array2 in rows)
				{
					string itemClass = (string)array2[1];
					byte[] array3 = array2[2] as byte[];
					if (EmailManager.CanReadMessageClassWithTui(itemClass) && (array3 == null || !this.hiddenThreads.ContainsKey(EmailManager.Conversation.FromConversationIndex(array3))))
					{
						num++;
					}
				}
				rows = query.GetRows(MessageItemList.PageSize);
			}
			if (query.CurrentRow != currentRow)
			{
				query.SeekToOffset(SeekReference.OriginBeginning, currentRow);
			}
			return num;
		}

		private static PropertyDefinition[] viewProperties = new PropertyDefinition[]
		{
			ItemSchema.Id,
			StoreObjectSchema.ItemClass,
			ItemSchema.ConversationIndex,
			ItemSchema.ConversationTopic,
			MessageItemSchema.IsRead
		};

		private StoreObjectId firstMessageId;

		private StoreObjectId folderId;

		private UMSubscriber user;

		private ContactInfo senderInfo;

		private EmailManager.PendingDeletion pendingDeletion;

		private uint numDeletions;

		private Dictionary<EmailManager.Conversation, object> hiddenThreads;

		private OneTimeSearch findByNameResults;

		private bool isRecurringMeetingRequest;

		private bool isSenderRoutable;

		private MessageItemList messageItemList;

		internal class ConfigClass : ActivityManagerConfig
		{
			internal ConfigClass(ActivityManagerConfig manager) : base(manager)
			{
			}

			internal override ActivityManager CreateActivityManager(ActivityManager manager)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Constructing Email activity manager.", new object[0]);
				return new EmailManager(manager, this);
			}
		}

		private abstract class PendingDeletion
		{
			protected PendingDeletion(BaseUMCallSession vo)
			{
				this.vo = vo;
			}

			internal StoreObjectId UndeleteRefId
			{
				get
				{
					return this.Id;
				}
			}

			protected BaseUMCallSession CallSession
			{
				get
				{
					return this.vo;
				}
			}

			protected bool IsValid
			{
				get
				{
					return this.valid;
				}
				set
				{
					this.valid = value;
				}
			}

			protected StoreObjectId Id
			{
				get
				{
					return this.objectId;
				}
				set
				{
					this.objectId = value;
				}
			}

			internal virtual void Commit(MailboxSession session)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "Committing a pendingDeletion of type={0}.", new object[]
				{
					this
				});
				this.IsValid = false;
			}

			internal virtual void Revert()
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "Reverting a pendingDeletion of type={0}.", new object[]
				{
					this
				});
				this.IsValid = false;
			}

			private BaseUMCallSession vo;

			private bool valid = true;

			private StoreObjectId objectId;
		}

		private class SingleItemPendingDeletion : EmailManager.PendingDeletion
		{
			internal SingleItemPendingDeletion(BaseUMCallSession vo, MessageItemList storeItemList) : base(vo)
			{
				base.Id = storeItemList.CurrentStoreObjectId;
				this.storeItemList = storeItemList;
				this.storeItemList.Ignore(storeItemList.CurrentStoreObjectId);
			}

			internal override void Commit(MailboxSession session)
			{
				if (base.IsValid && session != null)
				{
					base.Commit(session);
					session.Delete(DeleteItemFlags.MoveToDeletedItems, new StoreId[]
					{
						base.Id
					});
					base.CallSession.IncrementCounter(SubscriberAccessCounters.EmailMessagesDeleted);
				}
			}

			internal override void Revert()
			{
				if (base.IsValid)
				{
					base.Revert();
					this.storeItemList.UnIgnore(base.Id);
				}
			}

			private MessageItemList storeItemList;
		}

		private class ConversationPendingDeletion : EmailManager.PendingDeletion
		{
			internal ConversationPendingDeletion(BaseUMCallSession vo, StoreObjectId undeleteRefId, Dictionary<EmailManager.Conversation, object> hiddenThreads, EmailManager.Conversation conversation, params StoreObjectId[] ids) : base(vo)
			{
				base.Id = undeleteRefId;
				this.hiddenThreads = hiddenThreads;
				this.conversation = conversation;
				this.objectIds = ids;
			}

			internal override void Commit(MailboxSession session)
			{
				if (base.IsValid && session != null)
				{
					base.Commit(session);
					session.Delete(DeleteItemFlags.MoveToDeletedItems, this.objectIds);
					base.CallSession.IncrementCounter(SubscriberAccessCounters.EmailMessagesDeleted, (long)this.objectIds.Length);
				}
			}

			internal override void Revert()
			{
				if (base.IsValid)
				{
					base.Revert();
					this.hiddenThreads.Remove(this.conversation);
				}
			}

			private StoreObjectId[] objectIds;

			private Dictionary<EmailManager.Conversation, object> hiddenThreads;

			private EmailManager.Conversation conversation;
		}

		private class MeetingResponse : XsoRecordedMessage
		{
			internal MeetingResponse(BaseUMCallSession vo, StoreObjectId originalId, ResponseType responseType, UMSubscriber user, EmailManager context) : base(vo, user, context)
			{
				this.originalId = originalId;
				this.responseType = responseType;
			}

			public override void DoPostSubmit()
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "MeetingResponse::DoPostSubmit.", new object[0]);
				EmailManager emailManager = base.Manager as EmailManager;
				if (emailManager != null)
				{
					emailManager.DeleteCurrentMessage(false);
				}
				if (ResponseType.Decline == this.responseType && this.correlatedId != null)
				{
					using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = base.User.CreateSessionLock())
					{
						mailboxSessionLock.Session.Delete(DeleteItemFlags.MoveToDeletedItems, new StoreId[]
						{
							this.correlatedId
						});
						CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "Successfully deleted calendar item on meeting decline.", new object[0]);
					}
				}
				base.DoPostSubmit();
			}

			protected override MessageItem GenerateMessage(MailboxSession session)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "MeetingResponse::GenerateResponse.", new object[0]);
				EmailManager emailManager = (EmailManager)base.Manager;
				Microsoft.Exchange.Data.Storage.MeetingResponse result = null;
				using (MeetingRequest meetingRequest = MeetingRequest.Bind(session, this.originalId))
				{
					meetingRequest.OpenAsReadWrite();
					using (CalendarItemBase calendarItemBase = meetingRequest.UpdateCalendarItem(false))
					{
						if (calendarItemBase != null)
						{
							result = XsoUtil.RespondToMeetingRequest(calendarItemBase, this.responseType);
							calendarItemBase.Load(new PropertyDefinition[]
							{
								ItemSchema.HasAttachment
							});
							base.SetAttachmentName(calendarItemBase.AttachmentCollection);
							this.correlatedId = calendarItemBase.Id;
							CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "MeetingResponse::GenerateMessage successfully built response.", new object[0]);
						}
					}
				}
				return result;
			}

			private ResponseType responseType;

			private StoreObjectId originalId;

			private VersionedId correlatedId;
		}

		private abstract class EmailReplyBase : XsoRecordedMessage
		{
			internal EmailReplyBase(BaseUMCallSession vo, StoreObjectId originalId, UMSubscriber user, EmailManager context) : base(vo, user, context)
			{
				this.originalId = originalId;
			}

			public override void DoPostSubmit()
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "EmailReply::DoPostSubmit.", new object[0]);
				base.Session.IncrementCounter(SubscriberAccessCounters.ReplyMessagesSent);
				base.DoPostSubmit();
			}

			protected override MessageItem GenerateMessage(MailboxSession session)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "EmailReply::GenerateResponse.", new object[0]);
				MessageItem result;
				using (MessageItem messageItem = MessageItem.Bind(session, this.originalId))
				{
					messageItem.OpenAsReadWrite();
					base.SetAttachmentName(messageItem.AttachmentCollection);
					result = this.CreateReplyMessage(messageItem, base.PrepareMessageBodyPrefix(messageItem), BodyFormat.TextHtml, XsoUtil.GetDraftsFolderId(session));
				}
				return result;
			}

			protected abstract MessageItem CreateReplyMessage(MessageItem originalMessage, string bodyPrefix, BodyFormat bodyFormat, StoreObjectId parentFolderId);

			protected override void AddRecordedMessageText(MessageContentBuilder content)
			{
				content.AddRecordedReplyText(base.User.DisplayName);
			}

			protected override void AddMessageHeader(Item originalMessage, MessageContentBuilder content)
			{
				content.AddEmailHeader((MessageItem)originalMessage);
			}

			private StoreObjectId originalId;
		}

		private class EmailReply : EmailManager.EmailReplyBase
		{
			internal EmailReply(BaseUMCallSession vo, StoreObjectId originalId, UMSubscriber user, EmailManager context) : base(vo, originalId, user, context)
			{
			}

			protected override MessageItem CreateReplyMessage(MessageItem originalMessage, string bodyPrefix, BodyFormat bodyFormat, StoreObjectId parentFolderId)
			{
				ReplyForwardConfiguration replyForwardConfiguration = new ReplyForwardConfiguration(bodyFormat);
				replyForwardConfiguration.AddBodyPrefix(bodyPrefix);
				IADSystemConfigurationLookup iadsystemConfigurationLookup = ADSystemConfigurationLookupFactory.CreateFromOrganizationId(base.User.ADUser.OrganizationId);
				AcceptedDomain defaultAcceptedDomain = iadsystemConfigurationLookup.GetDefaultAcceptedDomain();
				replyForwardConfiguration.ConversionOptionsForSmime = new InboundConversionOptions(defaultAcceptedDomain.DomainName.ToString());
				replyForwardConfiguration.ConversionOptionsForSmime.UserADSession = ADRecipientLookupFactory.CreateFromUmUser(base.User).ScopedRecipientSession;
				return originalMessage.CreateReply(parentFolderId, replyForwardConfiguration);
			}
		}

		private class EmailReplyAll : EmailManager.EmailReplyBase
		{
			internal EmailReplyAll(BaseUMCallSession vo, StoreObjectId originalId, UMSubscriber user, EmailManager context) : base(vo, originalId, user, context)
			{
			}

			protected override MessageItem CreateReplyMessage(MessageItem originalMessage, string bodyPrefix, BodyFormat bodyFormat, StoreObjectId parentFolderId)
			{
				ReplyForwardConfiguration replyForwardConfiguration = new ReplyForwardConfiguration(bodyFormat);
				replyForwardConfiguration.AddBodyPrefix(bodyPrefix);
				IADSystemConfigurationLookup iadsystemConfigurationLookup = ADSystemConfigurationLookupFactory.CreateFromOrganizationId(base.User.ADUser.OrganizationId);
				AcceptedDomain defaultAcceptedDomain = iadsystemConfigurationLookup.GetDefaultAcceptedDomain();
				replyForwardConfiguration.ConversionOptionsForSmime = new InboundConversionOptions(defaultAcceptedDomain.DomainName.ToString());
				replyForwardConfiguration.ConversionOptionsForSmime.UserADSession = ADRecipientLookupFactory.CreateFromUmUser(base.User).ScopedRecipientSession;
				return originalMessage.CreateReplyAll(parentFolderId, replyForwardConfiguration);
			}
		}

		private class EmailForward : XsoRecordedMessage
		{
			internal EmailForward(BaseUMCallSession vo, StoreObjectId originalId, UMSubscriber user, EmailManager context) : base(vo, user, context)
			{
				this.originalId = originalId;
			}

			public override void DoPostSubmit()
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "EmailForward::DoPostSubmit.", new object[0]);
				base.Session.IncrementCounter(SubscriberAccessCounters.ForwardMessagesSent);
				base.DoPostSubmit();
			}

			protected override MessageItem GenerateMessage(MailboxSession session)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "EmailForward::GenerateResponse.", new object[0]);
				MessageItem result;
				using (MessageItem messageItem = MessageItem.Bind(session, this.originalId))
				{
					base.SetAttachmentName(messageItem.AttachmentCollection);
					ReplyForwardConfiguration replyForwardConfiguration = new ReplyForwardConfiguration(BodyFormat.TextHtml);
					replyForwardConfiguration.AddBodyPrefix(base.PrepareMessageBodyPrefix(messageItem), BodyInjectionFormat.Html);
					IADSystemConfigurationLookup iadsystemConfigurationLookup = ADSystemConfigurationLookupFactory.CreateFromOrganizationId(base.User.ADUser.OrganizationId);
					AcceptedDomain defaultAcceptedDomain = iadsystemConfigurationLookup.GetDefaultAcceptedDomain();
					replyForwardConfiguration.ConversionOptionsForSmime = new InboundConversionOptions(defaultAcceptedDomain.DomainName.ToString());
					replyForwardConfiguration.ConversionOptionsForSmime.UserADSession = ADRecipientLookupFactory.CreateFromUmUser(base.User).ScopedRecipientSession;
					MessageItem messageItem2 = messageItem.CreateForward(XsoUtil.GetDraftsFolderId(session), replyForwardConfiguration);
					messageItem2[MessageItemSchema.VoiceMessageAttachmentOrder] = XsoUtil.GetAttachmentOrderString(messageItem);
					result = messageItem2;
				}
				return result;
			}

			protected override void AddRecordedMessageText(MessageContentBuilder content)
			{
				content.AddRecordedForwardText(base.User.DisplayName);
			}

			protected override void AddMessageHeader(Item originalMessage, MessageContentBuilder content)
			{
				content.AddEmailHeader((MessageItem)originalMessage);
			}

			private StoreObjectId originalId;
		}

		private class Conversation : IEquatable<EmailManager.Conversation>
		{
			private Conversation()
			{
			}

			private Conversation(byte[] indexBytes)
			{
				this.indexBytes = indexBytes;
				if (indexBytes == null || indexBytes.Length < 22)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "Conversation constructed without a valid conversation index.", new object[0]);
					this.hash = EmailManager.Conversation.r.Next();
					return;
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "Conversation constructor with a valid conversation index", new object[0]);
				this.hash = BitConverter.ToInt32(indexBytes, 17);
			}

			public bool Equals(EmailManager.Conversation other)
			{
				if (this.indexBytes == null || other.indexBytes == null)
				{
					return false;
				}
				if (this.indexBytes.Length < 22 || other.indexBytes.Length < 22)
				{
					return false;
				}
				for (int i = 0; i < 22; i++)
				{
					if (this.indexBytes[i] != other.indexBytes[i])
					{
						return false;
					}
				}
				return true;
			}

			public override int GetHashCode()
			{
				return this.hash;
			}

			internal static EmailManager.Conversation FromConversationIndex(byte[] indexBytes)
			{
				return new EmailManager.Conversation(indexBytes);
			}

			private static Random r = new Random();

			private byte[] indexBytes;

			private int hash;
		}
	}
}
