using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCommon.MessageContent;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class RetrieveVoicemailManager : SendMessageManager
	{
		public bool MessageListIsNull
		{
			get
			{
				return this.voiceMessageList.MessageListIsNull;
			}
		}

		internal NameOrNumberOfCaller SpecifiedCallerDetails { get; private set; }

		internal RetrieveVoicemailManager(ActivityManager manager, RetrieveVoicemailManager.ConfigClass config) : base(manager, config)
		{
		}

		internal bool IsForwardEnabled
		{
			get
			{
				return !this.GlobalManager.LimitedOVAAccess && this.GlobalManager.AddressBookEnabled;
			}
		}

		internal bool DrmIsEnabled
		{
			get
			{
				return this.drmIsEnabled;
			}
		}

		internal override bool LargeGrammarsNeeded
		{
			get
			{
				return true;
			}
		}

		internal bool IsForwardToContactEnabled
		{
			get
			{
				return !this.GlobalManager.LimitedOVAAccess && this.GlobalManager.ContactsAccessEnabled;
			}
		}

		internal bool IsFindEnabled
		{
			get
			{
				return !this.GlobalManager.LimitedOVAAccess && this.GlobalManager.AddressBookEnabled;
			}
		}

		internal bool IsFindByContactEnabled
		{
			get
			{
				return !this.GlobalManager.LimitedOVAAccess && this.GlobalManager.ContactsAccessEnabled;
			}
		}

		internal static PhoneNumber ApplyDialingRules(UMSubscriber caller, PhoneNumber senderPhone, UMDialPlan targetDialPlan)
		{
			PIIMessage data = PIIMessage.Create(PIIType._PhoneNumber, senderPhone);
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, null, data, "ApplyDialingRules(phone: _PhoneNumber targetDialplan: {0}.", new object[]
			{
				(targetDialPlan == null) ? "<null>" : targetDialPlan.Name
			});
			if (PhoneNumber.IsNullOrEmpty(senderPhone))
			{
				return null;
			}
			PhoneNumber phoneNumber = null;
			PhoneNumber phoneNumber2 = DialPermissions.Canonicalize(senderPhone, caller.DialPlan, null, targetDialPlan);
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, null, data, "DialPermissions.Canonicalize(InputPhone:_PhoneNumber, OrigDP:{0}, TargetDP:{1}, CanonicalPhone:{2}.", new object[]
			{
				caller.DialPlan.Name,
				(targetDialPlan == null) ? "<null>" : targetDialPlan.Name,
				phoneNumber2
			});
			if (phoneNumber2 == null)
			{
				return null;
			}
			bool flag = DialPermissions.Check(phoneNumber2, (ADUser)caller.ADRecipient, caller.DialPlan, targetDialPlan, out phoneNumber);
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, null, "CheckDialPermissions(Phone:{0}, OrigDP:{1}, TargetDP:{2}, Allowed:{3}, Dial:{4}.", new object[]
			{
				phoneNumber2,
				caller.DialPlan.Name,
				(targetDialPlan == null) ? "<null>" : targetDialPlan.Name,
				flag,
				phoneNumber
			});
			if (!flag)
			{
				return null;
			}
			return phoneNumber;
		}

		internal override void Start(BaseUMCallSession vo, string refInfo)
		{
			vo.IncrementCounter(SubscriberAccessCounters.VoiceMessageQueueAccessed);
			this.caller = vo.CurrentCallContext.CallerInfo;
			this.SpecifiedCallerDetails = new NameOrNumberOfCaller(NameOrNumberOfCaller.TypeOfVoiceCall.VoicemailCall);
			this.drmIsEnabled = (this.caller != null && this.caller.DRMPolicyForInterpersonal != DRMProtectionOptions.None);
			using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.caller.CreateSessionLock())
			{
				using (UMSearchFolder umsearchFolder = UMSearchFolder.Get(mailboxSessionLock.Session, UMSearchFolder.Type.VoiceMail))
				{
					this.voicemailSearchFolderId = umsearchFolder.SearchFolder.Id.ObjectId;
				}
			}
			base.Start(vo, refInfo);
		}

		internal override void CheckAuthorization(UMSubscriber u)
		{
			if (!u.IsAuthenticated)
			{
				base.CheckAuthorization(u);
			}
		}

		internal override TransitionBase ExecuteAction(string action, BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, "RetrieveVoicemailManager asked to do action {0}.", new object[]
			{
				action
			});
			string input = null;
			if (string.Equals(action, "getNewMessages", StringComparison.OrdinalIgnoreCase))
			{
				this.voiceMessageList.InitializeCurrentPagerList(new MessageItemList(this.caller, this.voicemailSearchFolderId, this.GetSortTypeForUnreadVoicemessages(this.caller), RetrieveVoicemailManager.viewProperties));
				this.readingSavedMessages = false;
				this.readAsFirstMessage = true;
				input = this.GetNextMessage(vo);
			}
			else if (string.Equals(action, "getSavedMessages", StringComparison.OrdinalIgnoreCase))
			{
				this.voiceMessageList.InitializeCurrentPagerList(new MessageItemList(this.caller, this.voicemailSearchFolderId, MessageItemListSortType.LifoVoicemail, RetrieveVoicemailManager.viewProperties));
				this.readingSavedMessages = true;
				this.readAsFirstMessage = !this.haveReadSavedMessages;
				this.haveReadSavedMessages = true;
				input = this.GetNextMessage(vo);
			}
			else if (string.Equals(action, "getNextMessage", StringComparison.OrdinalIgnoreCase))
			{
				input = this.GetNextMessage(vo);
			}
			else if (string.Equals(action, "getPreviousMessage", StringComparison.OrdinalIgnoreCase))
			{
				input = this.GetPreviousMessage(vo);
			}
			else if (string.Equals(action, "getPriorityOfMessage", StringComparison.OrdinalIgnoreCase))
			{
				input = this.GetPriorityOfMessage();
			}
			else if (string.Equals(action, "deleteVoiceMail", StringComparison.OrdinalIgnoreCase))
			{
				this.DeleteCurrentMessage();
			}
			else if (string.Equals(action, "replyVoiceMail", StringComparison.OrdinalIgnoreCase))
			{
				base.SendMsg = new VoicemailReply(vo, this.voiceMessageList.CurrentMessageBeingRead, this.caller, this.currentSender, this, (bool)this.ReadVariable("protected"));
				base.WriteReplyIntroType(IntroType.Reply);
			}
			else if (string.Equals(action, "forwardVoiceMail", StringComparison.OrdinalIgnoreCase))
			{
				base.SendMsg = new VoicemailForward(vo, this.voiceMessageList.CurrentMessageBeingRead, this.caller, this);
				base.WriteReplyIntroType(IntroType.Forward);
			}
			else if (string.Equals(action, "undeleteVoiceMail", StringComparison.OrdinalIgnoreCase))
			{
				this.UndeleteCurrentMessage();
			}
			else if (string.Equals(action, "saveVoiceMail", StringComparison.OrdinalIgnoreCase))
			{
				this.SaveMessage();
			}
			else if (string.Equals(action, "markUnreadVoiceMail", StringComparison.OrdinalIgnoreCase))
			{
				this.MarkUnread();
			}
			else if (string.Equals(action, "flagVoiceMail", StringComparison.OrdinalIgnoreCase))
			{
				this.FlagMessage();
			}
			else if (string.Equals(action, "getEnvelopInfo", StringComparison.OrdinalIgnoreCase))
			{
				this.GetEnvelopInformation();
			}
			else if (string.Equals(action, "findByName", StringComparison.OrdinalIgnoreCase))
			{
				this.FindByName();
			}
			else
			{
				if (!string.Equals(action, "getMessageReadProperty", StringComparison.OrdinalIgnoreCase))
				{
					return base.ExecuteAction(action, vo);
				}
				input = (this.readingSavedMessages ? "currentSavedMessage" : "currentNewMessage");
			}
			return base.CurrentActivity.GetTransition(input);
		}

		internal string ReplyAll(BaseUMCallSession vo)
		{
			base.SendMsg = new VoicemailReplyAll(vo, this.voiceMessageList.CurrentMessageBeingRead, this.caller, this.currentSender, this, (bool)this.ReadVariable("protected"));
			base.WriteReplyIntroType(IntroType.ReplyAll);
			return null;
		}

		protected override void InternalDispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, "RetrieveVoicemailManager::Dispose.", new object[0]);
					if (this.findByNameResults != null)
					{
						this.findByNameResults.Dispose();
					}
					if (this.pendingDeletionObjectId != null)
					{
						this.CommitPendingDeletion();
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
			return DisposeTracker.Get<RetrieveVoicemailManager>(this);
		}

		private MessageItemListSortType GetSortTypeForUnreadVoicemessages(UMSubscriber user)
		{
			if (user.ConfigFolder.ReadUnreadVoicemailInFIFOOrder)
			{
				return MessageItemListSortType.FifoVoicemail;
			}
			return MessageItemListSortType.LifoVoicemail;
		}

		private string GetNextMessage(BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, "Fetching the next Message.", new object[0]);
			string result = null;
			using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.caller.CreateSessionLock())
			{
				if (!this.voiceMessageList.GetNextMessageToRead(this.readingSavedMessages))
				{
					return this.readingSavedMessages ? "noSavedMessages" : "noNewMessages";
				}
				using (MessageItem messageItem = Item.BindAsMessage(mailboxSessionLock.Session, this.voiceMessageList.CurrentMessageBeingRead, RetrieveVoicemailManager.voiceMessageProperties))
				{
					messageItem.OpenAsReadWrite();
					this.LoadCurrentMessageProperties(messageItem);
					this.readAsFirstMessage = false;
				}
			}
			return result;
		}

		private string GetPreviousMessage(BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, "Fetching the previous Message.", new object[0]);
			string result = null;
			using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.caller.CreateSessionLock())
			{
				if (!this.voiceMessageList.GetPreviousMessageToRead())
				{
					result = "noPreviousNewMessages";
				}
				else
				{
					using (MessageItem messageItem = Item.BindAsMessage(mailboxSessionLock.Session, this.voiceMessageList.CurrentMessageBeingRead, RetrieveVoicemailManager.voiceMessageProperties))
					{
						this.LoadCurrentMessageProperties(messageItem);
						this.readAsFirstMessage = false;
					}
				}
			}
			return result;
		}

		private void LoadCurrentMessageProperties(MessageItem currentMessage)
		{
			this.SpecifiedCallerDetails.ClearProperties();
			base.MessagePlayerContext.Reset(currentMessage.Id.ObjectId);
			PhoneNumber senderTelephoneNumber = XsoUtil.GetSenderTelephoneNumber(currentMessage);
			base.WriteVariable("senderCallerID", senderTelephoneNumber);
			this.SpecifiedCallerDetails.CallerId = senderTelephoneNumber;
			ContactInfo contactInfo = null;
			if (null != currentMessage.From && currentMessage.From.EmailAddress != null)
			{
				contactInfo = ContactInfo.FindByParticipant(this.caller, currentMessage.From);
			}
			this.currentSender = ((contactInfo != null) ? contactInfo : new DefaultContactInfo());
			bool flag = this.currentSender is DefaultContactInfo;
			PhoneNumber phoneNumber = RetrieveVoicemailManager.ApplyDialingRules(this.caller, senderTelephoneNumber, this.currentSender.DialPlan);
			base.TargetPhoneNumber = phoneNumber;
			base.WriteVariable("knowSenderPhoneNumber", null != phoneNumber);
			string text = flag ? null : this.currentSender.EMailAddress;
			base.WriteVariable("knowVoicemailSender", !string.IsNullOrEmpty(text) && SmtpAddress.IsValidSmtpAddress(text));
			base.WriteVariable("messageReceivedTime", currentMessage.ReceivedTime);
			this.SpecifiedCallerDetails.MessageReceivedTime = currentMessage.ReceivedTime;
			base.WriteVariable("receivedDayOfWeek", (int)currentMessage.ReceivedTime.DayOfWeek);
			base.WriteVariable("receivedOffset", (int)(this.caller.Now.Date - currentMessage.ReceivedTime.Date).TotalDays);
			if (flag)
			{
				string varValue = (string)XsoUtil.SafeGetProperty(currentMessage, MessageItemSchema.VoiceMessageSenderName, string.Empty);
				base.WriteVariable("emailSender", varValue);
			}
			else if (this.currentSender.ADOrgPerson == null)
			{
				base.WriteVariable("emailSender", this.currentSender.DisplayName);
			}
			else
			{
				base.SetRecordedName("emailSender", this.currentSender.ADOrgPerson);
			}
			base.WriteVariable("firstMessage", this.readAsFirstMessage);
			base.WriteVariable("read", this.readingSavedMessages);
			ReplyForwardType replyForwardType = GlobCfg.SubjectToReplyForwardType(currentMessage.Subject);
			base.WriteVariable("isReply", replyForwardType == ReplyForwardType.Reply);
			base.WriteVariable("isForward", replyForwardType == ReplyForwardType.Forward);
			base.WriteVariable("urgent", Importance.High == currentMessage.Importance);
			this.currentMessageImportance = currentMessage.Importance;
			base.WriteVariable("protected", currentMessage.IsRestricted);
			base.WriteReplyIntroType(IntroType.None);
			base.CallSession.IncrementCounter(SubscriberAccessCounters.VoiceMessagesHeard);
			if (currentMessage.IsRestricted)
			{
				base.CallSession.IncrementCounter(SubscriberAccessCounters.ProtectedVoiceMessagesHeard);
			}
		}

		private string GetPriorityOfMessage()
		{
			if (Importance.High != this.currentMessageImportance)
			{
				return null;
			}
			return "isHighPriority";
		}

		private void SaveMessage()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, "Attempting to save message.", new object[0]);
			using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.caller.CreateSessionLock())
			{
				using (MessageItem messageItem = MessageItem.Bind(mailboxSessionLock.Session, this.voiceMessageList.CurrentMessageBeingRead))
				{
					if (!messageItem.IsRead)
					{
						messageItem.OpenAsReadWrite();
						messageItem.MarkAsRead(false, true);
						ConflictResolutionResult conflictResolutionResult = messageItem.Save(SaveMode.ResolveConflicts);
						if (conflictResolutionResult.SaveStatus == SaveResult.IrresolvableConflict)
						{
							throw new SaveConflictException(ServerStrings.ExSaveFailedBecauseOfConflicts(this.voiceMessageList.CurrentMessageBeingRead), conflictResolutionResult);
						}
					}
				}
			}
		}

		private void MarkUnread()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, "Attempting to mark message as unread.", new object[0]);
			using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.caller.CreateSessionLock())
			{
				using (MessageItem messageItem = MessageItem.Bind(mailboxSessionLock.Session, this.voiceMessageList.CurrentMessageBeingRead))
				{
					if (messageItem.IsRead)
					{
						messageItem.OpenAsReadWrite();
						messageItem.MarkAsUnread(true);
						ConflictResolutionResult conflictResolutionResult = messageItem.Save(SaveMode.ResolveConflicts);
						if (conflictResolutionResult.SaveStatus == SaveResult.IrresolvableConflict)
						{
							throw new SaveConflictException(ServerStrings.ExSaveFailedBecauseOfConflicts(this.voiceMessageList.CurrentMessageBeingRead), conflictResolutionResult);
						}
					}
				}
			}
		}

		private void FlagMessage()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, "Attempting to flag message.", new object[0]);
			using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.caller.CreateSessionLock())
			{
				using (MessageItem messageItem = MessageItem.Bind(mailboxSessionLock.Session, this.voiceMessageList.CurrentMessageBeingRead))
				{
					messageItem.OpenAsReadWrite();
					messageItem.SetFlag(Strings.FollowUp.ToString(this.caller.TelephonyCulture), null, null);
					ConflictResolutionResult conflictResolutionResult = messageItem.Save(SaveMode.ResolveConflicts);
					if (conflictResolutionResult.SaveStatus == SaveResult.IrresolvableConflict)
					{
						throw new SaveConflictException(ServerStrings.ExSaveFailedBecauseOfConflicts(this.voiceMessageList.CurrentMessageBeingRead), conflictResolutionResult);
					}
				}
			}
		}

		private void GetEnvelopInformation()
		{
			using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.caller.CreateSessionLock())
			{
				using (MessageItem messageItem = MessageItem.Bind(mailboxSessionLock.Session, this.voiceMessageList.CurrentMessageBeingRead, RetrieveVoicemailManager.voiceMessageProperties))
				{
					int num = (int)XsoUtil.SafeGetProperty(messageItem, MessageItemSchema.VoiceMessageDuration, 0);
					base.WriteVariable("durationMinutes", num / 60);
					base.WriteVariable("durationSeconds", num % 60);
					base.WriteVariable("isHighPriority", Importance.High == messageItem.Importance);
					base.WriteVariable("isProtected", messageItem.IsRestricted);
					string text = (string)XsoUtil.SafeGetProperty(messageItem, MessageItemSchema.VoiceMessageSenderName, null);
					if (string.IsNullOrEmpty(text))
					{
						PhoneNumber senderTelephoneNumber = XsoUtil.GetSenderTelephoneNumber(messageItem);
						base.WriteVariable("senderCallerID", senderTelephoneNumber);
						this.SpecifiedCallerDetails.CallerId = senderTelephoneNumber;
					}
					base.WriteVariable("senderInfo", text);
					this.SpecifiedCallerDetails.CallerName = text;
				}
			}
		}

		private void DeleteCurrentMessage()
		{
			this.CommitPendingDeletion();
			if (this.numDeletions < 2U && (this.numDeletions += 1U) >= 2U)
			{
				base.WriteVariable("playedUndelete", true);
			}
			this.pendingDeletionObjectId = this.voiceMessageList.CurrentMessageBeingRead;
			this.voiceMessageList.DeleteMessage(this.pendingDeletionObjectId);
			base.WriteVariable("canUndelete", true);
			base.CallSession.IncrementCounter(SubscriberAccessCounters.VoiceMessagesDeleted);
		}

		private void UndeleteCurrentMessage()
		{
			this.voiceMessageList.UnDeleteMessage(this.pendingDeletionObjectId);
			this.pendingDeletionObjectId = null;
			base.WriteVariable("canUndelete", false);
			base.CallSession.DecrementCounter(SubscriberAccessCounters.VoiceMessagesDeleted);
		}

		private void FindByName()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, "VmailManager::FindByName.", new object[0]);
			UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = null;
			OneTimeSearch oneTimeSearch = null;
			int num = 0;
			try
			{
				ContactSearchItem contactSearchItem = (ContactSearchItem)this.ReadVariable("directorySearchResult");
				if (contactSearchItem == null)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, "VmailManager::FindByName received a null search result!", new object[0]);
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
					QueryFilter queryFilter = Filters.CreateVoicemailFindByNameFilter(contactSearchItem, this.caller.MessageSubmissionCulture);
					if (queryFilter != null)
					{
						StoreId fid = this.voicemailSearchFolderId;
						mailboxSessionLock = this.caller.CreateSessionLock();
						oneTimeSearch = OneTimeSearch.Execute(this.caller, mailboxSessionLock.Session, fid, queryFilter);
						num = oneTimeSearch.ItemCount;
						base.WriteVariable("numMessagesFromName", num);
						if (num > 0)
						{
							base.WriteVariable("inFindMode", true);
							this.readAsFirstMessage = true;
							this.haveReadSavedMessages = false;
							if (this.pendingDeletionObjectId != null)
							{
								this.CommitPendingDeletion();
							}
							base.WriteVariable("canUndelete", false);
							if (this.findByNameResults != null)
							{
								this.findByNameResults.Dispose();
							}
							this.findByNameResults = oneTimeSearch;
							oneTimeSearch = null;
							this.voiceMessageList.InitializeCurrentPagerList(new MessageItemList(this.caller, this.findByNameResults.FolderId, MessageItemListSortType.LifoVoicemail, RetrieveVoicemailManager.viewProperties));
						}
					}
					else
					{
						PIIMessage data = PIIMessage.Create(PIIType._PII, this.caller.ADRecipient.DistinguishedName);
						CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, data, "Unable to generate the search filter for Contact or Gal user with id {0} because neither displayname nor email properties are populated. Caller _PII. ", new object[]
						{
							contactSearchItem.Id ?? "not available"
						});
						base.WriteVariable("numMessagesFromName", 0);
					}
				}
			}
			finally
			{
				if (mailboxSessionLock != null)
				{
					mailboxSessionLock.Dispose();
					mailboxSessionLock = null;
				}
				if (num == 0)
				{
					base.WriteVariable("inFindMode", false);
				}
				if (oneTimeSearch != null)
				{
					oneTimeSearch.Dispose();
					oneTimeSearch = null;
				}
			}
		}

		private void CommitPendingDeletion()
		{
			if (this.caller != null && this.pendingDeletionObjectId != null)
			{
				using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.caller.CreateSessionLock())
				{
					mailboxSessionLock.Session.Delete(DeleteItemFlags.MoveToDeletedItems, new StoreId[]
					{
						this.pendingDeletionObjectId
					});
				}
			}
		}

		private static PropertyDefinition[] viewProperties = new PropertyDefinition[]
		{
			MessageItemSchema.IsRead
		};

		private static PropertyDefinition[] voiceMessageProperties = new PropertyDefinition[]
		{
			ItemSchema.ReceivedTime,
			ItemSchema.Importance,
			MessageItemSchema.SenderTelephoneNumber,
			MessageItemSchema.VoiceMessageSenderName,
			MessageItemSchema.VoiceMessageDuration
		};

		private VoiceMessageList voiceMessageList = new VoiceMessageList();

		private ContactInfo currentSender;

		private uint numDeletions;

		private UMSubscriber caller;

		private OneTimeSearch findByNameResults;

		private StoreObjectId voicemailSearchFolderId;

		private Importance currentMessageImportance;

		private bool readAsFirstMessage;

		private bool readingSavedMessages;

		private bool haveReadSavedMessages;

		private StoreObjectId pendingDeletionObjectId;

		private bool drmIsEnabled;

		internal class ConfigClass : ActivityManagerConfig
		{
			public ConfigClass(ActivityManagerConfig manager) : base(manager)
			{
			}

			internal override ActivityManager CreateActivityManager(ActivityManager manager)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Constructing Retrieve Voicemail activity manager.", new object[0]);
				return new RetrieveVoicemailManager(manager, this);
			}
		}
	}
}
