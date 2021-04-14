using System;
using System.Net;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationCache;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal static class ADSystemConfigurationSession
	{
		public static ITopologyConfigurationSession CreateRemoteForestSession(string fqdn, NetworkCredential credential)
		{
			if (string.IsNullOrEmpty(fqdn))
			{
				throw new ArgumentNullException("fqdn");
			}
			if (credential != null && (string.IsNullOrEmpty(credential.UserName) || string.IsNullOrEmpty(credential.Password)))
			{
				throw new ArgumentException("credential");
			}
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(null, true, ConsistencyMode.PartiallyConsistent, credential, ADSessionSettings.FromAccountPartitionRootOrgScopeSet(new PartitionId(fqdn)), 69, "CreateRemoteForestSession", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SystemConfiguration\\ADSystemConfigurationSession.cs");
			ADServerInfo remoteServerFromDomainFqdn = TopologyProvider.GetInstance().GetRemoteServerFromDomainFqdn(fqdn, credential);
			topologyConfigurationSession.DomainController = remoteServerFromDomainFqdn.Fqdn;
			topologyConfigurationSession.EnforceDefaultScope = false;
			return topologyConfigurationSession;
		}

		internal static ADObjectId GetFirstOrgUsersContainerId()
		{
			if (ADSystemConfigurationSession.firstOrgUsersContainerId == null)
			{
				ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(true, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 95, "GetFirstOrgUsersContainerId", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SystemConfiguration\\ADSystemConfigurationSession.cs");
				topologyConfigurationSession.UseConfigNC = false;
				ADObjectId domainNamingContext = topologyConfigurationSession.GetDomainNamingContext();
				ExchangeOrganizationalUnit exchangeOrganizationalUnit = topologyConfigurationSession.ResolveWellKnownGuid<ExchangeOrganizationalUnit>(WellKnownGuid.UsersWkGuid, domainNamingContext);
				ADObjectId id = exchangeOrganizationalUnit.Id;
				lock (ADSystemConfigurationSession.syncObj)
				{
					if (ADSystemConfigurationSession.firstOrgUsersContainerId == null)
					{
						ADSystemConfigurationSession.firstOrgUsersContainerId = id;
					}
				}
			}
			return ADSystemConfigurationSession.firstOrgUsersContainerId;
		}

		internal static ADObjectId GetRootOrgContainerIdForLocalForest()
		{
			return ADSystemConfigurationSession.GetRootOrgContainerId(TopologyProvider.LocalForestFqdn, null, null);
		}

		internal static ADObjectId GetRootOrgContainerId(string domainController, NetworkCredential credential)
		{
			return ADSystemConfigurationSession.GetRootOrgContainerId(TopologyProvider.LocalForestFqdn, domainController, credential);
		}

		internal static ADObjectId GetRootOrgContainerId(string partitionFqdn, string domainController, NetworkCredential credential)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("partitionFqdn", partitionFqdn);
			ADObjectId rootOrgContainerId = InternalDirectoryRootOrganizationCache.GetRootOrgContainerId(partitionFqdn);
			if (rootOrgContainerId != null)
			{
				return rootOrgContainerId;
			}
			Organization rootOrgContainer = ADSystemConfigurationSession.GetRootOrgContainer(partitionFqdn, domainController, credential);
			if (rootOrgContainer == null)
			{
				return ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest();
			}
			return rootOrgContainer.Id;
		}

		internal static Organization GetRootOrgContainer(string partitionFqdn, string domainController, NetworkCredential credential)
		{
			bool flag = string.IsNullOrEmpty(domainController);
			ADObjectId adobjectId;
			if (!PartitionId.IsLocalForestPartition(partitionFqdn))
			{
				adobjectId = ADSession.GetConfigurationNamingContext(partitionFqdn);
			}
			else if (flag)
			{
				adobjectId = ADSession.GetConfigurationNamingContextForLocalForest();
			}
			else
			{
				adobjectId = ADSession.GetConfigurationNamingContext(domainController, credential);
			}
			ADSessionSettings sessionSettings = ADSessionSettings.FromRootOrgBootStrapSession(adobjectId);
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.NonCacheSessionFactory.CreateTopologyConfigurationSession(domainController, true, ConsistencyMode.PartiallyConsistent, flag ? null : credential, sessionSettings, 226, "GetRootOrgContainer", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SystemConfiguration\\ADSystemConfigurationSession.cs");
			Organization[] array = topologyConfigurationSession.Find<Organization>(adobjectId, QueryScope.SubTree, null, null, 1);
			if (array != null && array.Length > 0)
			{
				if (string.IsNullOrEmpty(domainController) && credential == null)
				{
					InternalDirectoryRootOrganizationCache.PopulateCache(partitionFqdn, array[0]);
				}
				return array[0];
			}
			if (flag && (Globals.IsDatacenter || PartitionId.IsLocalForestPartition(partitionFqdn)))
			{
				throw new OrgContainerNotFoundException();
			}
			RootDse rootDse = topologyConfigurationSession.GetRootDse();
			if (rootDse.ConfigurationNamingContext.Equals(ADSession.GetConfigurationNamingContext(partitionFqdn)))
			{
				throw new OrgContainerNotFoundException();
			}
			return null;
		}

		public const string InformationStoreRDN = "InformationStore";

		private static readonly object syncObj = new object();

		private static ADObjectId firstOrgUsersContainerId = null;
	}
}
