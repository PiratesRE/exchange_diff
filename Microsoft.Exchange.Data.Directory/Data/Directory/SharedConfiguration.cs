using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.ProvisioningCache;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory
{
	internal class SharedConfiguration
	{
		private SharedConfiguration(OrganizationId tinyTenantId, ExchangeConfigurationUnit sharedConfigurationCU)
		{
			this.tinyTenantId = tinyTenantId;
			this.sharedConfigurationCU = sharedConfigurationCU;
		}

		public static string SCTNamePrefix
		{
			get
			{
				return SharedConfiguration.sctNamePrefix;
			}
		}

		public static int SctNameMaxLength
		{
			get
			{
				return SharedConfiguration.sctNameMaxLength;
			}
		}

		public OrganizationId TinyTenantId
		{
			get
			{
				return this.tinyTenantId;
			}
		}

		public ExchangeConfigurationUnit TenantCU
		{
			get
			{
				this.LoadTinyTenantCUIfNecessary();
				return this.tinyTenantCU;
			}
		}

		public OrganizationId SharedConfigId
		{
			get
			{
				return this.sharedConfigurationCU.OrganizationId;
			}
		}

		public ExchangeConfigurationUnit SharedConfigurationCU
		{
			get
			{
				return this.sharedConfigurationCU;
			}
		}

		public static ExchangeConfigurationUnit[] LoadSharedConfigurationsSorted(OrganizationId orgId)
		{
			if (orgId.Equals(OrganizationId.ForestWideOrgId))
			{
				return null;
			}
			ITenantConfigurationSession tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromAllTenantsPartitionId(orgId.PartitionId), 151, "LoadSharedConfigurationsSorted", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SharedConfiguration.cs");
			ExchangeConfigurationUnit[] array = tenantConfigurationSession.FindSharedConfigurationByOrganizationId(orgId);
			if (array.Length > 1)
			{
				Array.Sort<ExchangeConfigurationUnit>(array, new Comparison<ExchangeConfigurationUnit>(SharedConfiguration.CompareBySharedConfigurationInfo));
			}
			return array;
		}

		public static SharedConfiguration GetSharedConfiguration(OrganizationId orgId)
		{
			ExchangeConfigurationUnit[] array = SharedConfiguration.LoadSharedConfigurationsSorted(orgId);
			if (array == null)
			{
				return null;
			}
			ExchangeConfigurationUnit exchangeConfigurationUnit = null;
			if (array.Length == 1)
			{
				exchangeConfigurationUnit = array[0];
			}
			else if (array.Length > 1)
			{
				ExchangeConfigurationUnit exchangeConfigurationUnit2 = null;
				int i = 0;
				while (i < array.Length)
				{
					exchangeConfigurationUnit = array[i];
					int num = ServerVersion.Compare(exchangeConfigurationUnit.SharedConfigurationInfo.CurrentVersion, ServerVersion.InstalledVersion);
					if (num == 0)
					{
						break;
					}
					if (num > 0)
					{
						if (exchangeConfigurationUnit.SharedConfigurationInfo.CurrentVersion.Major > ServerVersion.InstalledVersion.Major && exchangeConfigurationUnit2 != null)
						{
							exchangeConfigurationUnit = exchangeConfigurationUnit2;
							break;
						}
						break;
					}
					else
					{
						exchangeConfigurationUnit2 = array[i];
						i++;
					}
				}
			}
			if (exchangeConfigurationUnit != null)
			{
				return new SharedConfiguration(orgId, exchangeConfigurationUnit);
			}
			return null;
		}

		internal static int CompareBySharedConfigurationInfo(ExchangeConfigurationUnit x, ExchangeConfigurationUnit y)
		{
			int result;
			if (x == null)
			{
				if (y == null)
				{
					result = 0;
				}
				else
				{
					result = -1;
				}
			}
			else if (y == null)
			{
				result = 1;
			}
			else
			{
				result = ServerVersion.Compare(x.SharedConfigurationInfo.CurrentVersion, y.SharedConfigurationInfo.CurrentVersion);
			}
			return result;
		}

		public static bool IsSharedConfiguration(OrganizationId orgId)
		{
			if (OrganizationId.ForestWideOrgId.Equals(orgId))
			{
				return false;
			}
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.FullyConsistent, ADSessionSettings.FromAllTenantsPartitionId(orgId.PartitionId), 279, "IsSharedConfiguration", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SharedConfiguration.cs");
			ExchangeConfigurationUnit[] array = tenantOrTopologyConfigurationSession.Find<ExchangeConfigurationUnit>(orgId.ConfigurationUnit, QueryScope.Base, new ExistsFilter(OrganizationSchema.SharedConfigurationInfo), null, 1);
			return array != null && array.Length == 1;
		}

		public static bool IsDehydratedConfiguration(OrganizationId orgId)
		{
			return Globals.IsDatacenter && !(orgId == null) && !OrganizationId.ForestWideOrgId.Equals(orgId) && !DatacenterRegistry.IsForefrontForOffice() && ProvisioningCache.Instance.TryAddAndGetOrganizationData<bool>(CannedProvisioningCacheKeys.IsDehydratedConfigurationCacheKey, orgId, delegate()
			{
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.FullyConsistent, ADSessionSettings.FromAllTenantsPartitionId(orgId.PartitionId), 310, "IsDehydratedConfiguration", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SharedConfiguration.cs");
				ExchangeConfigurationUnit[] array = tenantOrTopologyConfigurationSession.Find<ExchangeConfigurationUnit>(orgId.ConfigurationUnit, QueryScope.Base, new ExistsFilter(OrganizationSchema.SupportedSharedConfigurations), null, 1);
				return array != null && array.Length == 1 && array[0].IsDehydrated;
			});
		}

		public static bool IsDehydratedConfiguration(IConfigurationSession adConfigSession)
		{
			return SharedConfiguration.IsDehydratedConfiguration(adConfigSession.SessionSettings.CurrentOrganizationId);
		}

		public static IConfigurationSession CreateScopedToSharedConfigADSession(OrganizationId orgId)
		{
			ADSessionSettings adsessionSettings = null;
			if (SharedConfiguration.IsDehydratedConfiguration(orgId) || SharedConfiguration.IsSharedConfiguration(orgId))
			{
				SharedConfiguration sharedConfiguration = SharedConfiguration.GetSharedConfiguration(orgId);
				if (sharedConfiguration != null)
				{
					adsessionSettings = sharedConfiguration.GetSharedConfigurationSessionSettings();
				}
			}
			if (adsessionSettings == null)
			{
				adsessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), orgId, null, false);
			}
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, adsessionSettings, 366, "CreateScopedToSharedConfigADSession", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SharedConfiguration.cs");
		}

		public static IList<RetentionPolicy> GetDefaultRetentionPolicy(IConfigurationSession scopedSession, ADRawEntry user, SortBy sortBy, int resultSize)
		{
			bool isArbitrationMailbox = false;
			if (user[OrgPersonPresentationObjectSchema.RecipientTypeDetails] != null && (RecipientTypeDetails)user[OrgPersonPresentationObjectSchema.RecipientTypeDetails] == RecipientTypeDetails.ArbitrationMailbox)
			{
				isArbitrationMailbox = true;
			}
			return SharedConfiguration.GetDefaultRetentionPolicy(scopedSession, isArbitrationMailbox, sortBy, resultSize);
		}

		public static IList<RetentionPolicy> GetDefaultRetentionPolicy(IConfigurationSession scopedSession, bool isArbitrationMailbox, SortBy sortBy, int resultSize)
		{
			QueryFilter filter;
			if (isArbitrationMailbox)
			{
				filter = new ComparisonFilter(ComparisonOperator.Equal, RetentionPolicySchema.IsDefaultArbitrationMailbox, true);
			}
			else
			{
				filter = new ComparisonFilter(ComparisonOperator.Equal, RetentionPolicySchema.IsDefault, true);
			}
			return scopedSession.Find<RetentionPolicy>(null, QueryScope.SubTree, filter, sortBy, resultSize);
		}

		public static bool ExecutingUserHasRetentionPolicy(ADRawEntry executingUser, OrganizationId orgId)
		{
			if (executingUser[ADUserSchema.RetentionPolicy] != null)
			{
				return true;
			}
			if (executingUser[ADObjectSchema.OrganizationId] != null && !OrganizationId.ForestWideOrgId.Equals(executingUser[ADObjectSchema.OrganizationId]))
			{
				IConfigurationSession scopedSession = SharedConfiguration.CreateScopedToSharedConfigADSession(orgId);
				IList<RetentionPolicy> defaultRetentionPolicy = SharedConfiguration.GetDefaultRetentionPolicy(scopedSession, executingUser, null, 1);
				if (defaultRetentionPolicy != null && defaultRetentionPolicy.Count > 0)
				{
					return true;
				}
			}
			return false;
		}

		public static SharedTenantConfigurationState GetSharedConfigurationState(OrganizationId orgId)
		{
			if (OrganizationId.ForestWideOrgId.Equals(orgId))
			{
				return SharedTenantConfigurationState.UnSupported;
			}
			return ProvisioningCache.Instance.TryAddAndGetOrganizationData<SharedTenantConfigurationState>(CannedProvisioningCacheKeys.SharedConfigurationStateCacheKey, orgId, delegate()
			{
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.FullyConsistent, ADSessionSettings.FromAllTenantsPartitionId(orgId.PartitionId), 461, "GetSharedConfigurationState", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SharedConfiguration.cs");
				ExchangeConfigurationUnit[] array = tenantOrTopologyConfigurationSession.Find<ExchangeConfigurationUnit>(orgId.ConfigurationUnit, QueryScope.Base, QueryFilter.OrTogether(new QueryFilter[]
				{
					new ExistsFilter(OrganizationSchema.SupportedSharedConfigurations),
					new ExistsFilter(OrganizationSchema.SharedConfigurationInfo)
				}), null, 1);
				if (array == null || array.Length != 1)
				{
					return SharedTenantConfigurationState.NotShared;
				}
				if (null != array[0].SharedConfigurationInfo)
				{
					return SharedTenantConfigurationState.Shared;
				}
				SharedTenantConfigurationState sharedTenantConfigurationState = SharedTenantConfigurationState.UnSupported;
				if (array[0].IsStaticConfigurationShared)
				{
					sharedTenantConfigurationState |= SharedTenantConfigurationState.Static;
				}
				if (array[0].IsDehydrated)
				{
					sharedTenantConfigurationState |= SharedTenantConfigurationState.Dehydrated;
				}
				return sharedTenantConfigurationState;
			});
		}

		public ADObjectId[] GetSharedRoleGroupIds(ADObjectId[] origRoleGroupIds)
		{
			if (origRoleGroupIds == null)
			{
				throw new ArgumentNullException("origRoleGroupIds");
			}
			this.LoadTinyTenantCUIfNecessary();
			return this.GetGroupMapping(origRoleGroupIds, this.tinyTenantCU, this.sharedConfigurationCU);
		}

		public ADObjectId[] GetTinyTenantGroupIds(ADObjectId[] sharedGroupIds)
		{
			if (sharedGroupIds == null)
			{
				throw new ArgumentNullException("sharedGroupIds");
			}
			this.LoadTinyTenantCUIfNecessary();
			return this.GetGroupMapping(sharedGroupIds, this.sharedConfigurationCU, this.tinyTenantCU);
		}

		private ADObjectId[] GetGroupMapping(ADObjectId[] sourceGroupIds, ExchangeConfigurationUnit sourceConfigUnit, ExchangeConfigurationUnit targetConfigUnit)
		{
			if (sourceGroupIds == null)
			{
				throw new ArgumentNullException("sourceGroupIds");
			}
			if (sourceConfigUnit == null)
			{
				throw new ArgumentNullException("sourceConfigUnit");
			}
			if (targetConfigUnit == null)
			{
				throw new ArgumentNullException("targetConfigUnit");
			}
			List<ADObjectId> list = new List<ADObjectId>(sourceGroupIds.Length);
			foreach (ADObjectId adobjectId in sourceGroupIds)
			{
				Guid wkGuid;
				if (sourceConfigUnit.TryGetWellKnownGuidById(adobjectId, out wkGuid))
				{
					ADObjectId item;
					if (targetConfigUnit.TryGetIdByWellKnownGuid(wkGuid, out item))
					{
						list.Add(item);
					}
					else
					{
						list.Add(adobjectId);
					}
				}
			}
			return list.ToArray();
		}

		private void LoadTinyTenantCUIfNecessary()
		{
			if (this.tinyTenantCU == null)
			{
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.FullyConsistent, ADSessionSettings.FromAllTenantsPartitionId(this.tinyTenantId.PartitionId), 587, "LoadTinyTenantCUIfNecessary", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SharedConfiguration.cs");
				this.tinyTenantCU = tenantOrTopologyConfigurationSession.Read<ExchangeConfigurationUnit>(this.tinyTenantId.ConfigurationUnit);
			}
		}

		public ADObjectId GetSharedRoleAssignmentPolicy()
		{
			if (this.sharedRoleAssignmentPolicy == null)
			{
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, this.GetSharedConfigurationSessionSettings(), 602, "GetSharedRoleAssignmentPolicy", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SharedConfiguration.cs");
				RoleAssignmentPolicy[] array = tenantOrTopologyConfigurationSession.Find<RoleAssignmentPolicy>(null, QueryScope.SubTree, null, null, 1);
				this.sharedRoleAssignmentPolicy = array[0].Id;
			}
			return this.sharedRoleAssignmentPolicy;
		}

		public ADSessionSettings GetSharedConfigurationSessionSettings()
		{
			ADSessionSettings adsessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), this.sharedConfigurationCU.OrganizationId, null, false);
			adsessionSettings.IsSharedConfigChecked = true;
			adsessionSettings.IsRedirectedToSharedConfig = true;
			return adsessionSettings;
		}

		internal RbacContainer GetRbacContainer()
		{
			ITenantConfigurationSession tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.PartiallyConsistent, this.GetSharedConfigurationSessionSettings(), 648, "GetRbacContainer", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SharedConfiguration.cs");
			return tenantConfigurationSession.Read<RbacContainer>(this.sharedConfigurationCU.Id.GetDescendantId(new ADObjectId("CN=RBAC")));
		}

		internal static ExchangeConfigurationUnit FindOneSharedConfiguration(SharedConfigurationInfo sci, PartitionId partitionId)
		{
			ITenantConfigurationSession tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.FullyConsistent, ADSessionSettings.FromAllTenantsPartitionId(partitionId), 657, "FindOneSharedConfiguration", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SharedConfiguration.cs");
			ExchangeConfigurationUnit[] array = tenantConfigurationSession.FindSharedConfiguration(sci, true);
			if (array == null || array.Length == 0)
			{
				return null;
			}
			if (array.Length == 1)
			{
				return array[0];
			}
			Random random = new Random();
			return array[random.Next(array.Length)];
		}

		internal static OrganizationId FindOneSharedConfigurationId(SharedConfigurationInfo sci, PartitionId partitionId)
		{
			ExchangeConfigurationUnit exchangeConfigurationUnit = SharedConfiguration.FindOneSharedConfiguration(sci, partitionId);
			if (exchangeConfigurationUnit == null)
			{
				return null;
			}
			return exchangeConfigurationUnit.OrganizationId;
		}

		internal static bool DoesSctExistForVersion(ServerVersion version, string programId, string offerId, PartitionId partitionId)
		{
			SharedConfigurationInfo sci = new SharedConfigurationInfo(version, programId, offerId);
			return SharedConfiguration.FindOneSharedConfigurationId(sci, partitionId) != null;
		}

		internal static OrganizationId FindMostRecentSharedConfigurationInPartition(OrganizationId sourceOrganizationId, PartitionId targetAccountPartitionId, out Exception ex)
		{
			ex = null;
			OrganizationId organizationId = null;
			ExchangeConfigurationUnit[] array = SharedConfiguration.LoadSharedConfigurationsSorted(sourceOrganizationId);
			if (array != null && array.Length > 0)
			{
				int num = array.Length;
				SharedConfigurationInfo sharedConfigurationInfo = array[num - 1].SharedConfigurationInfo;
				organizationId = SharedConfiguration.FindOneSharedConfigurationId(sharedConfigurationInfo, targetAccountPartitionId);
				if (organizationId == null)
				{
					ex = new InvalidOperationException(DirectoryStrings.ErrorTargetPartitionSctMissing(sourceOrganizationId.ConfigurationUnit.DistinguishedName, targetAccountPartitionId.ForestFQDN, sharedConfigurationInfo.ToString()));
				}
			}
			return organizationId;
		}

		internal static string CreateSharedConfigurationName(string programId, string offerId)
		{
			SharedConfigurationInfo sharedConfigurationInfo = SharedConfigurationInfo.FromInstalledVersion(programId, offerId);
			string text = string.Format("{0}{1}{2}_{3}", new object[]
			{
				SharedConfiguration.SCTNamePrefix,
				"-",
				sharedConfigurationInfo.ToString().ToLower().Replace("_hydrated", null),
				Guid.NewGuid().ToString().Substring(0, 5)
			});
			if (text.Length > SharedConfiguration.SctNameMaxLength)
			{
				text = text.Substring(0, SharedConfiguration.SctNameMaxLength);
			}
			return text;
		}

		internal static SmtpDomain CreateSharedConfigurationDomainName(string sctName)
		{
			string text = sctName;
			string text2 = string.Format(".{0}", SharedConfiguration.SCTNamePrefix);
			int num = SharedConfiguration.SctNameMaxLength - text2.Length;
			if (text.Length > num)
			{
				text = text.Substring(0, num);
			}
			text = string.Format("{0}{1}", text, text2).ToLower();
			return new SmtpDomain(text);
		}

		private static string sctNamePrefix = "sct";

		private static int sctNameMaxLength = 64;

		private OrganizationId tinyTenantId;

		private ExchangeConfigurationUnit sharedConfigurationCU;

		private ExchangeConfigurationUnit tinyTenantCU;

		private ADObjectId sharedRoleAssignmentPolicy;
	}
}
