using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.Transport.RecipientAPI
{
	internal sealed class IsInternalResolver
	{
		public IsInternalResolver(OrganizationId organizationId, IsInternalResolver.GetAcceptedDomainCollectionDelegate getAcceptedDomainCollectionDelegate, IsInternalResolver.GetRemoteDomainCollectionDelegate getRemoteDomainCollectionDelegate)
		{
			if (getAcceptedDomainCollectionDelegate == null)
			{
				throw new ArgumentNullException("getAcceptedDomainCollectionDelegate");
			}
			if (getRemoteDomainCollectionDelegate == null)
			{
				throw new ArgumentNullException("getRemoteDomainCollectionDelegate");
			}
			this.organizationId = organizationId;
			this.getAcceptedDomainCollectionDelegate = getAcceptedDomainCollectionDelegate;
			this.getRemoteDomainCollectionDelegate = getRemoteDomainCollectionDelegate;
		}

		public static bool IsInternal(RoutingAddress routingAddress, OrganizationId organizationId)
		{
			return routingAddress.IsValid && IsInternalResolver.IsInternal(routingAddress.DomainPart, organizationId);
		}

		public static bool IsInternal(RoutingDomain routingDomain, OrganizationId organizationId)
		{
			return routingDomain.IsValid() && IsInternalResolver.IsInternal(routingDomain.Domain, organizationId);
		}

		public bool IsInternal(RoutingAddress address)
		{
			return address.IsValid && this.IsInternal(address.DomainPart);
		}

		public bool IsInternal(RoutingDomain domain)
		{
			return domain.IsValid() && this.IsInternal(domain.Domain);
		}

		private static bool IsInternal(string domainStringRepresentation, OrganizationId organizationId)
		{
			IConfigurationSession session = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(organizationId), 145, "IsInternal", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\EdgeExtensibility\\IsInternalResolver.cs");
			ADPagedReader<Microsoft.Exchange.Data.Directory.SystemConfiguration.AcceptedDomain> acceptedDomains = null;
			ADNotificationAdapter.RunADOperation(delegate()
			{
				acceptedDomains = session.FindAllPaged<Microsoft.Exchange.Data.Directory.SystemConfiguration.AcceptedDomain>();
			}, 3);
			foreach (Microsoft.Exchange.Data.Directory.SystemConfiguration.AcceptedDomain acceptedDomain in acceptedDomains)
			{
				if (acceptedDomain.DomainType != AcceptedDomainType.ExternalRelay)
				{
					SmtpDomainWithSubdomains smtpDomainWithSubdomains = new SmtpDomainWithSubdomains(acceptedDomain.DomainName.Domain, acceptedDomain.DomainName.IncludeSubDomains || acceptedDomain.MatchSubDomains);
					if (smtpDomainWithSubdomains.Match(domainStringRepresentation) >= 0)
					{
						return true;
					}
				}
			}
			ADPagedReader<DomainContentConfig> remoteDomains = null;
			ADNotificationAdapter.TryRunADOperation(delegate()
			{
				remoteDomains = session.FindAllPaged<DomainContentConfig>();
			});
			foreach (DomainContentConfig domainContentConfig in remoteDomains)
			{
				if (domainContentConfig.IsInternal && domainContentConfig.DomainName.Match(domainStringRepresentation) >= 0)
				{
					return true;
				}
			}
			return false;
		}

		private bool IsInternal(string routingDomainStringRepr)
		{
			bool flag;
			AcceptedDomainCollection acceptedDomainCollection = this.getAcceptedDomainCollectionDelegate(this.organizationId, out flag);
			Microsoft.Exchange.Data.Transport.AcceptedDomain acceptedDomain = (acceptedDomainCollection != null) ? acceptedDomainCollection.Find(routingDomainStringRepr) : null;
			if (acceptedDomain != null && acceptedDomain.IsInCorporation)
			{
				bool flag2 = true;
				if (!flag)
				{
					flag2 = (this.organizationId.Equals(OrganizationId.ForestWideOrgId) ? (acceptedDomain.TenantId == Guid.Empty) : (this.organizationId.ConfigurationUnit.ObjectGuid == acceptedDomain.TenantId));
				}
				if (flag2)
				{
					return true;
				}
			}
			RemoteDomainCollection remoteDomainCollection = this.getRemoteDomainCollectionDelegate(this.organizationId);
			RemoteDomain remoteDomain = (remoteDomainCollection != null) ? remoteDomainCollection.Find(routingDomainStringRepr) : null;
			return remoteDomain != null && remoteDomain.IsInternal;
		}

		private const int ADOperationRetryCount = 3;

		private readonly OrganizationId organizationId;

		private readonly IsInternalResolver.GetAcceptedDomainCollectionDelegate getAcceptedDomainCollectionDelegate;

		private readonly IsInternalResolver.GetRemoteDomainCollectionDelegate getRemoteDomainCollectionDelegate;

		internal delegate AcceptedDomainCollection GetAcceptedDomainCollectionDelegate(OrganizationId organizationId, out bool scopedToOrganization);

		internal delegate RemoteDomainCollection GetRemoteDomainCollectionDelegate(OrganizationId organizationId);
	}
}
