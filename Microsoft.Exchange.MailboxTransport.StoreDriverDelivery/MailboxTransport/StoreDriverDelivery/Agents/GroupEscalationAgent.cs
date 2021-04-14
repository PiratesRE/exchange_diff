using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.GroupMailbox.Escalation;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Data.Transport.StoreDriver;
using Microsoft.Exchange.Data.Transport.StoreDriverDelivery;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.StoreDriverDelivery;
using Microsoft.Exchange.MailboxTransport.StoreDriver.Configuration;
using Microsoft.Exchange.MailboxTransport.StoreDriverCommon;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	internal class GroupEscalationAgent : StoreDriverDeliveryAgent
	{
		public GroupEscalationAgent(ProcessedMessageTracker processedMessages)
		{
			this.processedMessages = processedMessages;
			base.OnCreatedMessage += this.OnCreatedMessageHandler;
			base.OnDeliveredMessage += this.OnDeliveredMessageHandler;
			base.OnPromotedMessage += this.OnPromotedMessageHandler;
		}

		public void OnDeliveredMessageHandler(StoreDriverEventSource source, StoreDriverDeliveryEventArgs args)
		{
			StoreDriverDeliveryEventArgsImpl storeDriverDeliveryEventArgsImpl = (StoreDriverDeliveryEventArgsImpl)args;
			MailboxSession mailboxSession = storeDriverDeliveryEventArgsImpl.MailboxSession;
			if (mailboxSession == null || !mailboxSession.IsGroupMailbox())
			{
				GroupEscalationAgent.Tracer.TraceDebug((long)this.GetHashCode(), "GroupEscalationAgent.OnDeliveredMessageHandler: Session null or not a group mailbox.");
				return;
			}
			MessageItem messageItem = storeDriverDeliveryEventArgsImpl.MessageItem;
			messageItem.Load(new PropertyDefinition[]
			{
				ItemSchema.InternetMessageId,
				ItemSchema.SentTime
			});
			this.processedMessages.ClearMessageFromProcessedList(messageItem.InternetMessageId, messageItem.SentTime, mailboxSession.MailboxGuid);
		}

		public void OnCreatedMessageHandler(StoreDriverEventSource source, StoreDriverDeliveryEventArgs args)
		{
			StoreDriverDeliveryEventArgsImpl storeDriverDeliveryEventArgsImpl = (StoreDriverDeliveryEventArgsImpl)args;
			MessageItem messageItem = storeDriverDeliveryEventArgsImpl.MessageItem;
			MbxTransportMailItem mbxTransportMailItem = storeDriverDeliveryEventArgsImpl.MailItemDeliver.MbxTransportMailItem;
			if (messageItem == null || mbxTransportMailItem == null)
			{
				GroupEscalationAgent.Tracer.TraceError((long)this.GetHashCode(), "No message to process");
				return;
			}
			MailboxSession mailboxSession = messageItem.Session as MailboxSession;
			if (GroupEscalationAgent.ShouldGenerateIrmNdr(mailboxSession, messageItem, storeDriverDeliveryEventArgsImpl.MailboxOwner))
			{
				GroupEscalationAgent.Tracer.TraceDebug((long)this.GetHashCode(), "GroupEscalationAgent.ShouldProcessMessage: NDR the message since it's irm message sent to group");
				throw new SmtpResponseException(GroupEscalationAgent.IrmMessageDeliveryToGroupMailBoxError);
			}
			if (!this.ShouldProcessMessage(mailboxSession, storeDriverDeliveryEventArgsImpl))
			{
				return;
			}
			IGroupEscalationFlightInfo groupEscalationFlightInfo = new GroupEscalationFlightInfo(storeDriverDeliveryEventArgsImpl.MailboxOwner.GetContext(null));
			TransportGroupEscalation transportGroupEscalation = new TransportGroupEscalation(mbxTransportMailItem, XSOFactory.Default, groupEscalationFlightInfo, new MailboxUrls(mailboxSession.MailboxOwner, false));
			bool flag;
			if (transportGroupEscalation.EscalateItem(messageItem, mailboxSession, out flag, false))
			{
				GroupEscalationAgent.Tracer.TraceDebug<string>((long)this.GetHashCode(), "GroupEscalationAgent.EscalateItem: Escalation of message {0} from group mailbox successful", messageItem.InternetMessageId);
				this.processedMessages.AddMessageToProcessedList(messageItem.InternetMessageId, messageItem.SentTime, mailboxSession.MailboxGuid, DeliveryStage.CreatedMessageEventHandled);
				return;
			}
			GroupEscalationAgent.Tracer.TraceError<string, bool>((long)this.GetHashCode(), "GroupEscalationAgent.EscalateItem: Escalation of message {0} from group mailbox failed. IsTransientError: {1}", messageItem.InternetMessageId, flag);
			this.processedMessages.ClearMessageFromProcessedList(messageItem.InternetMessageId, messageItem.SentTime, mailboxSession.MailboxGuid);
			if (flag)
			{
				throw new SmtpResponseException(GroupEscalationAgent.EscalationFailedTransientError);
			}
			throw new SmtpResponseException(GroupEscalationAgent.EscalationFailedPermanentError);
		}

		public void OnPromotedMessageHandler(StoreDriverEventSource source, StoreDriverDeliveryEventArgs args)
		{
			if (args == null)
			{
				return;
			}
			StoreDriverDeliveryEventArgsImpl storeDriverDeliveryEventArgsImpl = (StoreDriverDeliveryEventArgsImpl)args;
			MailboxSession mailboxSession = storeDriverDeliveryEventArgsImpl.MailboxSession;
			string messageClass = storeDriverDeliveryEventArgsImpl.MessageClass;
			if (GroupEscalationAgent.ShouldBlockMessageForGroup(mailboxSession, messageClass))
			{
				storeDriverDeliveryEventArgsImpl.DeliverToFolder = mailboxSession.GetDefaultFolderId(DefaultFolderType.DeletedItems);
				GroupEscalationAgent.Tracer.TraceDebug<string>((long)this.GetHashCode(), "GroupEscalationAgent.OnPromotedMessageHandler: message is blocked. Its class is {0}", messageClass);
			}
		}

		private static bool ShouldGenerateIrmNdr(MailboxSession session, MessageItem messageItem, MiniRecipient mailboxOwner)
		{
			return session != null && session.IsGroupMailbox() && messageItem.IsRestricted && !GroupEscalationAgent.IsIrmEnabledGroup(mailboxOwner);
		}

		private static bool IsIrmEnabledGroup(MiniRecipient mailboxOwner)
		{
			return true;
		}

		private static bool IsRepairUpdateMessage(MessageItem messageItem)
		{
			messageItem.Load(new PropertyDefinition[]
			{
				MeetingMessageSchema.AppointmentAuxiliaryFlags
			});
			AppointmentAuxiliaryFlags valueOrDefault = messageItem.GetValueOrDefault<AppointmentAuxiliaryFlags>(MeetingMessageSchema.AppointmentAuxiliaryFlags);
			return (valueOrDefault & AppointmentAuxiliaryFlags.RepairUpdateMessage) != (AppointmentAuxiliaryFlags)0;
		}

		private static bool IsEHAMigrationMeetingMessage(DeliverableMailItem messageItem)
		{
			return messageItem.Message.MimeDocument.RootPart.Headers.FindFirst(UnJournalAgent.UnjournalHeaders.MessageOriginalDate) != null;
		}

		private static bool ShouldBlockMessageForGroup(MailboxSession session, string messageClass)
		{
			return session != null && session.IsGroupMailbox() && GroupEscalationAgent.IsOofOrDsnMessage(messageClass);
		}

		private static bool IsOofOrDsnMessage(string messageClass)
		{
			return ObjectClass.IsOfClass(messageClass, "IPM.Note.Rules.OofTemplate.Microsoft") || ObjectClass.IsDsn(messageClass);
		}

		private bool ShouldProcessMessage(MailboxSession session, StoreDriverDeliveryEventArgsImpl argsImpl)
		{
			MessageItem messageItem = argsImpl.MessageItem;
			if (!StoreDriverConfig.Instance.IsGroupEscalationAgentEnabled)
			{
				GroupEscalationAgent.Tracer.TraceDebug((long)this.GetHashCode(), "GroupEscalationAgent.ShouldProcessMessage: skipping group message escalation as the feature is disabled via app config.");
				return false;
			}
			if (!GroupEscalation.IsEscalationEnabled())
			{
				GroupEscalationAgent.Tracer.TraceDebug((long)this.GetHashCode(), "GroupEscalationAgent.ShouldProcessMessage: skipping group message escalation as the feature is disabled.");
				return false;
			}
			if (session == null || !session.IsGroupMailbox())
			{
				GroupEscalationAgent.Tracer.TraceDebug((long)this.GetHashCode(), "GroupEscalationAgent.ShouldProcessMessage: skipping group message escalation as the session is not for a group mailbox.");
				return false;
			}
			if (this.processedMessages.IsAlreadyProcessedForStage(messageItem.InternetMessageId, messageItem.SentTime, session.MailboxGuid, DeliveryStage.CreatedMessageEventHandled))
			{
				GroupEscalationAgent.Tracer.TraceDebug((long)this.GetHashCode(), "GroupEscalationAgent.ShouldProcessMessage: skipping group message escalation as it was already processed earlier.");
				return false;
			}
			if (!ObjectClass.IsMessage(argsImpl.MessageClass, false) && !ObjectClass.IsMeetingMessage(argsImpl.MessageClass) && !ObjectClass.IsMeetingMessageSeries(argsImpl.MessageClass))
			{
				GroupEscalationAgent.Tracer.TraceDebug<string>((long)this.GetHashCode(), "GroupEscalationAgent.ShouldProcessMessage: ignoring messages that are not messages nor meeting messages. Message class {0}", argsImpl.MessageClass);
				return false;
			}
			if (ObjectClass.IsMeetingForwardNotification(argsImpl.MessageClass) || ObjectClass.IsMeetingForwardNotificationSeries(argsImpl.MessageClass))
			{
				GroupEscalationAgent.Tracer.TraceDebug((long)this.GetHashCode(), "GroupEscalationAgent.ShouldProcessMessage: item class is meeting forward notification. Do not process.");
				return false;
			}
			if (ObjectClass.IsMeetingResponse(argsImpl.MessageClass) || ObjectClass.IsMeetingResponseSeries(argsImpl.MessageClass))
			{
				GroupEscalationAgent.Tracer.TraceDebug((long)this.GetHashCode(), "GroupEscalationAgent.ShouldProcessMessage: item class is meeting response. Do not process.");
				return false;
			}
			if (GroupEscalationAgent.IsEHAMigrationMeetingMessage(argsImpl.MailItem))
			{
				GroupEscalationAgent.Tracer.TraceDebug((long)this.GetHashCode(), "GroupEscalationAgent.ShouldProcessMessage: ignoring EHA migration messages.");
				return false;
			}
			if (GroupEscalationAgent.IsRepairUpdateMessage(argsImpl.MessageItem))
			{
				GroupEscalationAgent.Tracer.TraceDebug((long)this.GetHashCode(), "GroupEscalationAgent.ShouldProcessMessage: ignoring RUM messages.");
				return false;
			}
			if (GroupEscalationAgent.IsOofOrDsnMessage(argsImpl.MessageClass))
			{
				GroupEscalationAgent.Tracer.TraceDebug((long)this.GetHashCode(), "GroupEscalationAgent.ShouldProcessMessage: ignoring OOF or DSN messages.");
				return false;
			}
			return true;
		}

		private static readonly Trace Tracer = ExTraceGlobals.GroupEscalationAgentTracer;

		private static readonly SmtpResponse IrmMessageDeliveryToGroupMailBoxError = new SmtpResponse("550", "5.7.1", new string[]
		{
			"GroupEscalationAgent; Message delivery failed due to IRM message sent to group mailbox"
		});

		private static readonly SmtpResponse EscalationFailedTransientError = new SmtpResponse("432", "4.3.2", new string[]
		{
			"GroupEscalationAgent; Escalation failed due to a transient error"
		});

		private static readonly SmtpResponse EscalationFailedPermanentError = new SmtpResponse("550", "5.7.1", new string[]
		{
			"GroupEscalationAgent; Escalation failed due to a permanent error"
		});

		private readonly ProcessedMessageTracker processedMessages;
	}
}
