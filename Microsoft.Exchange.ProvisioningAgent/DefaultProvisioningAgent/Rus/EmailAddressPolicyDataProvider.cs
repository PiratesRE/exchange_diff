using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ProvisioningCache;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.ProvisioningAgent;

namespace Microsoft.Exchange.DefaultProvisioningAgent.Rus
{
	internal class EmailAddressPolicyDataProvider : RusDataProviderBase<ADRawEntry>
	{
		public EmailAddressPolicyDataProvider(IConfigurationSession session, IConfigurationSession rootOrgSession, ProvisioningCache provisioningCache) : base(session, rootOrgSession, provisioningCache)
		{
		}

		protected override void LoadPolicies(PolicyContainer<ADRawEntry> container, LogMessageDelegate logger)
		{
			if (container == null)
			{
				throw new ArgumentNullException("container");
			}
			List<ADRawEntry> list = new List<ADRawEntry>();
			ADObjectId adobjectId = container.OrganizationId.ConfigurationUnit ?? base.OrgContainerId;
			IConfigurationSession configurationSession = (adobjectId == base.OrgContainerId) ? base.RootOrgConfigurationSession : base.ConfigurationSession;
			adobjectId = adobjectId.GetDescendantId(EmailAddressPolicy.RdnEapContainerToOrganization);
			if (logger != null)
			{
				logger(Strings.VerboseEmailAddressPoliciesForOganizationFromDC(container.OrganizationId.ToString(), base.DomainController));
			}
			ADPagedReader<ADRawEntry> adpagedReader = configurationSession.FindPagedADRawEntry(adobjectId, QueryScope.SubTree, new AndFilter(new QueryFilter[]
			{
				EmailAddressPolicyDataProvider.dummyObject.ImplicitFilter,
				EmailAddressPolicyDataProvider.dummyObject.VersioningFilter,
				new ComparisonFilter(ComparisonOperator.Equal, EmailAddressPolicySchema.PolicyOptionListValue, EmailAddressPolicy.PolicyGuid.ToByteArray())
			}), null, 0, EmailAddressPolicyDataProvider.attributes);
			if (adpagedReader != null)
			{
				foreach (ADRawEntry item in adpagedReader)
				{
					list.Add(item);
				}
			}
			container.Policies = list;
		}

		private static EmailAddressPolicy dummyObject = new EmailAddressPolicy();

		private static ADPropertyDefinition[] attributes = new ADPropertyDefinition[]
		{
			EmailAddressPolicySchema.LdapRecipientFilter,
			EmailAddressPolicySchema.RecipientContainer,
			EmailAddressPolicySchema.Priority,
			EmailAddressPolicySchema.EnabledEmailAddressTemplates,
			EmailAddressPolicySchema.PolicyOptionListValue
		};
	}
}
