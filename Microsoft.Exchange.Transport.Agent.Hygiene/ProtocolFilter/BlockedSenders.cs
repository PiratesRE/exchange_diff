using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics.Components.ProtocolFilter;
using Microsoft.Exchange.Transport.Agent.AntiSpam.Common;

namespace Microsoft.Exchange.Transport.Agent.ProtocolFilter
{
	internal sealed class BlockedSenders
	{
		public BlockedSenders(ICollection<SmtpAddress> blockedSenders, ICollection<SmtpDomain> blockedDomains, ICollection<SmtpDomain> blockedDomainsAndSubdomains)
		{
			if (blockedSenders.Count > 0)
			{
				this.blockedSenders = new Dictionary<RoutingAddress, object>(blockedSenders.Count);
				foreach (SmtpAddress smtpAddress in blockedSenders)
				{
					this.blockedSenders.Add((RoutingAddress)smtpAddress.ToString(), null);
				}
			}
			List<SenderDomainEntry> list = new List<SenderDomainEntry>();
			foreach (SmtpDomain domain in blockedDomains)
			{
				list.Add(new SenderDomainEntry(domain, false));
			}
			foreach (SmtpDomain domain2 in blockedDomainsAndSubdomains)
			{
				list.Add(new SenderDomainEntry(domain2, true));
			}
			if (list.Count > 0)
			{
				this.domainMatchMap = new DomainMatchMap<SenderDomainEntry>(list);
			}
		}

		public bool IsBlocked(RoutingAddress address, out LogEntry logEntry)
		{
			logEntry = null;
			if (this.blockedSenders != null && this.blockedSenders.ContainsKey(address))
			{
				ExTraceGlobals.SenderFilterAgentTracer.TraceDebug<RoutingAddress>((long)this.GetHashCode(), "Sender {0} is an exact match", address);
				logEntry = SenderFilterAgent.RejectContext.ExactMatch(address.ToString());
				return true;
			}
			if (this.domainMatchMap != null)
			{
				SmtpDomain domainPart = SmtpDomain.GetDomainPart(address);
				SenderDomainEntry bestMatch = this.domainMatchMap.GetBestMatch(domainPart);
				if (bestMatch != null)
				{
					ExTraceGlobals.SenderFilterAgentTracer.TraceDebug<RoutingAddress>((long)this.GetHashCode(), "Sender {0} was matched because of a blocked domain or parent domain", address);
					logEntry = ((!bestMatch.DomainName.IncludeSubDomains) ? SenderFilterAgent.RejectContext.DomainMatch(address.DomainPart) : SenderFilterAgent.RejectContext.SubdomainMatch(address.DomainPart));
					return true;
				}
			}
			return false;
		}

		private Dictionary<RoutingAddress, object> blockedSenders;

		private DomainMatchMap<SenderDomainEntry> domainMatchMap;
	}
}
