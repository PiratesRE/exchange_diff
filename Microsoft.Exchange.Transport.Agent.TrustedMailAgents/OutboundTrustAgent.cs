using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Data.Transport.Routing;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;
using Microsoft.Exchange.Transport.Agent.AntiSpam.Common;

namespace Microsoft.Exchange.Transport.Agent.TrustedMail
{
	internal class OutboundTrustAgent : RoutingAgent
	{
		public OutboundTrustAgent(OutboundTrustAgentFactory parent, SmtpServer server, bool enabled)
		{
			if (!enabled)
			{
				return;
			}
			this.parent = parent;
			base.OnCategorizedMessage += this.OnCategorizedEventHandler;
		}

		public OutboundTrustAgent()
		{
		}

		private static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.OutboundTrustAgentTracer;
			}
		}

		protected virtual string ComputerName
		{
			get
			{
				return this.parent.ComputerName;
			}
		}

		protected virtual bool IsEdge
		{
			get
			{
				return Components.Configuration.LocalServer.TransportServer.IsEdgeServer;
			}
		}

		private static bool IsCrossPremisesMail(EmailMessage message)
		{
			return message.RootPart != null && HeaderFirewall.CrossPremisesHeadersPresent(message.RootPart.Headers);
		}

		protected virtual bool IsMessageNdr(EmailMessage message)
		{
			return message.MapiMessageClass != null && message.MapiMessageClass.Equals("Report.IPM.Note.NDR", StringComparison.InvariantCultureIgnoreCase);
		}

		protected virtual bool HasInternalDestinationOnEdge(EnvelopeRecipient recipient)
		{
			if (!this.IsEdge)
			{
				throw new InvalidOperationException("Internal destination should only be checked on Edge");
			}
			MailRecipient mailRecipient = ((MailRecipientWrapper)recipient).MailRecipient;
			string value;
			if (mailRecipient.RoutingOverride != null && mailRecipient.RoutingOverride.DeliveryQueueDomain == DeliveryQueueDomain.UseOverrideDomain)
			{
				value = mailRecipient.RoutingOverride.RoutingDomain.Domain;
			}
			else
			{
				value = recipient.Address.DomainPart;
			}
			return Components.Configuration.FirstOrgAcceptedDomainTable.EdgeToBHDomains.Contains(value);
		}

		public void OnCategorizedEventHandler(CategorizedMessageEventSource source, QueuedMessageEventArgs args)
		{
			this.mailItem = args.MailItem;
			this.messageId = this.mailItem.Message.MessageId;
			if (this.mailItem.Recipients.Count == 0)
			{
				OutboundTrustAgent.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Message {0} is skipped for CrossPremises processing since it has no recipients", this.messageId);
				return;
			}
			if (this.IsMessageNdr(this.mailItem.Message))
			{
				this.HandleNDRWithAttachedCrossPremisesMessage(this.mailItem);
			}
			if (TrustedMailUtils.HeadersPreservedOutbound(this.mailItem.Message.RootPart.Headers))
			{
				OutboundTrustAgent.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Message {0} is skipped for CrossPremises processing as the headers are already preserved", this.messageId);
				return;
			}
			List<EnvelopeRecipient> list = new List<EnvelopeRecipient>();
			List<EnvelopeRecipient> list2 = new List<EnvelopeRecipient>();
			List<EnvelopeRecipient> list3 = new List<EnvelopeRecipient>();
			bool flag = TrustedMailUtils.PreserveHeadersOnEdge(this.mailItem.Message.RootPart.Headers);
			foreach (EnvelopeRecipient envelopeRecipient in this.mailItem.Recipients)
			{
				bool? preserveCrossPremisesHeaders = null;
				object obj;
				if (envelopeRecipient.Properties.TryGetValue("PreserveCrossPremisesHeaders", out obj))
				{
					preserveCrossPremisesHeaders = new bool?((bool)obj);
				}
				if (((preserveCrossPremisesHeaders != null && preserveCrossPremisesHeaders.Value) || CommonUtils.IsExternalRecipient(envelopeRecipient, OutboundTrustAgent.Tracer, this.GetHashCode())) && !SmtpProxyAddress.IsEncapsulatedAddress(envelopeRecipient.Address.ToString()) && (!this.IsEdge || flag || !this.HasInternalDestinationOnEdge(envelopeRecipient)))
				{
					if (TrustedMailUtils.IsOutboundDomainTrusted(this.GetHashCode(), OutboundTrustAgent.Tracer, new TrustedMailUtils.GetRemoteDomainEntryDelegate(this.GetRemoteDomainEntry), new TrustedMailUtils.GetAcceptedDomainEntryDelegate(this.GetAcceptedDomainEntry), envelopeRecipient.Address.DomainPart, this.mailItem, envelopeRecipient, preserveCrossPremisesHeaders))
					{
						OutboundTrustAgent.Tracer.TraceDebug<string, string>((long)this.GetHashCode(), "Headers will be preserved on message {0} for recipient {1} as the recipient is external, domain is trustedand this is not an edge sending to an internal domain", this.messageId, envelopeRecipient.Address.ToString());
						list.Add(envelopeRecipient);
					}
					else if (TrustedMailUtils.IsDeliveryTypeSmtpRelayToEdge(envelopeRecipient))
					{
						OutboundTrustAgent.Tracer.TraceDebug<string, string>((long)this.GetHashCode(), "Recipient {0} on message {1} has next hop destination of Edge", envelopeRecipient.Address.ToString(), this.messageId);
						list2.Add(envelopeRecipient);
					}
					else if (HeaderFirewall.CrossPremisesHeadersPresent(this.mailItem.Message.RootPart.Headers))
					{
						OutboundTrustAgent.Tracer.TraceDebug<string, string>((long)this.GetHashCode(), "Headers will be blocked on message {0} for recipient {1} as the recipient is external, domain is not trustedand this is not an edge sending to an internal domain", this.messageId, envelopeRecipient.Address.ToString());
						list3.Add(envelopeRecipient);
					}
				}
			}
			if (list.Count != 0)
			{
				source.Fork(list);
				HeaderFirewall.FilterCrossPremisesHeaders(this.mailItem.Message.RootPart.Headers);
				if (HeaderFirewall.PreserveOutgoingcrossPremisesHeaders(OutboundTrustAgent.Tracer, this.mailItem.Message.RootPart.Headers))
				{
					TrustedMailUtils.StampHeader(this.mailItem.Message.RootPart.Headers, "X-MS-Exchange-Organization-Cross-Premises-Headers-Preserved", this.ComputerName);
					TrustedMailUtils.StampHeader(this.mailItem.Message.RootPart.Headers, "X-OrganizationHeadersPreserved", this.ComputerName);
					return;
				}
			}
			else
			{
				if (list2.Count != 0)
				{
					source.Fork(list2);
					TrustedMailUtils.StampHeader(this.mailItem.Message.RootPart.Headers, "X-MS-Exchange-Organization-PreserveHeadersOnEdge");
					return;
				}
				if (list3.Count != 0)
				{
					source.Fork(list3);
					HeaderFirewall.FilterCrossPremisesHeaders(this.mailItem.Message.RootPart.Headers);
					TrustedMailUtils.StampHeader(this.mailItem.Message.RootPart.Headers, "X-MS-Exchange-Organization-Cross-Premises-Headers-Blocked", this.ComputerName);
					TrustedMailUtils.StampHeader(this.mailItem.Message.RootPart.Headers, "X-CrossPremiseHeadersBlocked", this.ComputerName);
				}
			}
		}

		private void HandleNDRWithAttachedCrossPremisesMessage(MailItem mailItem)
		{
			EmailMessage emailMessage = null;
			foreach (Attachment attachment in mailItem.Message.Attachments)
			{
				if (attachment.ContentType.Equals("message/rfc822", StringComparison.OrdinalIgnoreCase) && attachment.EmbeddedMessage != null)
				{
					emailMessage = attachment.EmbeddedMessage;
					break;
				}
			}
			if (emailMessage == null)
			{
				OutboundTrustAgent.Tracer.TraceDebug<string>((long)this.GetHashCode(), "NDR with MsgId {0} doesn't have an attached message.", this.messageId);
				return;
			}
			if (OutboundTrustAgent.IsCrossPremisesMail(emailMessage))
			{
				OutboundTrustAgent.Tracer.TraceDebug<string>((long)this.GetHashCode(), "NDR with MsgId {0} has attached message with cross premise headers.", this.messageId);
				HeaderFirewall.FilterCrossPremisesHeaders(emailMessage.RootPart.Headers);
				if (mailItem.Message.RootPart.Headers.FindFirst("X-MS-Exchange-Organization-Dsn-Version") != null)
				{
					OutboundTrustAgent.Tracer.TraceDebug<string, string>((long)this.GetHashCode(), "Removing header {0} from NDR with MsgId {1}.", "X-MS-Exchange-Organization-Dsn-Version", this.messageId);
					mailItem.Message.RootPart.Headers.RemoveAll("X-MS-Exchange-Organization-Dsn-Version");
					return;
				}
			}
			else
			{
				OutboundTrustAgent.Tracer.TraceDebug<string>((long)this.GetHashCode(), "NDR with MsgId {0} has attached message which doesn't have preserved cross premises headers.", this.messageId);
			}
		}

		protected virtual RemoteDomainEntry GetRemoteDomainEntry(SmtpDomain domain, MailItem mailItem)
		{
			return TrustedMailUtils.GetRemoteDomainEntry(domain, mailItem);
		}

		protected virtual AcceptedDomainEntry GetAcceptedDomainEntry(SmtpDomain domain, MailItem mailItem)
		{
			return TrustedMailUtils.GetAcceptedDomainEntry(domain, mailItem);
		}

		private const string MapiNdrClass = "Report.IPM.Note.NDR";

		private const string DiagnosticHeadersPreservedHeader = "X-OrganizationHeadersPreserved";

		private const string DiagnosticHeadersBlockedHeader = "X-CrossPremiseHeadersBlocked";

		private string messageId;

		private MailItem mailItem;

		private OutboundTrustAgentFactory parent;
	}
}
