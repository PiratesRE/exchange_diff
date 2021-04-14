using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	[Serializable]
	internal class FfoTenantConfigurationSession : FfoConfigurationSession, ITenantConfigurationSession, IConfigurationSession, IDirectorySession, IConfigDataProvider
	{
		public FfoTenantConfigurationSession(ConsistencyMode consistencyMode, ADSessionSettings sessionSettings) : base(true, true, consistencyMode, null, sessionSettings)
		{
		}

		public FfoTenantConfigurationSession(bool readOnly, ConsistencyMode consistencyMode, ADSessionSettings sessionSettings) : base(true, readOnly, consistencyMode, null, sessionSettings)
		{
		}

		public FfoTenantConfigurationSession(string domainController, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings) : base(true, readOnly, consistencyMode, networkCredential, sessionSettings)
		{
			base.DomainController = domainController;
		}

		public FfoTenantConfigurationSession(string domainController, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings, ConfigScopes configScope) : this(domainController, readOnly, consistencyMode, networkCredential, sessionSettings)
		{
			if (ConfigScopes.TenantSubTree != configScope)
			{
				throw new NotSupportedException("Only ConfigScopes.TenantSubTree is supported by this constructor");
			}
			if (ConfigScopes.TenantSubTree == configScope)
			{
				base.ConfigScope = configScope;
			}
		}

		public FfoTenantConfigurationSession(ADObjectId tenantId) : base(tenantId)
		{
		}

		AcceptedDomain[] ITenantConfigurationSession.FindAllAcceptedDomainsInOrg(ADObjectId organizationCU)
		{
			ComparisonFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.OrganizationalUnitRoot, organizationCU);
			return base.DataProvider.Find<AcceptedDomain>(filter, null, false, null).Cast<AcceptedDomain>().ToArray<AcceptedDomain>();
		}

		ExchangeConfigurationUnit[] ITenantConfigurationSession.FindSharedConfiguration(SharedConfigurationInfo sharedConfigInfo, bool enabledSharedOrgOnly)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return new ExchangeConfigurationUnit[0];
		}

		ExchangeConfigurationUnit[] ITenantConfigurationSession.FindSharedConfigurationByOrganizationId(OrganizationId tinyTenantId)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return new ExchangeConfigurationUnit[0];
		}

		public ADRawEntry[] FindDeletedADRawEntryByUsnRange(ADObjectId lastKnownParentId, long startUsn, int sizeLimit, IEnumerable<PropertyDefinition> properties)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return new ADRawEntry[0];
		}

		ExchangeConfigurationUnit ITenantConfigurationSession.GetExchangeConfigurationUnitByExternalId(string externalDirectoryOrganizationId)
		{
			IEnumerable<FfoTenant> source = base.FindTenantObject<FfoTenant>(new object[]
			{
				ADObjectSchema.OrganizationalUnitRoot,
				new ADObjectId(new Guid(externalDirectoryOrganizationId))
			}).Cast<FfoTenant>();
			return source.Select(new Func<FfoTenant, ExchangeConfigurationUnit>(FfoConfigurationSession.GetExchangeConfigurationUnit)).FirstOrDefault<ExchangeConfigurationUnit>();
		}

		ExchangeConfigurationUnit ITenantConfigurationSession.GetExchangeConfigurationUnitByExternalId(Guid externalDirectoryOrganizationId)
		{
			return ((ITenantConfigurationSession)this).GetExchangeConfigurationUnitByExternalId(externalDirectoryOrganizationId.ToString());
		}

		ExchangeConfigurationUnit ITenantConfigurationSession.GetExchangeConfigurationUnitByName(string organizationName)
		{
			ComparisonFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, organizationName);
			FfoTenant ffoTenant = (FfoTenant)base.DataProvider.Find<FfoTenant>(filter, null, false, null).FirstOrDefault<IConfigurable>();
			if (ffoTenant == null)
			{
				return null;
			}
			return FfoConfigurationSession.GetExchangeConfigurationUnit(ffoTenant);
		}

		ADObjectId ITenantConfigurationSession.GetExchangeConfigurationUnitIdByName(string organizationName)
		{
			return ((ITenantConfigurationSession)this).GetExchangeConfigurationUnitByName(organizationName).Id;
		}

		ExchangeConfigurationUnit[] ITenantConfigurationSession.FindAllOpenConfigurationUnits(bool excludeFull)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return null;
		}

		ExchangeConfigurationUnit ITenantConfigurationSession.GetExchangeConfigurationUnitByNameOrAcceptedDomain(string organizationName)
		{
			return ((ITenantConfigurationSession)this).GetExchangeConfigurationUnitByName(organizationName);
		}

		OrganizationId ITenantConfigurationSession.GetOrganizationIdFromOrgNameOrAcceptedDomain(string domainName)
		{
			return ((ITenantConfigurationSession)this).GetOrganizationIdFromOrgNameOrAcceptedDomain(domainName);
		}

		OrganizationId ITenantConfigurationSession.GetOrganizationIdFromExternalDirectoryOrgId(Guid externalDirectoryOrgId)
		{
			return ((ITenantConfigurationSession)this).GetOrganizationIdFromExternalDirectoryOrgId(externalDirectoryOrgId);
		}

		ExchangeConfigurationUnit ITenantConfigurationSession.GetExchangeConfigurationUnitByUserNetID(string userNetID)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return null;
		}

		MsoTenantCookieContainer ITenantConfigurationSession.GetMsoTenantCookieContainer(Guid contextId)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return null;
		}

		Result<ADRawEntry>[] ITenantConfigurationSession.ReadMultipleOrganizationProperties(ADObjectId[] organizationOUIds, PropertyDefinition[] properties)
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return null;
		}

		T ITenantConfigurationSession.GetDefaultFilteringConfiguration<T>()
		{
			string name;
			int num;
			if (typeof(T) == typeof(MalwareFilterPolicy))
			{
				name = MalwareFilterPolicySchema.MalwareFilterPolicyFlags.Name;
				num = 512;
			}
			else if (typeof(T) == typeof(HostedContentFilterPolicy))
			{
				name = HostedContentFilterPolicySchema.SpamFilteringFlags.Name;
				num = 64;
			}
			else if (typeof(T) == typeof(HostedConnectionFilterPolicy))
			{
				name = HostedConnectionFilterPolicySchema.ConnectionFilterFlags.Name;
				num = 1;
			}
			else
			{
				if (!(typeof(T) == typeof(HostedOutboundSpamFilterPolicy)))
				{
					throw new NotSupportedException(string.Format("Type {0} is not supporeted", typeof(T).Name));
				}
				name = HostedOutboundSpamFilterPolicySchema.OutboundSpamFilterFlags.Name;
				num = 0;
			}
			QueryFilter queryFilter = new ComparisonFilter(ComparisonOperator.Equal, DalHelper.PropertyNameProp, name);
			QueryFilter queryFilter2 = new ComparisonFilter(ComparisonOperator.Equal, DalHelper.PropertyValueIntegerProp, num);
			QueryFilter queryFilter3 = new ComparisonFilter(ComparisonOperator.Equal, DalHelper.PropertyValueIntegerMaskProp, num);
			QueryFilter filter = QueryFilter.AndTogether(new QueryFilter[]
			{
				queryFilter,
				queryFilter2,
				queryFilter3
			});
			return (T)((object)((IConfigDataProvider)this).Find<T>(filter, null, true, null).FirstOrDefault<IConfigurable>());
		}

		public bool IsTenantLockedOut()
		{
			FfoDirectorySession.LogNotSupportedInFFO(null);
			return false;
		}

		public void UndeleteTenant(Guid tenantId, DateTime deletedDatetime)
		{
			if (tenantId == Guid.Empty)
			{
				throw new ArgumentException("The tenantId must not be empty.");
			}
			base.DataProvider.Save(new TenantUndeleteRequest
			{
				TenantId = tenantId,
				DeletedDatetime = deletedDatetime
			});
		}
	}
}
