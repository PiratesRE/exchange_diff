using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal sealed class DomainToOrganizationIdCache
	{
		public static DomainToOrganizationIdCache Singleton
		{
			get
			{
				return DomainToOrganizationIdCache.singleton;
			}
		}

		public OrganizationId Get(SmtpDomain smtpDomain)
		{
			if (smtpDomain == null)
			{
				throw new ArgumentNullException("smtpDomain");
			}
			SmtpDomainWithSubdomains smtpDomainWithSubdomain = new SmtpDomainWithSubdomains(smtpDomain, false);
			return this.Get(smtpDomainWithSubdomain);
		}

		public OrganizationId Get(SmtpDomainWithSubdomains smtpDomainWithSubdomain)
		{
			DomainProperties domainProperties = DomainPropertyCache.Singleton.Get(smtpDomainWithSubdomain);
			if (domainProperties == null)
			{
				return null;
			}
			return domainProperties.OrganizationId;
		}

		private static DomainToOrganizationIdCache singleton = new DomainToOrganizationIdCache();
	}
}
