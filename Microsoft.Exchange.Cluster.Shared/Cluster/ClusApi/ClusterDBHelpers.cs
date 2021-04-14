using System;
using Microsoft.Exchange.Net;
using Microsoft.Win32;

namespace Microsoft.Exchange.Cluster.ClusApi
{
	public static class ClusterDBHelpers
	{
		public static void ReadServerDatabaseSchemaVersionRange(IClusterDB iClusterDB, Guid serverGuid, int defaultMinimum, int defaultMaximum, out int minVersion, out int maxVersion)
		{
			string keyName = string.Format("Exchange\\Servers\\{0}\\Schema", serverGuid);
			minVersion = iClusterDB.GetValue<int>(keyName, "Minimum Version", defaultMinimum);
			maxVersion = iClusterDB.GetValue<int>(keyName, "Maximum Version", defaultMaximum);
		}

		public static void ReadServerDatabaseSchemaVersionRange(Guid serverGuid, int defaultMinimum, int defaultMaximum, out int minVersion, out int maxVersion)
		{
			string subkeyName = string.Format("Cluster\\Exchange\\Servers\\{0}\\Schema", serverGuid);
			minVersion = RegistryReader.Instance.GetValue<int>(Registry.LocalMachine, subkeyName, "Minimum Version", defaultMinimum);
			maxVersion = RegistryReader.Instance.GetValue<int>(Registry.LocalMachine, subkeyName, "Maximum Version", defaultMaximum);
		}

		public static void WriteServerDatabaseSchemaVersionRange(IClusterDB iClusterDB, Guid serverGuid, int minVersion, int maxVersion)
		{
			string registryKey = string.Format("Exchange\\Servers\\{0}\\Schema", serverGuid);
			using (IClusterDBWriteBatch clusterDBWriteBatch = iClusterDB.CreateWriteBatch(registryKey))
			{
				clusterDBWriteBatch.SetValue("Minimum Version", minVersion);
				clusterDBWriteBatch.SetValue("Maximum Version", maxVersion);
				clusterDBWriteBatch.Execute();
			}
		}

		public static void RemoveServerSchemaVersionRange(IClusterDB iClusterDB, Guid serverGuid)
		{
			using (IClusterDBWriteBatch clusterDBWriteBatch = iClusterDB.CreateWriteBatch("Exchange\\Servers"))
			{
				clusterDBWriteBatch.DeleteKey(serverGuid.ToString());
				clusterDBWriteBatch.Execute();
			}
		}

		public static void ReadRequestedDatabaseSchemaVersion(IClusterDB iClusterDB, Guid databaseGuid, int defaultVersion, out int requestedVersion)
		{
			string keyName = string.Format("Exchange\\Databases\\{0}\\Schema", databaseGuid);
			requestedVersion = iClusterDB.GetValue<int>(keyName, "Requested Version", defaultVersion);
		}

		public static void ReadRequestedDatabaseSchemaVersion(Guid databaseGuid, int defaultVersion, out int requestedVersion)
		{
			string subkeyName = string.Format("Exchange\\Databases\\{0}\\Schema", databaseGuid);
			requestedVersion = RegistryReader.Instance.GetValue<int>(Registry.LocalMachine, subkeyName, "Requested Version", defaultVersion);
		}

		public static void WriteRequestedDatabaseSchemaVersion(IClusterDB iClusterDB, Guid databaseGuid, int requestedVersion)
		{
			string registryKey = string.Format("Exchange\\Databases\\{0}\\Schema", databaseGuid);
			using (IClusterDBWriteBatch clusterDBWriteBatch = iClusterDB.CreateWriteBatch(registryKey))
			{
				clusterDBWriteBatch.SetValue("Requested Version", requestedVersion);
				clusterDBWriteBatch.Execute();
			}
		}

		public static void RemoveDatabaseRequestedSchemaVersion(IClusterDB iClusterDB, Guid databaseGuid)
		{
			using (IClusterDBWriteBatch clusterDBWriteBatch = iClusterDB.CreateWriteBatch("Exchange\\Databases"))
			{
				clusterDBWriteBatch.DeleteKey(databaseGuid.ToString());
				clusterDBWriteBatch.Execute();
			}
		}

		private const string ServerClusterDBRootPath = "Exchange\\Servers";

		private const string ServerClusterDBPath = "Exchange\\Servers\\{0}\\Schema";

		private const string ServerClusterDBRegistryPath = "Cluster\\Exchange\\Servers\\{0}\\Schema";

		private const string DatabaseClusterDBRootPath = "Exchange\\Databases";

		private const string DatabaseClusterDBPath = "Exchange\\Databases\\{0}\\Schema";

		private const string DatabaseClusterDBRegistryPath = "Cluster\\Exchange\\Databases\\{0}\\Schema";

		private const string MinimumVersion = "Minimum Version";

		private const string MaximumVersion = "Maximum Version";

		private const string RequestedVersion = "Requested Version";
	}
}
