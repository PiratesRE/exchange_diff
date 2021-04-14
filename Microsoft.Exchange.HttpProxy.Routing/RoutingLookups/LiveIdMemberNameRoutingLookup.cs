using System;
using Microsoft.Exchange.HttpProxy.Routing.Providers;
using Microsoft.Exchange.HttpProxy.Routing.RoutingKeys;

namespace Microsoft.Exchange.HttpProxy.Routing.RoutingLookups
{
	internal class LiveIdMemberNameRoutingLookup : MailboxRoutingLookupBase<LiveIdMemberNameRoutingKey>
	{
		public LiveIdMemberNameRoutingLookup(IUserProvider userProvider) : base(userProvider)
		{
		}

		protected override User FindUser(LiveIdMemberNameRoutingKey liveIdMemberNameRoutingKey, IRoutingDiagnostics diagnostics)
		{
			return base.UserProvider.FindByLiveIdMemberName(liveIdMemberNameRoutingKey.LiveIdMemberName, liveIdMemberNameRoutingKey.OrganizationContext, diagnostics);
		}

		protected override string GetDomainName(LiveIdMemberNameRoutingKey routingKey)
		{
			return routingKey.OrganizationDomain;
		}
	}
}
