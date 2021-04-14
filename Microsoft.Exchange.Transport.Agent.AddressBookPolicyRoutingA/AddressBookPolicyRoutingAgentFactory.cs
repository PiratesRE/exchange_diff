using System;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Routing;

namespace Microsoft.Exchange.Transport.Agent.AddressBookPolicyRoutingAgent
{
	public sealed class AddressBookPolicyRoutingAgentFactory : RoutingAgentFactory
	{
		public AddressBookPolicyRoutingAgentFactory()
		{
			this.cacheTimeout = TimeSpan.FromMinutes((double)TransportAppConfig.GetConfigInt("AddressBookPolicyRoutingCacheTimeoutMinutes", 1, 30, 30));
			this.deferralTimeout = TimeSpan.FromMinutes((double)TransportAppConfig.GetConfigInt("AddressBookPolicyRoutingDeferralTimeoutMinutes", 1, 60, 15));
		}

		public override RoutingAgent CreateAgent(SmtpServer server)
		{
			return new AddressBookPolicyRoutingAgent(this.abpToGalCache, this.orgToGalCache, this.userToAddrListCache, this.cacheTimeout, this.deferralTimeout);
		}

		private const string DeferralTimeoutName = "AddressBookPolicyRoutingDeferralTimeoutMinutes";

		private const string CacheTimeoutName = "AddressBookPolicyRoutingCacheTimeoutMinutes";

		private readonly TimeSpan cacheTimeout;

		private readonly TimeSpan deferralTimeout;

		private readonly TimeoutCache<Guid, Guid> abpToGalCache = new TimeoutCache<Guid, Guid>(16, 1024, false);

		private readonly TimeoutCache<OrganizationId, AddressBookBase[]> orgToGalCache = new TimeoutCache<OrganizationId, AddressBookBase[]>(16, 1024, false);

		private readonly TimeoutCache<ADObjectId, ADMultiValuedProperty<ADObjectId>> userToAddrListCache = new TimeoutCache<ADObjectId, ADMultiValuedProperty<ADObjectId>>(16, 1024, false);
	}
}
