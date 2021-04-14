using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Data.Transport.Routing;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;

namespace Microsoft.Exchange.Transport.Agent.TrustedMail
{
	internal class CentralizedMailFlowAgent : RoutingAgent
	{
		private static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.InboundTrustAgentTracer;
			}
		}

		private static bool OverrideAlreadySetOnce(EmailMessage message)
		{
			Header header = message.RootPart.Headers.FindFirst("X-MS-Exchange-Organization-CentralizedMailFlowOverrideSet");
			return null != header;
		}

		public CentralizedMailFlowAgent(bool enabled)
		{
			if (!enabled)
			{
				return;
			}
			if (Components.Configuration.FirstOrgAcceptedDomainTable.DefaultDomain != null)
			{
				this.defaultAcceptedDomain = new RoutingDomain(Components.Configuration.FirstOrgAcceptedDomainTable.DefaultDomain.DomainName.Domain);
				CentralizedMailFlowAgent.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Default accepted domain found :{0}", Components.Configuration.FirstOrgAcceptedDomainTable.DefaultDomain.DomainName.Domain);
			}
			else
			{
				foreach (Microsoft.Exchange.Data.Transport.AcceptedDomain acceptedDomain in Components.Configuration.FirstOrgAcceptedDomainTable)
				{
					if (!acceptedDomain.NameSpecification.Equals(SmtpDomainWithSubdomains.StarDomain.Domain))
					{
						this.defaultAcceptedDomain = new RoutingDomain(acceptedDomain.NameSpecification);
						CentralizedMailFlowAgent.Tracer.TraceDebug<string>((long)this.GetHashCode(), "No default accepted domain found, using first returned accepted domain :{0}", acceptedDomain.NameSpecification);
						break;
					}
				}
			}
			base.OnResolvedMessage += this.OnResolvedMessageHandler;
		}

		public CentralizedMailFlowAgent(RoutingDomain defaultAcceptedDomain)
		{
			this.defaultAcceptedDomain = defaultAcceptedDomain;
		}

		private void OnResolvedMessageHandler(ResolvedMessageEventSource source, QueuedMessageEventArgs args)
		{
			if (this.defaultAcceptedDomain == RoutingDomain.Empty)
			{
				CentralizedMailFlowAgent.Tracer.TraceError((long)this.GetHashCode(), "Not processing message as no accepted domains found");
				return;
			}
			if (CentralizedMailFlowAgent.OverrideAlreadySetOnce(args.MailItem.Message))
			{
				CentralizedMailFlowAgent.Tracer.TraceDebug((long)this.GetHashCode(), "Not processing message as an override has already been set on its recipients once");
				return;
			}
			SmtpDomain originatingDomain = this.GetOriginatingDomain(args);
			bool flag = false;
			if (originatingDomain == null)
			{
				CentralizedMailFlowAgent.Tracer.TraceDebug((long)this.GetHashCode(), "No originating domain present on mail");
				return;
			}
			if (TrustedMailUtils.InboundTrustEnabledOnMail(this.GetHashCode(), CentralizedMailFlowAgent.Tracer, args.MailItem.Properties, new TrustedMailUtils.GetRemoteDomainEntryDelegate(this.GetRemoteDomainEntry), new TrustedMailUtils.GetAcceptedDomainEntryDelegate(this.GetAcceptedDomainEntry), originatingDomain, null, out flag))
			{
				int num = 0;
				foreach (EnvelopeRecipient envelopeRecipient in args.MailItem.Recipients)
				{
					if (this.FindAcceptedDomain(envelopeRecipient.Address) == null)
					{
						num++;
						this.SetHubRoutingOverride(source, envelopeRecipient, this.defaultAcceptedDomain);
					}
				}
				CentralizedMailFlowAgent.Tracer.TraceDebug<int, int>((long)this.GetHashCode(), "Set RoutingOverride for {0} out of {1} recipients", num, args.MailItem.Recipients.Count);
				TrustedMailUtils.StampHeader(args.MailItem.Message.RootPart.Headers, "X-MS-Exchange-Organization-CentralizedMailFlowOverrideSet", null);
				return;
			}
			CentralizedMailFlowAgent.Tracer.TraceDebug((long)this.GetHashCode(), "Originating domain present on message but not enabled for inbound trust");
		}

		protected virtual SmtpDomain GetOriginatingDomain(QueuedMessageEventArgs queuedMessageEventArgs)
		{
			string originatorOrganization = queuedMessageEventArgs.MailItem.OriginatorOrganization;
			if (!string.IsNullOrEmpty(originatorOrganization))
			{
				return new SmtpDomain(originatorOrganization);
			}
			return null;
		}

		protected virtual RemoteDomainEntry GetRemoteDomainEntry(SmtpDomain domain, MailItem mailItem)
		{
			return TrustedMailUtils.GetRemoteDomainEntry(domain, mailItem);
		}

		protected virtual AcceptedDomainEntry GetAcceptedDomainEntry(SmtpDomain domain, MailItem mailItem)
		{
			return TrustedMailUtils.GetAcceptedDomainEntry(domain, mailItem);
		}

		protected virtual Microsoft.Exchange.Data.Transport.AcceptedDomain FindAcceptedDomain(RoutingAddress address)
		{
			return Components.Configuration.FirstOrgAcceptedDomainTable.Find(address);
		}

		private void SetHubRoutingOverride(ResolvedMessageEventSource source, EnvelopeRecipient recipient, RoutingDomain overrideDomain)
		{
			RoutingOverride routingOverride = source.GetRoutingOverride(recipient);
			RoutingOverride routingOverride2 = new RoutingOverride(overrideDomain, DeliveryQueueDomain.UseOverrideDomain);
			source.SetRoutingOverride(recipient, routingOverride2);
			CentralizedMailFlowAgent.Tracer.TraceDebug<RoutingAddress, object, RoutingOverride>((long)this.GetHashCode(), "Recipient = {0}, Old Override = {1},New Override = {2}", recipient.Address, routingOverride ?? "none", routingOverride2);
		}

		public const string CentralizedMailFlowOverrideSetHeader = "X-MS-Exchange-Organization-CentralizedMailFlowOverrideSet";

		private readonly RoutingDomain defaultAcceptedDomain = RoutingDomain.Empty;
	}
}
