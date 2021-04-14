using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage.MailboxRules
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ForwardWorkItem : WorkItem
	{
		public ForwardWorkItem(IRuleEvaluationContext context, AdrEntry[] recipients, RuleAction.Forward.ActionFlags flags, int actionIndex) : base(context, actionIndex)
		{
			if (RuleUtil.IsNullOrEmpty(recipients))
			{
				throw new ArgumentException("Forward recipient list is empty");
			}
			this.flags = flags;
			this.recipients = recipients;
		}

		public override bool ShouldExecuteOnThisStage
		{
			get
			{
				return base.ShouldExecuteOnThisStage || ExecutionStage.OnPublicFolderAfter == base.Context.ExecutionStage;
			}
		}

		public override ExecutionStage Stage
		{
			get
			{
				return ExecutionStage.OnPromotedMessage | ExecutionStage.OnDeliveredMessage | ExecutionStage.OnPublicFolderAfter;
			}
		}

		private bool IsForwarding
		{
			get
			{
				return (this.flags & RuleAction.Forward.ActionFlags.PreserveSender) == RuleAction.Forward.ActionFlags.None;
			}
		}

		public override void Execute()
		{
			if (!this.ShouldExecuteOnThisStage)
			{
				return;
			}
			if (base.Context.DetectLoop())
			{
				base.Context.TraceDebug("Forward action: Loop detected, message will not be forwarded.");
				return;
			}
			string argument = this.IsForwarding ? "Forward" : "Redirect";
			base.Context.TraceDebug<string>("{0} action: Creating message.", argument);
			using (MessageItem messageItem = this.CreateMessage())
			{
				messageItem[ItemSchema.IsAutoForwarded] = true;
				base.Context.CopyProperty(base.Context.Message, messageItem, ItemSchema.SpamConfidenceLevel);
				IList<ProxyAddress> senderProxyAddresses = base.GetSenderProxyAddresses();
				if (this.IsForwarding)
				{
					base.Context.SetMailboxOwnerAsSender(messageItem);
					this.SetFromForMeetingMessages(messageItem);
					if (RuleUtil.SetRecipients(base.Context, messageItem, senderProxyAddresses, this.recipients, true))
					{
						base.Context.TraceDebug<string>("{0} action: Submitting message.", argument);
						base.SubmitMessage(messageItem);
						base.Context.TraceDebug<string>("{0} action: Message submitted.", argument);
					}
					else
					{
						base.Context.TraceDebug<string>("{0} action: There are no recipients.", argument);
					}
				}
				else
				{
					messageItem.From = base.Context.Message.From;
					MailboxSession mailboxSession = base.Context.StoreSession as MailboxSession;
					PublicFolderSession publicFolderSession = base.Context.StoreSession as PublicFolderSession;
					string value = string.Empty;
					if (mailboxSession != null)
					{
						value = mailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString();
					}
					else if (publicFolderSession != null)
					{
						value = publicFolderSession.MailboxPrincipal.MailboxInfo.PrimarySmtpAddress.ToString();
					}
					if (!string.IsNullOrEmpty(value))
					{
						messageItem[ItemSchema.ResentFrom] = value;
					}
					RuleUtil.SetRecipients(base.Context, messageItem, null, base.Context.Message.Recipients, false);
					if (publicFolderSession != null)
					{
						messageItem.MarkAllRecipientAsSubmitted();
					}
					IEnumerable<Participant> enumerable = from recipient in this.recipients
					select RuleUtil.ParticipantFromAddressEntry(recipient);
					string text = base.Context.Message.TryGetProperty(ItemSchema.ResentFrom) as string;
					IList<Participant> list = new List<Participant>();
					foreach (Participant participant in enumerable)
					{
						if (RuleUtil.IsRecipientSameAsSender(senderProxyAddresses, participant.EmailAddress) || (!string.IsNullOrEmpty(text) && string.Equals(text, participant.EmailAddress, StringComparison.OrdinalIgnoreCase)))
						{
							base.Context.TraceDebug<string>("Skipping redirectee {0} because that was the original sender or resent-from user.", participant.EmailAddress);
						}
						else
						{
							list.Add(participant);
						}
					}
					if (list.Count > 0)
					{
						base.Context.TraceDebug<string>("{0} action: Submitting message.", argument);
						base.SubmitMessage(messageItem, base.Context.Recipient, enumerable);
						base.Context.TraceDebug<string>("{0} action: Message submitted.", argument);
					}
					else
					{
						base.Context.TraceDebug<string>("{0} action: There are no recipients.", argument);
					}
				}
			}
		}

		private MessageItem CreateMessage()
		{
			if ((this.flags & RuleAction.Forward.ActionFlags.SendSmsAlert) != RuleAction.Forward.ActionFlags.None)
			{
				return RuleMessageUtils.CreateSmsAlert(base.Context.Message, base.Context.StoreSession.PreferedCulture, base.Context.DefaultDomainName, base.Context.XLoopValue, base.Context.DetermineRecipientTimeZone(), base.Context);
			}
			if (this.IsForwarding)
			{
				bool asAttachment = (this.flags & RuleAction.Forward.ActionFlags.ForwardAsAttachment) != RuleAction.Forward.ActionFlags.None;
				return RuleMessageUtils.CreateForward(base.Context.Message, asAttachment, base.Context.StoreSession.PreferedCulture, base.Context.DefaultDomainName, base.Context.XLoopValue, base.Context.DetermineRecipientTimeZone(), base.Context);
			}
			return RuleMessageUtils.CreateRedirect(base.Context.Message, base.Context.StoreSession.PreferedCulture, base.Context.DefaultDomainName, base.Context.XLoopValue, base.Context);
		}

		private void SetFromForMeetingMessages(MessageItem newMessage)
		{
			MessageItem message = base.Context.Message;
			if (message != null && (ObjectClass.IsMeetingMessage(message.ClassName) || ObjectClass.IsCalendarItemCalendarItemOccurrenceOrRecurrenceException(message.ClassName)))
			{
				newMessage.From = message.From;
			}
		}

		private RuleAction.Forward.ActionFlags flags;

		private AdrEntry[] recipients;
	}
}
