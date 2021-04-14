using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Provisioning;
using Microsoft.Exchange.Provisioning.Agent;
using Microsoft.Exchange.Provisioning.LoadBalancing;

namespace Microsoft.Exchange.ProvisioningAgent
{
	[CmdletHandler("move-offlineaddressbook")]
	internal class MoveOfflineAddressbookProvisioningHandler : ProvisioningHandlerBase
	{
		public override IConfigurable ProvisionDefaultProperties(IConfigurable readOnlyIConfigurable)
		{
			if (base.UserSpecifiedParameters["Server"] == null)
			{
				OfflineAddressBook offlineAddressBook = readOnlyIConfigurable as OfflineAddressBook;
				if (offlineAddressBook != null && offlineAddressBook.Server != null)
				{
					return null;
				}
				if (base.UserSpecifiedParameters["Identity"] != null)
				{
					OfflineAddressBookIdParameter offlineAddressBookIdParameter = (OfflineAddressBookIdParameter)base.UserSpecifiedParameters["Identity"];
					IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 59, "ProvisionDefaultProperties", "f:\\15.00.1497\\sources\\dev\\Management\\src\\ProvisioningAgent\\MoveOfflineAddressBookProvisioningHandler.cs");
					IEnumerable<OfflineAddressBook> objects = offlineAddressBookIdParameter.GetObjects<OfflineAddressBook>(null, tenantOrTopologyConfigurationSession);
					using (IEnumerator<OfflineAddressBook> enumerator = objects.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							offlineAddressBook = enumerator.Current;
						}
					}
				}
				string domainController = null;
				if (base.UserSpecifiedParameters["DomainController"] != null)
				{
					domainController = (Fqdn)base.UserSpecifiedParameters["DomainController"];
				}
				Server server = PhysicalResourceLoadBalancing.FindMailboxServer(domainController, offlineAddressBook, base.LogMessage);
				if (server != null)
				{
					offlineAddressBook = new OfflineAddressBook();
					offlineAddressBook.Server = (ADObjectId)server.Identity;
					return offlineAddressBook;
				}
			}
			return null;
		}
	}
}
