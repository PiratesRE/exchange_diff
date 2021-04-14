using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal static class PublicFolderHelper
	{
		public static bool IsPublicFolderReplicationEnabled()
		{
			bool result = false;
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 52, "IsPublicFolderReplicationEnabled", "f:\\15.00.1497\\sources\\dev\\cluster\\src\\Replay\\Core\\PublicFolderHelper.cs");
			PublicFolderTree[] array = tenantOrTopologyConfigurationSession.Find<PublicFolderTree>(null, QueryScope.SubTree, new ComparisonFilter(ComparisonOperator.Equal, PublicFolderTreeSchema.PublicFolderTreeType, PublicFolderTreeType.Mapi), null, 0);
			if (array != null && array.Length == 1)
			{
				MultiValuedProperty<ADObjectId> publicDatabases = array[0].PublicDatabases;
				if (publicDatabases != null && publicDatabases.Count > 1)
				{
					result = true;
				}
			}
			return result;
		}

		public static bool IsPublicFolderReplicationSuspended()
		{
			bool result = false;
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 97, "IsPublicFolderReplicationSuspended", "f:\\15.00.1497\\sources\\dev\\cluster\\src\\Replay\\Core\\PublicFolderHelper.cs");
			Organization[] array = tenantOrTopologyConfigurationSession.Find<Organization>(null, QueryScope.SubTree, null, null, 0);
			if (array != null && array.Length > 0 && (array[0].Heuristics & HeuristicsFlags.SuspendFolderReplication) != HeuristicsFlags.None)
			{
				result = true;
			}
			return result;
		}

		private const string PublicfolderDatabaseObjectClass = "msExchPublicMDB";
	}
}
