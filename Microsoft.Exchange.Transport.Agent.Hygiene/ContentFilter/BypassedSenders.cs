using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics.Components.ContentFilter;
using Microsoft.Exchange.Transport.Agent.AntiSpam.Common;

namespace Microsoft.Exchange.Transport.Agent.ContentFilter
{
	internal sealed class BypassedSenders
	{
		public BypassedSenders(ICollection<SmtpAddress> bypassSenders, ICollection<SmtpDomainWithSubdomains> bypassSenderDomainsAndSubdomains)
		{
			List<SenderDomainEntry> list = new List<SenderDomainEntry>();
			foreach (SmtpDomainWithSubdomains smtpDomainWithSubdomains in bypassSenderDomainsAndSubdomains)
			{
				if (smtpDomainWithSubdomains.Equals(SmtpDomainWithSubdomains.StarDomain))
				{
					this.matchAllDomains = true;
					return;
				}
				list.Add(new SenderDomainEntry(smtpDomainWithSubdomains));
			}
			if (list.Count > 0)
			{
				this.domainMatchMap = new DomainMatchMap<SenderDomainEntry>(list);
			}
			if (bypassSenders.Count > 0)
			{
				this.bypassedSenders = new Dictionary<RoutingAddress, object>(bypassSenders.Count);
				foreach (SmtpAddress smtpAddress in bypassSenders)
				{
					this.bypassedSenders.Add((RoutingAddress)smtpAddress.ToString(), null);
				}
			}
		}

		public bool IsBypassed(RoutingAddress address)
		{
			if (!address.IsValid)
			{
				ExTraceGlobals.BypassedSendersTracer.TraceDebug<RoutingAddress>((long)this.GetHashCode(), "Invalid From address {0}.", address);
				return false;
			}
			RoutingAddress routingAddress = address;
			if (this.matchAllDomains)
			{
				ExTraceGlobals.BypassedSendersTracer.TraceDebug<RoutingAddress>((long)this.GetHashCode(), "Content Filter Config is configured to bypass senders for * (all domains). Bypassing mail from {0}", address);
				return true;
			}
			if (this.bypassedSenders != null && this.bypassedSenders.ContainsKey(routingAddress))
			{
				ExTraceGlobals.BypassedSendersTracer.TraceDebug<RoutingAddress>((long)this.GetHashCode(), "Sender {0} is configured as an org wide safe sender. Bypassing scan.", address);
				return true;
			}
			if (this.domainMatchMap != null)
			{
				SmtpDomain domainPart = SmtpDomain.GetDomainPart(routingAddress);
				SenderDomainEntry bestMatch = this.domainMatchMap.GetBestMatch(domainPart);
				if (bestMatch != null)
				{
					ExTraceGlobals.BypassedSendersTracer.TraceDebug<RoutingAddress>((long)this.GetHashCode(), "Sender {0} matched a safe domain entry in the org wide safe domains. Bypassing scan.", address);
					return true;
				}
			}
			return false;
		}

		private bool matchAllDomains;

		private Dictionary<RoutingAddress, object> bypassedSenders;

		private DomainMatchMap<SenderDomainEntry> domainMatchMap;
	}
}
