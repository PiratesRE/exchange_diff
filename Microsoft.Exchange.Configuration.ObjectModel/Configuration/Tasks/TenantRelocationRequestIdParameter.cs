using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public sealed class TenantRelocationRequestIdParameter : ADIdParameter
	{
		public TenantRelocationRequestIdParameter(string identity) : base(identity)
		{
		}

		public TenantRelocationRequestIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public TenantRelocationRequestIdParameter()
		{
		}

		public TenantRelocationRequestIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public TenantRelocationRequestIdParameter(TenantRelocationRequest tenant) : this(tenant.Name)
		{
		}

		public TenantRelocationRequestIdParameter(OrganizationId organizationId) : base(organizationId.OrganizationalUnit)
		{
		}

		protected override QueryFilter AdditionalQueryFilter
		{
			get
			{
				return QueryFilter.AndTogether(new QueryFilter[]
				{
					base.AdditionalQueryFilter,
					TenantRelocationRequest.TenantRelocationRequestFilter,
					new ExistsFilter(ADOrganizationalUnitSchema.ConfigurationUnitLink)
				});
			}
		}

		public static TenantRelocationRequestIdParameter Parse(string identity)
		{
			return new TenantRelocationRequestIdParameter(identity);
		}

		internal OrganizationId ResolveOrganizationId()
		{
			if (string.IsNullOrEmpty(base.RawIdentity) || string.IsNullOrEmpty(base.RawIdentity.Trim()))
			{
				throw new ArgumentException("Cannot resolve tenant name - RawIdentity is null or empty");
			}
			ExchangeConfigurationUnit exchangeConfigurationUnit = null;
			try
			{
				PartitionId partitionId = ADAccountPartitionLocator.GetPartitionIdByAcceptedDomainName(base.RawIdentity);
				ADSessionSettings adsessionSettings = ADSessionSettings.FromAllTenantsPartitionId(partitionId);
				adsessionSettings.TenantConsistencyMode = TenantConsistencyMode.IncludeRetiredTenants;
				ITenantConfigurationSession tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.PartiallyConsistent, adsessionSettings, 142, "ResolveOrganizationId", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\IdentityParameter\\TenantRelocationRequestIdParameter.cs");
				exchangeConfigurationUnit = tenantConfigurationSession.GetExchangeConfigurationUnitByName(base.RawIdentity);
			}
			catch (CannotResolveTenantNameException)
			{
			}
			if (exchangeConfigurationUnit == null)
			{
				foreach (PartitionId partitionId2 in ADAccountPartitionLocator.GetAllAccountPartitionIds())
				{
					ADSessionSettings adsessionSettings2 = ADSessionSettings.FromAllTenantsPartitionId(partitionId2);
					adsessionSettings2.TenantConsistencyMode = TenantConsistencyMode.IncludeRetiredTenants;
					ITenantConfigurationSession tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.PartiallyConsistent, adsessionSettings2, 160, "ResolveOrganizationId", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\IdentityParameter\\TenantRelocationRequestIdParameter.cs");
					exchangeConfigurationUnit = tenantConfigurationSession.GetExchangeConfigurationUnitByName(base.RawIdentity);
					if (exchangeConfigurationUnit != null)
					{
						break;
					}
				}
			}
			if (exchangeConfigurationUnit != null && !string.IsNullOrEmpty(exchangeConfigurationUnit.RelocationSourceForestRaw))
			{
				PartitionId partitionId = new PartitionId(exchangeConfigurationUnit.RelocationSourceForestRaw);
				ADSessionSettings adsessionSettings = ADSessionSettings.FromAllTenantsPartitionId(partitionId);
				adsessionSettings.TenantConsistencyMode = TenantConsistencyMode.IncludeRetiredTenants;
				ITenantConfigurationSession tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.PartiallyConsistent, adsessionSettings, 175, "ResolveOrganizationId", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\IdentityParameter\\TenantRelocationRequestIdParameter.cs");
				exchangeConfigurationUnit = tenantConfigurationSession.GetExchangeConfigurationUnitByName(base.RawIdentity);
			}
			if (exchangeConfigurationUnit != null)
			{
				return exchangeConfigurationUnit.OrganizationId;
			}
			throw new CannotResolveTenantNameException(DirectoryStrings.CannotResolveTenantRelocationRequestIdentity(base.RawIdentity));
		}

		internal override IEnumerable<T> GetObjects<T>(ADObjectId rootId, IDirectorySession session, IDirectorySession subTreeSession, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			TaskLogger.LogEnter();
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (!(session is IConfigurationSession))
			{
				throw new ArgumentException(Strings.ErrorInvalidType(session.GetType().Name), "type");
			}
			notFoundReason = null;
			EnumerableWrapper<T> result = EnumerableWrapper<T>.Empty;
			if (this.IsWildcardDefined(base.RawIdentity))
			{
				notFoundReason = new LocalizedString?(Strings.ErrorOrganizationWildcard);
				return result;
			}
			OrganizationId organizationId = this.ResolveOrganizationId();
			if (!OrganizationId.ForestWideOrgId.Equals(organizationId))
			{
				ADSessionSettings adsessionSettings = ADSessionSettings.FromCustomScopeSet(ScopeSet.ResolveUnderScope(organizationId, session.SessionSettings.ScopeSet), session.SessionSettings.RootOrgId, organizationId, session.SessionSettings.ExecutingUserOrganizationId, false);
				adsessionSettings.TenantConsistencyMode = TenantConsistencyMode.IncludeRetiredTenants;
				ITenantConfigurationSession tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(session.DomainController, session.ReadOnly, session.ConsistencyMode, session.NetworkCredential, adsessionSettings, 257, "GetObjects", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\IdentityParameter\\TenantRelocationRequestIdParameter.cs");
				tenantConfigurationSession.UseConfigNC = session.UseConfigNC;
				tenantConfigurationSession.UseGlobalCatalog = session.UseGlobalCatalog;
				if (typeof(TenantRelocationRequest).Equals(typeof(T)) && organizationId.ConfigurationUnit != null)
				{
					List<TenantRelocationRequest> list = new List<TenantRelocationRequest>();
					TenantRelocationRequest[] array = tenantConfigurationSession.Find<TenantRelocationRequest>(organizationId.ConfigurationUnit, QueryScope.SubTree, TenantRelocationRequest.TenantRelocationRequestFilter, null, 1);
					if (array != null && array.Length > 0)
					{
						list.Add(array[0]);
						result = EnumerableWrapper<T>.GetWrapper((IEnumerable<T>)list, this.GetEnumerableFilter<T>());
					}
				}
			}
			TaskLogger.LogExit();
			return result;
		}
	}
}
