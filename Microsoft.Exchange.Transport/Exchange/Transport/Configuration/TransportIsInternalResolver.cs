using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Transport.RecipientAPI;

namespace Microsoft.Exchange.Transport.Configuration
{
	internal static class TransportIsInternalResolver
	{
		public static bool IsInternal(OrganizationId organizationId, RoutingAddress routingAddress, bool acceptedDomainsOnly = false)
		{
			IsInternalResolver isInternalResolver = TransportIsInternalResolver.CreateIsInternalResolver(organizationId, acceptedDomainsOnly);
			return isInternalResolver.IsInternal(routingAddress);
		}

		public static bool IsInternal(OrganizationId organizationId, RoutingDomain routingDomain, bool acceptedDomainsOnly = false)
		{
			IsInternalResolver isInternalResolver = TransportIsInternalResolver.CreateIsInternalResolver(organizationId, acceptedDomainsOnly);
			return isInternalResolver.IsInternal(routingDomain);
		}

		private static IsInternalResolver CreateIsInternalResolver(OrganizationId organizationId, bool acceptedDomainsOnly)
		{
			return new IsInternalResolver(organizationId, new IsInternalResolver.GetAcceptedDomainCollectionDelegate(TransportIsInternalResolver.GetAcceptedDomainCollection), acceptedDomainsOnly ? new IsInternalResolver.GetRemoteDomainCollectionDelegate(TransportIsInternalResolver.GetEmptyRemoteDomainCollection) : new IsInternalResolver.GetRemoteDomainCollectionDelegate(TransportIsInternalResolver.GetRemoteDomainCollection));
		}

		private static AcceptedDomainCollection GetAcceptedDomainCollection(OrganizationId organizationId, out bool scopedToOrganization)
		{
			scopedToOrganization = true;
			PerTenantAcceptedDomainTable perTenantAcceptedDomainTable;
			if (Components.Configuration.TryGetAcceptedDomainTable(organizationId, out perTenantAcceptedDomainTable))
			{
				return perTenantAcceptedDomainTable.AcceptedDomainTable;
			}
			return null;
		}

		private static RemoteDomainCollection GetRemoteDomainCollection(OrganizationId organizationId)
		{
			PerTenantRemoteDomainTable perTenantRemoteDomainTable;
			if (Components.Configuration.TryGetRemoteDomainTable(organizationId, out perTenantRemoteDomainTable))
			{
				return perTenantRemoteDomainTable.RemoteDomainTable;
			}
			return null;
		}

		private static RemoteDomainCollection GetEmptyRemoteDomainCollection(OrganizationId organizationId)
		{
			return RemoteDomainMap.Empty;
		}
	}
}
