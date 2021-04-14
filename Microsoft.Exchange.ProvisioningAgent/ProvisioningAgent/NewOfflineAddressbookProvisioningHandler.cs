using System;
using System.Collections.Generic;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.OAB;
using Microsoft.Exchange.Provisioning.Agent;
using Microsoft.Exchange.Provisioning.LoadBalancing;

namespace Microsoft.Exchange.ProvisioningAgent
{
	[CmdletHandler("new-offlineaddressbook")]
	internal class NewOfflineAddressbookProvisioningHandler : MoveOfflineAddressbookProvisioningHandler
	{
		public override IConfigurable ProvisionDefaultProperties(IConfigurable readOnlyIConfigurable)
		{
			OfflineAddressBook offlineAddressBook = base.ProvisionDefaultProperties(readOnlyIConfigurable) as OfflineAddressBook;
			string domainController = null;
			if (base.UserSpecifiedParameters["DomainController"] != null)
			{
				domainController = (Fqdn)base.UserSpecifiedParameters["DomainController"];
			}
			if (Datacenter.IsMultiTenancyEnabled())
			{
				if (base.UserSpecifiedParameters["VirtualDirectories"] == null && base.UserSpecifiedParameters["GlobalWebDistributionEnabled"] == null)
				{
					if (offlineAddressBook == null)
					{
						offlineAddressBook = new OfflineAddressBook();
					}
					offlineAddressBook.VirtualDirectories = null;
					offlineAddressBook.GlobalWebDistributionEnabled = true;
				}
			}
			else
			{
				bool flag = (bool)(base.UserSpecifiedParameters["GlobalWebDistributionEnabled"] ?? false);
				if (base.UserSpecifiedParameters["Server"] == null && !flag && base.UserSpecifiedParameters["VirtualDirectories"] == null)
				{
					OfflineAddressBook offlineAddressBook2 = readOnlyIConfigurable as OfflineAddressBook;
					if (offlineAddressBook2 != null && offlineAddressBook2.VirtualDirectories != null && offlineAddressBook2.VirtualDirectories.Count != 0)
					{
						return offlineAddressBook;
					}
					MultiValuedProperty<ADObjectId> multiValuedProperty = new MultiValuedProperty<ADObjectId>();
					foreach (object obj in PhysicalResourceLoadBalancing.FindVirtualDirectories(domainController, base.LogMessage))
					{
						ADObjectId item = (ADObjectId)obj;
						multiValuedProperty.Add(item);
					}
					if (multiValuedProperty.Count > 0)
					{
						if (offlineAddressBook == null)
						{
							offlineAddressBook = new OfflineAddressBook();
						}
						offlineAddressBook.VirtualDirectories = multiValuedProperty;
					}
				}
			}
			if (OABVariantConfigurationSettings.IsLinkedOABGenMailboxesEnabled && !base.UserSpecifiedParameters.IsModified("GeneratingMailbox"))
			{
				offlineAddressBook.GeneratingMailbox = this.FindGeneratingMailbox(domainController, offlineAddressBook.OrganizationId);
			}
			return offlineAddressBook;
		}

		private ADObjectId FindGeneratingMailbox(string domainController, OrganizationId orgId)
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), orgId, null, false);
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(domainController, true, ConsistencyMode.FullyConsistent, sessionSettings, 148, "FindGeneratingMailbox", "f:\\15.00.1497\\sources\\dev\\Management\\src\\ProvisioningAgent\\OfflineAddressBookProvisioningHandler.cs");
			List<ADUser> organizationMailboxesByCapability = OrganizationMailbox.GetOrganizationMailboxesByCapability(tenantOrRootOrgRecipientSession, OrganizationCapability.OABGen);
			if (organizationMailboxesByCapability == null || organizationMailboxesByCapability.Count == 0)
			{
				return null;
			}
			if (organizationMailboxesByCapability.Count == 1)
			{
				return organizationMailboxesByCapability[0].Id;
			}
			foreach (ADUser aduser in organizationMailboxesByCapability)
			{
				if (string.Equals(aduser.Name, "SystemMailbox{bb558c35-97f1-4cb9-8ff7-d53741dc928c}", StringComparison.OrdinalIgnoreCase))
				{
					return aduser.Id;
				}
			}
			return organizationMailboxesByCapability[NewOfflineAddressbookProvisioningHandler.random.Next(organizationMailboxesByCapability.Count)].Id;
		}

		private const string DefaultOABGenMailboxName = "SystemMailbox{bb558c35-97f1-4cb9-8ff7-d53741dc928c}";

		private static readonly Random random = new Random();
	}
}
