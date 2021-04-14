using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public sealed class OrganizationIdParameter : ADIdParameter
	{
		public OrganizationIdParameter(string identity) : base(identity)
		{
		}

		public OrganizationIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public OrganizationIdParameter()
		{
		}

		public OrganizationIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public OrganizationIdParameter(TenantOrganizationPresentationObject tenant) : base(tenant.DistinguishedName)
		{
		}

		public OrganizationIdParameter(OrganizationId organizationId) : base(organizationId.OrganizationalUnit)
		{
		}

		protected override QueryFilter AdditionalQueryFilter
		{
			get
			{
				return QueryFilter.AndTogether(new QueryFilter[]
				{
					base.AdditionalQueryFilter,
					QueryFilter.OrTogether(new QueryFilter[]
					{
						new ExistsFilter(ADOrganizationalUnitSchema.ConfigurationUnitLink),
						new ExistsFilter(ExchangeConfigurationUnitSchema.OrganizationalUnitLink)
					})
				});
			}
		}

		internal PartitionId AccountPartition { get; set; }

		public static OrganizationIdParameter Parse(string identity)
		{
			return new OrganizationIdParameter(identity);
		}

		internal static string GetHierarchicalIdentityFromDN(string dnString)
		{
			string distinguishedName = ADObjectId.ParseDnOrGuid(dnString).DistinguishedName;
			string[] array = DNConvertor.SplitDistinguishedName(distinguishedName, ',');
			bool flag = false;
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = array.Length - 1; i >= 0; i--)
			{
				if (!flag)
				{
					if (array[i].StartsWith("OU=Microsoft Exchange Hosted Organizations", StringComparison.OrdinalIgnoreCase))
					{
						flag = true;
					}
				}
				else
				{
					if (array[i].Length <= 3 || !array[i].StartsWith("OU="))
					{
						throw new FormatException(Strings.ErrorInvalidOrganizationalUnitDNFormat(dnString));
					}
					if (stringBuilder.Length != 0)
					{
						stringBuilder.Append("\\");
					}
					stringBuilder.Append(array[i].Substring(3));
				}
			}
			if (!flag)
			{
				throw new FormatException(Strings.ErrorInvalidOrganizationalUnitDNFormat(dnString));
			}
			return stringBuilder.ToString();
		}

		private OrganizationId ResolveOrganizationId(OrganizationId originalOrgId)
		{
			if (!OrganizationId.ForestWideOrgId.Equals(originalOrgId) || !base.IsMultitenancyEnabled())
			{
				return originalOrgId;
			}
			string text = null;
			ADObjectId adobjectId;
			if (base.InternalADObjectId != null)
			{
				text = base.InternalADObjectId.DistinguishedName;
			}
			else if (ADIdParameter.TryResolveCanonicalName(base.RawIdentity, out adobjectId))
			{
				text = adobjectId.DistinguishedName;
			}
			string orgName = base.RawIdentity;
			if (!string.IsNullOrEmpty(text))
			{
				try
				{
					orgName = OrganizationIdParameter.GetHierarchicalIdentityFromDN(text);
				}
				catch (FormatException)
				{
				}
			}
			return base.GetOrganizationId(originalOrgId, orgName);
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
				throw new ArgumentException(Strings.ErrorInvalidType(session.GetType().Name), "session");
			}
			if (((typeof(T) == typeof(ADOrganizationalUnit) && session.UseConfigNC) || (typeof(T) == typeof(ExchangeConfigurationUnit) && !session.UseConfigNC)) && !Environment.StackTrace.Contains("Microsoft.Exchange.Management.Deployment.OrganizationTaskHelper.GetExchangeConfigUnitFromOrganizationId"))
			{
				throw new ArgumentException("Session is using the wrong Naming Context for the desired search");
			}
			notFoundReason = null;
			EnumerableWrapper<T> result = EnumerableWrapper<T>.Empty;
			if (base.IsMultitenancyEnabled())
			{
				if (this.IsWildcardDefined(base.RawIdentity))
				{
					if (null == this.AccountPartition)
					{
						notFoundReason = new LocalizedString?(Strings.ErrorOrganizationWildcard);
						return result;
					}
					IEnumerable<ExchangeConfigurationUnit> configurationUnits = this.GetConfigurationUnits((IConfigurationSession)session, base.RawIdentity);
					return EnumerableWrapper<T>.GetWrapper((IEnumerable<T>)configurationUnits, this.GetEnumerableFilter<T>());
				}
				else
				{
					OrganizationId organizationId = this.ResolveOrganizationId(session.SessionSettings.CurrentOrganizationId);
					if (!OrganizationId.ForestWideOrgId.Equals(organizationId))
					{
						ADSessionSettings adsessionSettings = ADSessionSettings.FromCustomScopeSet(ScopeSet.ResolveUnderScope(organizationId, session.SessionSettings.ScopeSet), session.SessionSettings.RootOrgId, organizationId, session.SessionSettings.ExecutingUserOrganizationId, true);
						adsessionSettings.TenantConsistencyMode = TenantConsistencyMode.IncludeRetiredTenants;
						bool flag = TaskHelper.ShouldPassDomainControllerToSession(session.DomainController, adsessionSettings);
						ITenantConfigurationSession tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(flag ? session.DomainController : null, session.ReadOnly, session.ConsistencyMode, flag ? session.NetworkCredential : null, adsessionSettings, 314, "GetObjects", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\IdentityParameter\\organizationidparameter.cs");
						tenantConfigurationSession.UseConfigNC = session.UseConfigNC;
						tenantConfigurationSession.UseGlobalCatalog = session.UseGlobalCatalog;
						if (typeof(ExchangeConfigurationUnit) == typeof(T) && organizationId.ConfigurationUnit != null)
						{
							List<ExchangeConfigurationUnit> list = new List<ExchangeConfigurationUnit>();
							ExchangeConfigurationUnit exchangeConfigurationUnit = tenantConfigurationSession.Read<ExchangeConfigurationUnit>(organizationId.ConfigurationUnit);
							if (exchangeConfigurationUnit != null)
							{
								list.Add(exchangeConfigurationUnit);
								result = EnumerableWrapper<T>.GetWrapper((IEnumerable<T>)list, this.GetEnumerableFilter<T>());
							}
						}
						else if (organizationId.OrganizationalUnit != null)
						{
							List<ADOrganizationalUnit> list2 = new List<ADOrganizationalUnit>();
							ADOrganizationalUnit adorganizationalUnit = tenantConfigurationSession.Read<ADOrganizationalUnit>(organizationId.OrganizationalUnit);
							if (adorganizationalUnit != null)
							{
								list2.Add(adorganizationalUnit);
								result = EnumerableWrapper<T>.GetWrapper((IEnumerable<T>)list2, this.GetEnumerableFilter<T>());
							}
						}
					}
				}
			}
			TaskLogger.LogExit();
			return result;
		}

		private IEnumerable<ExchangeConfigurationUnit> GetConfigurationUnits(IConfigurationSession tenantConfigurationSession, string organizationFilter)
		{
			if (string.IsNullOrEmpty(organizationFilter))
			{
				throw new ArgumentException("Filter must contain a non-empty value", "organizationFilter");
			}
			QueryFilter filter = base.CreateWildcardFilter(ADObjectSchema.Name, organizationFilter);
			ADObjectId configurationUnitsRoot = ADSession.GetConfigurationUnitsRoot(this.AccountPartition.ForestFQDN);
			ADPagedReader<ADRawEntry> source = tenantConfigurationSession.FindPagedADRawEntry(configurationUnitsRoot, QueryScope.OneLevel, filter, null, 0, new ADPropertyDefinition[]
			{
				ADObjectSchema.Id
			});
			IEnumerable<ADObjectId> source2 = from tenant in source
			select tenant.Id.GetChildId("Configuration");
			Result<ExchangeConfigurationUnit>[] source3 = tenantConfigurationSession.ReadMultiple<ExchangeConfigurationUnit>(source2.ToArray<ADObjectId>());
			return from result in source3
			select result.Data into cu
			where null != cu
			select cu;
		}
	}
}
