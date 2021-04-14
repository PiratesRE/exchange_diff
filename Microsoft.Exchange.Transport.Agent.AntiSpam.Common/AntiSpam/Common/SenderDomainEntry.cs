using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Transport.Agent.AntiSpam.Common
{
	internal sealed class SenderDomainEntry : DomainMatchMap<SenderDomainEntry>.IDomainEntry
	{
		public SenderDomainEntry(SmtpDomainWithSubdomains domain)
		{
			this.domain = domain;
		}

		public SenderDomainEntry(SmtpDomain domain, bool includeSubdomains)
		{
			this.domain = new SmtpDomainWithSubdomains(domain, includeSubdomains);
		}

		public SmtpDomainWithSubdomains DomainName
		{
			get
			{
				return this.domain;
			}
		}

		private readonly SmtpDomainWithSubdomains domain;
	}
}
