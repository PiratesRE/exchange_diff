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
	internal class AddressBookDataProvider : RusDataProviderBase<ADRawEntry>
	{
		public AddressBookDataProvider(IConfigurationSession session, IConfigurationSession rootOrgSession, ProvisioningCache provisioningCache) : base(session, rootOrgSession, provisioningCache)
		{
		}

		protected override void LoadPolicies(PolicyContainer<ADRawEntry> container, LogMessageDelegate logger)
		{
			if (container == null)
			{
				throw new ArgumentNullException("container");
			}
			container.Policies = base.ProvisioningCache.TryAddAndGetOrganizationData<List<ADRawEntry>>(CannedProvisioningCacheKeys.AddressBookPolicies, container.OrganizationId, delegate()
			{
				List<ADRawEntry> list = new List<ADRawEntry>();
				ADObjectId adobjectId = container.OrganizationId.ConfigurationUnit ?? this.OrgContainerId;
				IConfigurationSession configurationSession = (adobjectId == this.OrgContainerId) ? this.RootOrgConfigurationSession : this.ConfigurationSession;
				if (logger != null)
				{
					logger(Strings.VerboseAddressListsForOganizationFromDC(container.OrganizationId.ToString(), this.DomainController));
				}
				ADPagedReader<ADRawEntry> adpagedReader = configurationSession.FindPagedADRawEntry(adobjectId, QueryScope.SubTree, new AndFilter(new QueryFilter[]
				{
					AddressBookDataProvider.dummyObject.ImplicitFilter,
					AddressBookDataProvider.dummyObject.VersioningFilter
				}), null, 0, AddressBookDataProvider.attributes);
				if (adpagedReader != null)
				{
					foreach (ADRawEntry item in adpagedReader)
					{
						list.Add(item);
					}
				}
				return list;
			});
		}

		private static AddressBookBase dummyObject = new AddressBookBase();

		private static ADPropertyDefinition[] attributes = new ADPropertyDefinition[]
		{
			AddressBookBaseSchema.LdapRecipientFilter,
			AddressBookBaseSchema.RecipientContainer,
			AddressBookBaseSchema.IsSystemAddressList
		};
	}
}
