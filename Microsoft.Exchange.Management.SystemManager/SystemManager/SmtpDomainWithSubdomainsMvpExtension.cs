using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.SystemManager
{
	public static class SmtpDomainWithSubdomainsMvpExtension
	{
		public static void InsertDomain(this MultiValuedProperty<SmtpDomainWithSubdomains> domainsList, string domainName)
		{
			if (domainsList != null && !string.IsNullOrEmpty(domainName))
			{
				SmtpDomainWithSubdomains domain = new SmtpDomainWithSubdomains(domainName);
				domainsList.InsertDomain(domain);
			}
		}

		public static void InsertDomain(this MultiValuedProperty<SmtpDomainWithSubdomains> domainsList, SmtpDomainWithSubdomains domain)
		{
			if (domainsList != null && domain != null && !domainsList.Contains(domain))
			{
				domainsList.Add(domain);
			}
		}

		public static void InsertDomainRange(this MultiValuedProperty<SmtpDomainWithSubdomains> domainsList, MultiValuedProperty<SmtpDomainWithSubdomains> domains)
		{
			if (domainsList != null && domains != null)
			{
				foreach (SmtpDomainWithSubdomains domain in domains)
				{
					domainsList.InsertDomain(domain);
				}
			}
		}

		public static MultiValuedProperty<SmtpDomainWithSubdomains> AddPrefixForEachDomain(this MultiValuedProperty<SmtpDomainWithSubdomains> domainsList, string prefix)
		{
			MultiValuedProperty<SmtpDomainWithSubdomains> multiValuedProperty = new MultiValuedProperty<SmtpDomainWithSubdomains>();
			if (domainsList != null)
			{
				foreach (SmtpDomainWithSubdomains smtpDomainWithSubdomains in domainsList)
				{
					string s = prefix + smtpDomainWithSubdomains.SmtpDomain.Domain;
					multiValuedProperty.InsertDomain(new SmtpDomainWithSubdomains(s, smtpDomainWithSubdomains.IncludeSubDomains));
				}
			}
			return multiValuedProperty;
		}

		public static bool IsSameWith(this MultiValuedProperty<SmtpDomainWithSubdomains> domainsList, MultiValuedProperty<SmtpDomainWithSubdomains> domains)
		{
			if (domainsList == domains)
			{
				return true;
			}
			if (domains.Count != domainsList.Count)
			{
				return false;
			}
			foreach (SmtpDomainWithSubdomains item in domains)
			{
				if (!domainsList.Contains(item))
				{
					return false;
				}
			}
			return true;
		}

		public static SmtpDomainWithSubdomains GetShortestDomain(this MultiValuedProperty<SmtpDomainWithSubdomains> domainsList)
		{
			SmtpDomainWithSubdomains smtpDomainWithSubdomains = null;
			if (domainsList != null)
			{
				foreach (SmtpDomainWithSubdomains smtpDomainWithSubdomains2 in domainsList)
				{
					int num = (smtpDomainWithSubdomains == null) ? int.MaxValue : smtpDomainWithSubdomains.ToString().Length;
					if (smtpDomainWithSubdomains2.ToString().Length < num)
					{
						smtpDomainWithSubdomains = smtpDomainWithSubdomains2;
					}
				}
			}
			return smtpDomainWithSubdomains;
		}
	}
}
