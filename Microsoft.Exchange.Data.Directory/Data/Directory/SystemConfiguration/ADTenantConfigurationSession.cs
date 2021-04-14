using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class ADTenantConfigurationSession : ADConfigurationSession, ITenantConfigurationSession, IConfigurationSession, IDirectorySession, IConfigDataProvider
	{
		public ADTenantConfigurationSession(ConsistencyMode consistencyMode, ADSessionSettings sessionSettings) : base(true, true, consistencyMode, null, sessionSettings)
		{
		}

		public ADTenantConfigurationSession(bool readOnly, ConsistencyMode consistencyMode, ADSessionSettings sessionSettings) : base(true, readOnly, consistencyMode, null, sessionSettings)
		{
		}

		public ADTenantConfigurationSession(string domainController, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings) : base(true, readOnly, consistencyMode, networkCredential, sessionSettings)
		{
			base.DomainController = domainController;
		}

		public ADTenantConfigurationSession(string domainController, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings, ConfigScopes configScope) : this(domainController, readOnly, consistencyMode, networkCredential, sessionSettings)
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

		public override ADObjectId DeletedObjectsContainer
		{
			get
			{
				ADObjectId namingContextId = ADSession.IsTenantConfigInDomainNC(base.SessionSettings.GetAccountOrResourceForestFqdn()) ? base.GetDomainNamingContext() : base.GetConfigurationNamingContext();
				return ADSession.GetDeletedObjectsContainer(namingContextId);
			}
		}

		public AcceptedDomain[] FindAllAcceptedDomainsInOrg(ADObjectId organizationCU)
		{
			ADPagedReader<AcceptedDomain> adpagedReader = base.FindPaged<AcceptedDomain>(organizationCU, QueryScope.SubTree, null, null, 0);
			List<AcceptedDomain> list = new List<AcceptedDomain>();
			foreach (AcceptedDomain item in adpagedReader)
			{
				list.Add(item);
			}
			return list.ToArray();
		}

		public ADRawEntry[] FindDeletedADRawEntryByUsnRange(ADObjectId lastKnownParentId, long startUsn, int sizeLimit, IEnumerable<PropertyDefinition> properties)
		{
			if (sizeLimit > ADDataSession.RangedValueDefaultPageSize)
			{
				throw new ArgumentOutOfRangeException("sizeLimit");
			}
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ADRecipientSchema.UsnChanged, startUsn),
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.OrganizationalUnitRoot, lastKnownParentId)
			});
			ADObjectId rootId = ADSession.IsTenantConfigInDomainNC(base.SessionSettings.GetAccountOrResourceForestFqdn()) ? ADSession.GetDeletedObjectsContainer(lastKnownParentId.DomainId) : ADSession.GetDeletedObjectsContainer(base.ConfigurationNamingContext);
			return base.Find<ADRawEntry>(rootId, QueryScope.OneLevel, filter, ADDataSession.SortByUsn, sizeLimit, properties, true);
		}

		public ExchangeConfigurationUnit[] FindSharedConfiguration(SharedConfigurationInfo sharedConfigInfo, bool enabledSharedOrgOnly)
		{
			if (!base.SessionSettings.IsGlobal && base.ConfigScope != ConfigScopes.AllTenants)
			{
				throw new NotSupportedException("FindEnabledSharedConfiguration cannot be invoked on non Global sessions");
			}
			if (sharedConfigInfo == null)
			{
				throw new ArgumentNullException("sharedConfigInfo");
			}
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, OrganizationSchema.SharedConfigurationInfo, sharedConfigInfo),
				new ComparisonFilter(ComparisonOperator.Equal, ExchangeConfigurationUnitSchema.OrganizationStatus, OrganizationStatus.Active),
				new ComparisonFilter(ComparisonOperator.Equal, OrganizationSchema.ImmutableConfiguration, true),
				new ComparisonFilter(ComparisonOperator.Equal, OrganizationSchema.EnableAsSharedConfiguration, enabledSharedOrgOnly)
			});
			return base.Find<ExchangeConfigurationUnit>(null, QueryScope.SubTree, filter, null, 0);
		}

		public ExchangeConfigurationUnit[] FindSharedConfigurationByOrganizationId(OrganizationId tinyTenantId)
		{
			if (!base.SessionSettings.IsGlobal && base.ConfigScope != ConfigScopes.AllTenants)
			{
				throw new NotSupportedException("FindSharedConfigurationByCU cannot be invoked on non Global sessions");
			}
			if (tinyTenantId == null)
			{
				throw new ArgumentNullException("tinyTenantId");
			}
			if (tinyTenantId == OrganizationId.ForestWideOrgId)
			{
				throw new ArgumentException("tinyTenantId cannot be ForestWideOrgId");
			}
			return base.Find<ExchangeConfigurationUnit>(null, QueryScope.SubTree, new ComparisonFilter(ComparisonOperator.Equal, OrganizationSchema.SupportedSharedConfigurationsBL, tinyTenantId.ConfigurationUnit), null, 0);
		}

		public T GetDefaultFilteringConfiguration<T>() where T : ADConfigurationObject, new()
		{
			QueryFilter filter;
			if (typeof(T) == typeof(HostedSpamFilterConfig))
			{
				filter = new ComparisonFilter(ComparisonOperator.Equal, HostedSpamFilterConfigSchema.IsDefault, true);
			}
			else if (typeof(T) == typeof(HostedContentFilterPolicy))
			{
				filter = new ComparisonFilter(ComparisonOperator.Equal, HostedContentFilterPolicySchema.IsDefault, true);
			}
			else if (typeof(T) == typeof(MalwareFilterPolicy))
			{
				filter = new ComparisonFilter(ComparisonOperator.Equal, MalwareFilterPolicySchema.IsDefault, true);
			}
			else if (typeof(T) == typeof(HostedConnectionFilterPolicy))
			{
				filter = new ComparisonFilter(ComparisonOperator.Equal, HostedConnectionFilterPolicySchema.IsDefault, true);
			}
			else
			{
				if (!(typeof(T) == typeof(HostedOutboundSpamFilterPolicy)))
				{
					throw new NotSupportedException(typeof(T).FullName);
				}
				filter = null;
			}
			T[] array = base.Find<T>(null, QueryScope.SubTree, filter, null, 1);
			if (array == null || array.Length == 0)
			{
				return default(T);
			}
			return array[0];
		}

		public bool IsTenantLockedOut()
		{
			if (!base.SessionSettings.IsTenantScoped)
			{
				throw new InvalidOperationException("IsTenantLockedOut() is only supported for tenant-scoped sessions");
			}
			return null != base.Find<ExchangeConfigurationUnit>(null, QueryScope.SubTree, new ComparisonFilter(ComparisonOperator.Equal, ExchangeConfigurationUnitSchema.OrganizationStatus, OrganizationStatus.LockedOut), null, 1).FirstOrDefault<ExchangeConfigurationUnit>();
		}

		public ExchangeConfigurationUnit GetExchangeConfigurationUnitByExternalId(string externalDirectoryOrganizationId)
		{
			ComparisonFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ExchangeConfigurationUnitSchema.ExternalDirectoryOrganizationId, externalDirectoryOrganizationId);
			return base.FindPaged<ExchangeConfigurationUnit>(null, QueryScope.SubTree, filter, null, 1).FirstOrDefault<ExchangeConfigurationUnit>();
		}

		public ExchangeConfigurationUnit GetExchangeConfigurationUnitByExternalId(Guid externalDirectoryOrganizationId)
		{
			return this.GetExchangeConfigurationUnitByExternalId(externalDirectoryOrganizationId.ToString());
		}

		public ExchangeConfigurationUnit GetExchangeConfigurationUnitByName(string organizationName)
		{
			ADObjectId exchangeConfigurationUnitIdByName = this.GetExchangeConfigurationUnitIdByName(organizationName);
			return base.Read<ExchangeConfigurationUnit>(exchangeConfigurationUnitIdByName);
		}

		public ADObjectId GetExchangeConfigurationUnitIdByName(string organizationName)
		{
			ADObjectId configurationUnitsRoot = base.GetConfigurationUnitsRoot();
			return configurationUnitsRoot.GetChildId("CN", organizationName).GetChildId("CN", "Configuration");
		}

		public ExchangeConfigurationUnit GetExchangeConfigurationUnitByUserNetID(string userNetID)
		{
			ITenantRecipientSession tenantRecipientSession = DirectorySessionFactory.Default.CreateTenantRecipientSession(true, ConsistencyMode.IgnoreInvalid, base.SessionSettings, 352, "GetExchangeConfigurationUnitByUserNetID", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SystemConfiguration\\ADTenantConfigurationSession.cs");
			ADRawEntry adrawEntry = tenantRecipientSession.FindByNetID(userNetID, new ADPropertyDefinition[]
			{
				ADObjectSchema.OrganizationalUnitRoot
			}).FirstOrDefault<ADRawEntry>();
			if (adrawEntry == null)
			{
				return null;
			}
			ADObjectId adobjectId = (ADObjectId)adrawEntry[ADObjectSchema.OrganizationalUnitRoot];
			return this.GetExchangeConfigurationUnitByName(adobjectId.Name);
		}

		public ExchangeConfigurationUnit[] FindAllOpenConfigurationUnits(bool excludeFull)
		{
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ExchangeConfigurationUnitSchema.ResellerId, "MSOnline.BPOS_Unmanaged_Hydrated");
			return base.Find<ExchangeConfigurationUnit>(null, QueryScope.SubTree, filter, null, 0, null, false);
		}

		public ExchangeConfigurationUnit GetExchangeConfigurationUnitByNameOrAcceptedDomain(string organizationName)
		{
			if (Datacenter.IsMultiTenancyEnabled())
			{
				ExchangeConfigurationUnit exchangeConfigurationUnitByName = this.GetExchangeConfigurationUnitByName(organizationName);
				if (exchangeConfigurationUnitByName != null)
				{
					return exchangeConfigurationUnitByName;
				}
			}
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, AcceptedDomainSchema.DomainName, organizationName),
				new ComparisonFilter(ComparisonOperator.NotEqual, AcceptedDomainSchema.AcceptedDomainType, AcceptedDomainType.ExternalRelay)
			});
			AcceptedDomain[] array = base.Find<AcceptedDomain>(null, QueryScope.SubTree, filter, null, 1);
			if (array == null || array.Length == 0)
			{
				return null;
			}
			ADObjectId entryId = array[0].Id.AncestorDN(3);
			return base.Read<ExchangeConfigurationUnit>(entryId);
		}

		public OrganizationId GetOrganizationIdFromOrgNameOrAcceptedDomain(string domainName)
		{
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, AcceptedDomainSchema.DomainName, domainName),
				new ComparisonFilter(ComparisonOperator.NotEqual, AcceptedDomainSchema.AcceptedDomainType, AcceptedDomainType.ExternalRelay)
			});
			ADRawEntry[] array = base.FindADRawEntryWithDefaultFilters<AcceptedDomain>(null, QueryScope.SubTree, filter, null, 1, new PropertyDefinition[]
			{
				ADObjectSchema.OrganizationId
			});
			if (array == null || array.Count<ADRawEntry>() < 1)
			{
				return null;
			}
			return (OrganizationId)array[0][ADObjectSchema.OrganizationId];
		}

		public OrganizationId GetOrganizationIdFromExternalDirectoryOrgId(Guid externalDirectoryOrgId)
		{
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ExchangeConfigurationUnitSchema.ExternalDirectoryOrganizationId, externalDirectoryOrgId);
			ADRawEntry[] array = base.FindADRawEntryWithDefaultFilters<ExchangeConfigurationUnit>(null, QueryScope.SubTree, filter, null, 1, new PropertyDefinition[]
			{
				ExchangeConfigurationUnitSchema.OrganizationId
			});
			if (array == null || array.Count<ADRawEntry>() < 1)
			{
				return null;
			}
			return (OrganizationId)array[0][ADObjectSchema.OrganizationId];
		}

		public MsoTenantCookieContainer GetMsoTenantCookieContainer(Guid contextId)
		{
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, MsoTenantCookieContainerSchema.ExternalDirectoryOrganizationId, contextId.ToString());
			MsoTenantCookieContainer[] array = base.Find<MsoTenantCookieContainer>(null, QueryScope.SubTree, filter, null, 1);
			if (array == null || array.Length == 0)
			{
				return null;
			}
			return array[0];
		}

		public Result<ADRawEntry>[] ReadMultipleOrganizationProperties(ADObjectId[] organizationOUIds, PropertyDefinition[] properties)
		{
			if (organizationOUIds == null)
			{
				throw new ArgumentNullException("organizationOUIds");
			}
			if (organizationOUIds.Length == 0)
			{
				return new Result<ADRawEntry>[0];
			}
			PropertyDefinition[] array;
			if (properties == null)
			{
				array = new ADPropertyDefinition[]
				{
					ADObjectSchema.OrganizationalUnitRoot
				};
			}
			else
			{
				array = new PropertyDefinition[properties.Length + 1];
				properties.CopyTo(array, 0);
				array[array.Length - 1] = ADObjectSchema.OrganizationalUnitRoot;
			}
			return base.ReadMultiple<ADObjectId, ADRawEntry>(organizationOUIds, (ADObjectId ouId) => new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.OrganizationalUnitRoot, ouId),
				new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectClass, ExchangeConfigurationUnit.MostDerivedClass)
			}), delegate(Hashtable hash, ADRawEntry entry)
			{
				ADObjectId adobjectId = (ADObjectId)entry[ADObjectSchema.OrganizationalUnitRoot];
				hash.Add(adobjectId.DistinguishedName, new Result<ADRawEntry>(entry, null));
				hash.Add(adobjectId.ObjectGuid.ToString(), new Result<ADRawEntry>(entry, null));
			}, new ADDataSession.HashLookup<ADObjectId, ADRawEntry>(ADRecipientObjectSession.ADObjectIdHashLookup<ADRawEntry>), array, true);
		}
	}
}
