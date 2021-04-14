using System;
using Microsoft.Exchange.HttpProxy.Routing.Providers;
using Microsoft.Exchange.HttpProxy.Routing.RoutingKeys;

namespace Microsoft.Exchange.HttpProxy.Routing.RoutingLookups
{
	internal class ExternalDirectoryObjectIdRoutingLookup : MailboxRoutingLookupBase<ExternalDirectoryObjectIdRoutingKey>
	{
		public ExternalDirectoryObjectIdRoutingLookup(IUserProvider userProvider) : base(userProvider)
		{
		}

		protected override User FindUser(ExternalDirectoryObjectIdRoutingKey externalDirectoryObjectIdRoutingKey, IRoutingDiagnostics diagnostics)
		{
			return base.UserProvider.FindByExternalDirectoryObjectId(externalDirectoryObjectIdRoutingKey.UserGuid, externalDirectoryObjectIdRoutingKey.TenantGuid, diagnostics);
		}

		protected override string GetDomainName(ExternalDirectoryObjectIdRoutingKey routingKey)
		{
			return string.Empty;
		}
	}
}
