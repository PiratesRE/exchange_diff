using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ProvisioningCache;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	internal class SiteMailboxAddressesTemplate
	{
		public string UserPrincipalNameDomain { get; set; }

		public MultiValuedProperty<ProxyAddressTemplate> AddressTemplates { get; private set; }

		public bool IsValid
		{
			get
			{
				return !string.IsNullOrEmpty(this.UserPrincipalNameDomain) && this.AddressTemplates.Count > 0;
			}
		}

		public SiteMailboxAddressesTemplate()
		{
			this.AddressTemplates = new MultiValuedProperty<ProxyAddressTemplate>();
		}

		public static SiteMailboxAddressesTemplate GetSiteMailboxAddressesTemplate(IConfigurationSession cfgSession, ProvisioningCache provisioningCache)
		{
			if (cfgSession == null)
			{
				throw new ArgumentNullException("cfgSession");
			}
			if (provisioningCache == null)
			{
				throw new ArgumentNullException("provisioningCache");
			}
			OrganizationId orgId = cfgSession.GetOrgContainer().OrganizationId;
			return provisioningCache.TryAddAndGetOrganizationData<SiteMailboxAddressesTemplate>(CannedProvisioningCacheKeys.OrganizationSiteMailboxAddressesTemplate, orgId, delegate()
			{
				ADObjectId rootId = orgId.ConfigurationUnit ?? provisioningCache.TryAddAndGetGlobalData<ADObjectId>(CannedProvisioningCacheKeys.FirstOrgContainerId, () => cfgSession.GetOrgContainerId());
				MultiValuedProperty<SmtpDomain> multiValuedProperty = null;
				ADPagedReader<OnPremisesOrganization> adpagedReader = cfgSession.FindPaged<OnPremisesOrganization>(rootId, QueryScope.SubTree, null, null, 0);
				using (IEnumerator<OnPremisesOrganization> enumerator = adpagedReader.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						OnPremisesOrganization onPremisesOrganization = enumerator.Current;
						multiValuedProperty = onPremisesOrganization.HybridDomains;
					}
				}
				QueryFilter filter = new NotFilter(new BitMaskAndFilter(AcceptedDomainSchema.AcceptedDomainFlags, 1UL));
				ADPagedReader<AcceptedDomain> adpagedReader2 = cfgSession.FindPaged<AcceptedDomain>(rootId, QueryScope.SubTree, filter, new SortBy(ADObjectSchema.Name, SortOrder.Ascending), 0);
				bool flag = false;
				string text = string.Empty;
				string text2 = string.Empty;
				string text3 = string.Empty;
				foreach (AcceptedDomain acceptedDomain in adpagedReader2)
				{
					if (acceptedDomain.AuthenticationType != AuthenticationType.Federated && (string.IsNullOrEmpty(text) || acceptedDomain.Default))
					{
						text = acceptedDomain.DomainName.Domain;
					}
					if ((multiValuedProperty == null || multiValuedProperty.Count == 0 || multiValuedProperty.Contains(acceptedDomain.DomainName.SmtpDomain)) && (string.IsNullOrEmpty(text2) || acceptedDomain.Default))
					{
						text2 = acceptedDomain.DomainName.Domain;
					}
					if (acceptedDomain.IsCoexistenceDomain && string.IsNullOrEmpty(text3))
					{
						text3 = acceptedDomain.DomainName.Domain;
					}
					flag = (flag || acceptedDomain.Default);
					if (flag && !string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2) && !string.IsNullOrEmpty(text3))
					{
						break;
					}
				}
				SiteMailboxAddressesTemplate siteMailboxAddressesTemplate = new SiteMailboxAddressesTemplate();
				siteMailboxAddressesTemplate.UserPrincipalNameDomain = text;
				if (!string.IsNullOrEmpty(text2))
				{
					siteMailboxAddressesTemplate.AddressTemplates.Add(new SmtpProxyAddressTemplate(string.Format("@{0}", text2), true));
					if (!string.IsNullOrEmpty(text3) && !string.Equals(text2, text3, StringComparison.OrdinalIgnoreCase))
					{
						siteMailboxAddressesTemplate.AddressTemplates.Add(new SmtpProxyAddressTemplate(string.Format("@{0}", text3), false));
					}
				}
				if (!siteMailboxAddressesTemplate.IsValid)
				{
					throw new ErrorSiteMailboxCannotLoadAddressTemplateException();
				}
				return siteMailboxAddressesTemplate;
			});
		}
	}
}
