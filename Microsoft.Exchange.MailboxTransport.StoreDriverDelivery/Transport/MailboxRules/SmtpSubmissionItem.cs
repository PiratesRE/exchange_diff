using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Mime.Internal;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.MailboxRules;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxTransport.Shared.SubmissionItem;
using Microsoft.Exchange.MailboxTransport.StoreDriverCommon;
using Microsoft.Exchange.MailboxTransport.StoreDriverDelivery;
using Microsoft.Exchange.MessageSecurity.MessageClassifications;
using Microsoft.Exchange.Transport.Logging.MessageTracking;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Transport.MailboxRules
{
	internal class SmtpSubmissionItem : SubmissionItemBase, ISubmissionItem, IDisposable
	{
		public SmtpSubmissionItem(RuleEvaluationContext context, MessageItem item, WorkItem workItem) : base("Microsoft SMTP Server")
		{
			base.Session = context.StoreSession;
			base.Item = item;
			this.context = context;
			this.workItem = workItem;
			this.submissionTime = DateTime.UtcNow;
		}

		public override string SourceServerFqdn
		{
			get
			{
				return this.context.LocalServerFqdn;
			}
		}

		public override IPAddress SourceServerNetworkAddress
		{
			get
			{
				return this.context.LocalServerNetworkAddress;
			}
		}

		public override DateTime OriginalCreateTime
		{
			get
			{
				return this.submissionTime;
			}
		}

		public void Submit()
		{
			TransportMailItem transportMailItem = this.CreateMailItem();
			SubmissionItemUtils.CopySenderTo(this, transportMailItem);
			List<string> invalidRecipients = null;
			List<string> list = null;
			SubmissionItemUtils.CopyRecipientsTo(this, transportMailItem, new SubmissionRecipientHandler(this.RecipientHandler), ref invalidRecipients, ref list);
			this.ValidateRecipientCollections(transportMailItem, invalidRecipients);
			this.SubmitMailItem(transportMailItem, invalidRecipients);
		}

		public void Submit(ProxyAddress sender, IEnumerable<Participant> recipients)
		{
			TransportMailItem mailItem = this.CreateMailItem();
			this.SetEnvelopeSender(mailItem, sender);
			IList<string> invalidRecipients = null;
			this.SetEnvelopeRecipients(mailItem, recipients, ref invalidRecipients);
			this.ValidateRecipientCollections(mailItem, invalidRecipients);
			this.SubmitMailItem(mailItem, invalidRecipients);
		}

		private TransportMailItem CreateMailItem()
		{
			TransportMailItem transportMailItem = TransportMailItem.NewSideEffectMailItem(this.context.MbxTransportMailItem, this.context.RecipientCache.OrganizationId, LatencyComponent.MailboxRules, MailDirectionality.Originating, this.context.MbxTransportMailItem.ExternalOrganizationId);
			base.CopyContentTo(transportMailItem);
			base.DecorateMessage(transportMailItem);
			base.ApplySecurityAttributesTo(transportMailItem);
			transportMailItem.PrioritizationReason = this.context.PrioritizationReason;
			transportMailItem.Priority = this.context.Priority;
			ClassificationUtils.PromoteStoreClassifications(transportMailItem.RootPart.Headers);
			SubmissionItemUtils.PatchQuarantineSender(transportMailItem, base.QuarantineOriginalSender);
			HeaderList headers = transportMailItem.RootPart.Headers;
			if (headers.FindFirst(HeaderId.MessageId) == null)
			{
				headers.AppendChild(new AsciiTextHeader("Message-Id", string.Concat(new string[]
				{
					"<",
					Guid.NewGuid().ToString("N"),
					"@",
					this.SourceServerFqdn,
					">"
				})));
			}
			MimeInternalHelpers.CopyHeaderBetweenList(this.context.RootPart.Headers, headers, "X-MS-Exchange-Moderation-Loop");
			transportMailItem.UpdateCachedHeaders();
			return transportMailItem;
		}

		private void SubmitMailItem(TransportMailItem mailItem, IList<string> invalidRecipients)
		{
			this.SetGeneratedByRulePropertyIfNecessary(mailItem);
			this.MessageTrackMailItem(mailItem, invalidRecipients);
			if (this.context.ProcessingTestMessage)
			{
				this.context.TraceDebug("SubmitMailItem: Do not enqueue rule generated message from diagnostic test message");
				return;
			}
			if (mailItem.Recipients.Count == 0)
			{
				this.context.TraceDebug("SubmitMailItem: Skipping submission since there are no recipients");
				return;
			}
			for (int i = 1; i <= 3; i++)
			{
				try
				{
					this.context.Server.SubmitMailItem(mailItem, false);
					break;
				}
				catch (StoreDriverAgentTransientException ex)
				{
					this.context.TraceDebug(string.Format(CultureInfo.InvariantCulture, "SubmitMailItem: Encountered StoreDriverAgentTransientException on attempt {0} of {1}: {2}", new object[]
					{
						i,
						3,
						ex
					}));
				}
			}
		}

		private void MessageTrackMailItem(TransportMailItem mailItem, IList<string> invalidRecipients)
		{
			MsgTrackReceiveInfo msgTrackInfo = new MsgTrackReceiveInfo(StoreDriverDelivery.LocalIPAddress, ((ulong)this.workItem.Rule.ID).ToString(), this.context.Message.InternetMessageId, mailItem.MessageTrackingSecurityInfo, invalidRecipients);
			if (mailItem.Recipients.Count == 0)
			{
				MessageTrackingLog.TrackReceive(MessageTrackingSource.MAILBOXRULE, mailItem, msgTrackInfo);
				this.context.TraceDebug("SmtpSubmissionItem: Rule generated mail without valid recipients");
				return;
			}
			if (this.context.ProcessingTestMessage)
			{
				this.context.TraceDebug("SmtpSubmissionItem: Do not commit rule generated message from diagnostic test message");
				return;
			}
			MessageTrackingLog.TrackReceive(MessageTrackingSource.MAILBOXRULE, mailItem, msgTrackInfo);
			this.context.TraceDebug<long>("SmtpSubmissionItem committed new message {0}", mailItem.RecordId);
		}

		private void SetEnvelopeSender(TransportMailItem mailItem, ProxyAddress sender)
		{
			RoutingAddress routingAddress;
			if (!SubmissionItemUtils.TryGetRoutingAddress(mailItem.ADRecipientCache, sender.AddressString, sender.PrefixString, "SmtpSubmissionItem.SetEnvelopeSender", out routingAddress))
			{
				this.context.TraceError<string>("SmtpSubmissionItem.SetEnvelopeSender: Could not get routing address for sender {0}.", sender.ToString());
				throw new InvalidSenderException(sender);
			}
			this.context.TraceDebug<RoutingAddress>("SmtpSubmissionItem.SetEnvelopeSender: Setting envelope sender: {0}", routingAddress);
			mailItem.From = routingAddress;
			mailItem.MimeSender = routingAddress;
		}

		private void SetEnvelopeRecipients(TransportMailItem mailItem, IEnumerable<Participant> recipients, ref IList<string> invalidAddresses)
		{
			this.context.TraceDebug<long>("SmtpSubmissionItem.SetEnvelopeRecipients: Setting envelope recipients on mailitem {0}", mailItem.RecordId);
			foreach (Participant participant in recipients)
			{
				RoutingAddress argument;
				if (participant == null)
				{
					this.context.TraceError("SmtpSubmissionItem.SetEnvelopeRecipients: Null participant found in recipient list.");
				}
				else if (SubmissionItemUtils.TryGetRoutingAddress(mailItem.ADRecipientCache, participant.EmailAddress, participant.RoutingType, "SmtpSubmissionItem.SetEnvelopeRecipients", out argument))
				{
					this.context.TraceDebug<RoutingAddress>("SmtpSubmissionItem.SetEnvelopeRecipients: Added envelope recipient: {0}", argument);
					mailItem.Recipients.Add(argument.ToString());
				}
				else
				{
					if (invalidAddresses == null)
					{
						invalidAddresses = new List<string>(1);
					}
					string item = SubmissionItemUtils.BuildParticipantString(participant);
					invalidAddresses.Add(item);
					this.context.TraceError<string, string, string>("SmtpSubmissionItem.SetEnvelopeRecipients: Could not get routing address for \"{0}\" / \"{1}:{2}\"", participant.DisplayName, participant.RoutingType, participant.EmailAddress);
				}
			}
		}

		private void ValidateRecipientCollections(TransportMailItem mailItem, IList<string> invalidRecipients)
		{
			if (this.IsGeneratingReply())
			{
				return;
			}
			if ((invalidRecipients != null && invalidRecipients.Count > 0) || mailItem.Recipients.Count == 0)
			{
				Rule rule = this.workItem.Rule;
				RuleAction.Type actionType = this.workItem.Rule.Actions[this.workItem.ActionIndex].ActionType;
				int actionIndex = this.workItem.ActionIndex;
				this.context.MarkRuleInError(rule, actionType, actionIndex, DeferredError.RuleError.Execution);
			}
		}

		private bool IsGeneratingReply()
		{
			Rule rule = this.workItem.Rule;
			int actionIndex = this.workItem.ActionIndex;
			RuleAction ruleAction = rule.Actions[actionIndex];
			RuleAction.Type actionType = ruleAction.ActionType;
			return actionType == RuleAction.Type.OP_REPLY || actionType == RuleAction.Type.OP_OOF_REPLY || actionType == RuleAction.Type.OP_DELETE;
		}

		private void RecipientHandler(int? recipientType, Recipient recipient, TransportMailItem mailItem, MailRecipient mailRecipient)
		{
			if (mailRecipient == null)
			{
				Participant participant = recipient.Participant;
				this.context.TraceError<string, string, string>("SmtpSubmissionItem.RecipientHandler: Could not get routing address for \"{0}\" / \"{1}:{2}\"", participant.DisplayName, participant.RoutingType, participant.EmailAddress);
			}
		}

		private void SetGeneratedByRulePropertyIfNecessary(TransportMailItem mailItem)
		{
			if (OrganizationId.ForestWideOrgId.Equals(mailItem.OrganizationId))
			{
				this.context.TraceDebug<string>("Skip setting {0} property because we are in Enterprise environment.", "Microsoft.Exchange.Transport.GeneratedByMailboxRule");
				return;
			}
			if (mailItem.Recipients.Count == 0)
			{
				this.context.TraceDebug<string, string>("Skip setting {0} property because there were no recipients on the generated message. RuleName={1}", "Microsoft.Exchange.Transport.GeneratedByMailboxRule", this.context.CurrentRule.Name);
				return;
			}
			string text = string.Format(CultureInfo.InvariantCulture, "{0}:{1}", new object[]
			{
				this.context.Recipient.AddressString,
				this.context.CurrentRule.Name
			});
			this.context.TraceDebug<string, string>("Set {0} property to value {1}", "Microsoft.Exchange.Transport.GeneratedByMailboxRule", text);
			mailItem.ExtendedProperties.SetValue<string>("Microsoft.Exchange.Transport.GeneratedByMailboxRule", text);
		}

		private const int RetryCount = 3;

		private readonly DateTime submissionTime;

		private RuleEvaluationContext context;

		private WorkItem workItem;
	}
}
