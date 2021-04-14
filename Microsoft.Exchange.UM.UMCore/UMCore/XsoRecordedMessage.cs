using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class XsoRecordedMessage : IRecordedMessage
	{
		internal XsoRecordedMessage(BaseUMCallSession vo, UMSubscriber user, ActivityManager manager, bool autoPostSubmit)
		{
			this.session = vo;
			this.user = user;
			this.manager = manager;
			this.autoPostSubmit = autoPostSubmit;
			using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = user.CreateSessionLock())
			{
				if (XsoUtil.IsOverSendQuota(mailboxSessionLock.Session.Mailbox, 0UL))
				{
					throw new QuotaExceededException(LocalizedString.Empty);
				}
			}
		}

		internal XsoRecordedMessage(BaseUMCallSession vo, UMSubscriber user, ActivityManager manager) : this(vo, user, manager, true)
		{
		}

		protected virtual bool IsPureVoiceMessage
		{
			get
			{
				return false;
			}
		}

		protected virtual bool IsReplyToAProtectedMessage
		{
			get
			{
				return false;
			}
		}

		protected BaseUMCallSession Session
		{
			get
			{
				return this.session;
			}
		}

		protected ActivityManager Manager
		{
			get
			{
				return this.manager;
			}
		}

		protected UMSubscriber User
		{
			get
			{
				return this.user;
			}
		}

		protected bool AutoPostSubmit
		{
			get
			{
				return this.autoPostSubmit;
			}
		}

		protected string AttachName
		{
			get
			{
				return this.attachName;
			}
		}

		public void DoSubmit(Importance importance)
		{
			this.DoSubmit(importance, false, null);
		}

		public virtual void DoSubmit(Importance importance, bool markPrivate, Stack<Participant> recips)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "RecordedMessage::Submit.", new object[0]);
			ExAssert.RetailAssert(!markPrivate || (markPrivate && this.IsPureVoiceMessage), "Only Pure voice messages can be marked private");
			RecordContext recordContext = this.Manager.RecordContext;
			this.DetermineIfMessageNeedsToBeProtected(markPrivate);
			using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.user.CreateSessionLock())
			{
				using (MessageItem messageItem = this.GenerateNewMessage(mailboxSessionLock.Session))
				{
					int num = 10000;
					if (recips != null)
					{
						while (recips.Count > 0 && --num > 0)
						{
							Participant participant = recips.Pop();
							messageItem.Recipients.Add(participant, RecipientItemType.To);
						}
					}
					messageItem.ExpandRecipientTable();
					messageItem[MessageItemSchema.VoiceMessageDuration] = recordContext.TotalSeconds;
					XsoUtil.SetSubscriberAccessSenderProperties(messageItem, this.User);
					messageItem.Importance = importance;
					if (markPrivate)
					{
						messageItem.Sensitivity = Sensitivity.Private;
					}
					string waveFilePath = null;
					if (recordContext.Recording != null && recordContext.TotalSeconds > 0)
					{
						waveFilePath = recordContext.Recording.FilePath;
					}
					UMMessageSubmission.SubmitXSOVoiceMail(this.session.CallId, this.session.CurrentCallContext.CallerId, this.User, waveFilePath, recordContext.TotalSeconds, this.User.DialPlan.AudioCodec, this.AttachName, this.User.TelephonyCulture, messageItem, null, this.session.CurrentCallContext.CallerIdDisplayName, this.session.CurrentCallContext.TenantGuid);
					this.Session.SetCounter(SubscriberAccessCounters.AverageSentVoiceMessageSize, XsoRecordedMessage.sizeAverage.Update((long)recordContext.TotalSeconds));
					this.Session.SetCounter(SubscriberAccessCounters.AverageRecentSentVoiceMessageSize, XsoRecordedMessage.sizeMovingAverage.Update((long)recordContext.TotalSeconds));
					if (this.AutoPostSubmit)
					{
						this.DoPostSubmit();
					}
				}
			}
		}

		public virtual void DoPostSubmit()
		{
			this.Manager.RecordContext.Reset();
			this.Session.IncrementCounter(SubscriberAccessCounters.VoiceMessagesSent);
		}

		protected abstract MessageItem GenerateMessage(MailboxSession session);

		protected virtual MessageItem GenerateProtectedMessage(MailboxSession session)
		{
			throw new InvalidOperationException();
		}

		protected virtual void AddRecordedMessageText(MessageContentBuilder content)
		{
		}

		protected virtual void AddMessageHeader(Item originalMessage, MessageContentBuilder content)
		{
		}

		protected void SetAttachmentName(AttachmentCollection attachmentCollection)
		{
			this.attachName = Util.BuildAttachmentName(this.User.Extension, this.Manager.RecordContext.TotalSeconds, attachmentCollection, this.User.TelephonyCulture, this.User.DialPlan.AudioCodec, this.messageIsToBeProtected);
		}

		protected string PrepareMessageBodyPrefix(Item originalMessage)
		{
			MessageContentBuilder messageContentBuilder = MessageContentBuilder.Create(this.User.TelephonyCulture);
			if (!string.IsNullOrEmpty(this.AttachName))
			{
				this.AddRecordedMessageText(messageContentBuilder);
			}
			this.AddMessageHeader(originalMessage, messageContentBuilder);
			return messageContentBuilder.ToString();
		}

		private MessageItem GenerateNewMessage(MailboxSession session)
		{
			if (this.messageIsToBeProtected)
			{
				return this.GenerateProtectedMessage(session);
			}
			return this.GenerateMessage(session);
		}

		private void DetermineIfMessageNeedsToBeProtected(bool markPrivate)
		{
			if (this.IsReplyToAProtectedMessage)
			{
				this.messageIsToBeProtected = true;
				return;
			}
			if (!this.IsPureVoiceMessage)
			{
				this.messageIsToBeProtected = false;
				return;
			}
			if ((markPrivate && this.User.DRMPolicyForInterpersonal == DRMProtectionOptions.Private) || this.User.DRMPolicyForInterpersonal == DRMProtectionOptions.All)
			{
				this.messageIsToBeProtected = true;
				return;
			}
			this.messageIsToBeProtected = false;
		}

		private static MovingAverage sizeMovingAverage = new MovingAverage(50);

		private static Average sizeAverage = new Average();

		private BaseUMCallSession session;

		private ActivityManager manager;

		private UMSubscriber user;

		private bool autoPostSubmit;

		private string attachName;

		private bool messageIsToBeProtected;
	}
}
