using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Win32;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmClusdbPeriodicCleanup : TimerComponent
	{
		public AmClusdbPeriodicCleanup() : base(TimeSpan.FromSeconds((double)RegistryParameters.ClusdbPeriodicCleanupStartDelayInSecs), TimeSpan.FromSeconds((double)RegistryParameters.ClusdbPeriodicCleanupIntervalInSecs), "AmClusdbPeriodicCleanup")
		{
		}

		protected override void TimerCallbackInternal()
		{
			try
			{
				if (AmSystemManager.Instance.Config.IsPAM)
				{
					using (IClusterDB clusterDB = ClusterDB.Open())
					{
						this.CleanupStaleEntries(clusterDB);
					}
				}
			}
			catch (Exception ex)
			{
				ReplayCrimsonEvents.ClusdbPeriodicCleanupFailed.Log<string, string>("Common", ex.ToString());
			}
			finally
			{
				if (this.staleDatabases.Count > 0 || this.staleServers.Count > 0)
				{
					ReplayCrimsonEvents.ClusdbPeriodicCleanupFoundStale.Log<int, int>(this.staleDatabases.Count, this.staleServers.Count);
				}
				else
				{
					ReplayCrimsonEvents.ClusdbPeriodicCleanupNoStale.Log();
				}
			}
		}

		internal Dictionary<Guid, AmDbStateInfo> GetAllDbStatesFromClusdb(IClusterDB clusdb)
		{
			Dictionary<Guid, AmDbStateInfo> dictionary = new Dictionary<Guid, AmDbStateInfo>();
			if (!DistributedStore.Instance.IsKeyExist("ExchangeActiveManager\\DbState", null))
			{
				return dictionary;
			}
			IEnumerable<Tuple<string, RegistryValueKind>> valueInfos = clusdb.GetValueInfos("ExchangeActiveManager\\DbState");
			if (valueInfos != null)
			{
				foreach (Tuple<string, RegistryValueKind> tuple in valueInfos)
				{
					string item = tuple.Item1;
					string value = clusdb.GetValue<string>("ExchangeActiveManager\\DbState", item, string.Empty);
					Guid guid;
					if (!string.IsNullOrEmpty(value) && Guid.TryParse(item, out guid))
					{
						AmDbStateInfo value2 = AmDbStateInfo.Parse(guid, value);
						dictionary.Add(guid, value2);
					}
				}
			}
			return dictionary;
		}

		private HashSet<Guid> GetDatabasesToBeDeleted(IMonitoringADConfig adConfig, IClusterDB clusdb, out HashSet<Guid> validDatabaseGuids)
		{
			validDatabaseGuids = new HashSet<Guid>();
			Dictionary<Guid, AmDbStateInfo> allDbStatesFromClusdb = this.GetAllDbStatesFromClusdb(clusdb);
			foreach (KeyValuePair<Guid, AmDbStateInfo> keyValuePair in allDbStatesFromClusdb)
			{
				Guid key = keyValuePair.Key;
				DateTime lastMountedTime = keyValuePair.Value.LastMountedTime;
				IADDatabase iaddatabase;
				bool flag = adConfig.DatabaseByGuidMap.TryGetValue(key, out iaddatabase);
				bool flag2 = lastMountedTime > DateTime.UtcNow.AddDays(-5.0);
				if (flag || flag2)
				{
					validDatabaseGuids.Add(key);
				}
			}
			return new HashSet<Guid>(allDbStatesFromClusdb.Keys.Except(validDatabaseGuids));
		}

		internal HashSet<Guid> GetAllServersInClusdb(IClusterDB clusdb)
		{
			HashSet<Guid> hashSet = new HashSet<Guid>();
			if (!DistributedStore.Instance.IsKeyExist("Exchange\\Servers", null))
			{
				return hashSet;
			}
			IEnumerable<string> subKeyNames = clusdb.GetSubKeyNames("Exchange\\Servers");
			if (subKeyNames != null)
			{
				foreach (string input in subKeyNames)
				{
					Guid item;
					if (Guid.TryParse(input, out item))
					{
						hashSet.Add(item);
					}
				}
			}
			return hashSet;
		}

		private HashSet<Guid> GetServersToBeDeleted(IMonitoringADConfig adConfig, IClusterDB clusdb, out Dictionary<Guid, IADServer> validServers)
		{
			validServers = new Dictionary<Guid, IADServer>();
			HashSet<Guid> allServersInClusdb = this.GetAllServersInClusdb(clusdb);
			Dictionary<Guid, IADServer> dictionary = adConfig.Servers.ToDictionary((IADServer s) => s.Guid);
			foreach (Guid key in allServersInClusdb)
			{
				IADServer value;
				if (dictionary.TryGetValue(key, out value))
				{
					validServers.Add(key, value);
				}
			}
			allServersInClusdb.ExceptWith(validServers.Keys);
			return allServersInClusdb;
		}

		private void CleanupStaleEntries(IClusterDB clusdb)
		{
			IMonitoringADConfig config = Dependencies.MonitoringADConfigProvider.GetConfig(true);
			HashSet<Guid> validDatabaseGuids;
			HashSet<Guid> databasesToBeDeleted = this.GetDatabasesToBeDeleted(config, clusdb, out validDatabaseGuids);
			Dictionary<Guid, IADServer> validServers;
			HashSet<Guid> serversToBeDeleted = this.GetServersToBeDeleted(config, clusdb, out validServers);
			if (databasesToBeDeleted.Count == 0 && serversToBeDeleted.Count == 0)
			{
				this.staleDatabases.Clear();
				this.staleServers.Clear();
				return;
			}
			IADToplogyConfigurationSession adSession = ADSessionFactory.CreateIgnoreInvalidRootOrgSession(true);
			this.CleanupDatabases(adSession, clusdb, databasesToBeDeleted, validDatabaseGuids);
			this.CleanupServers(adSession, clusdb, serversToBeDeleted, validServers);
		}

		private bool CleanupDatabases(IADToplogyConfigurationSession adSession, IClusterDB clusdb, HashSet<Guid> dbGuidsToDelete, HashSet<Guid> validDatabaseGuids)
		{
			if (validDatabaseGuids.Count == 0)
			{
				ReplayCrimsonEvents.ClusdbPeriodicCleanupSkipped.Log<string, string>("Database", "Could not find a single valid database in cached list");
				return false;
			}
			if (adSession.FindDatabaseByGuid(validDatabaseGuids.First<Guid>()) == null)
			{
				ReplayCrimsonEvents.ClusdbPeriodicCleanupSkipped.Log<string, string>("Database", "Could not find a single valid database in AD");
				return false;
			}
			bool result = false;
			try
			{
				this.RemoveStaleDatabaseGuids(clusdb, dbGuidsToDelete);
				dbGuidsToDelete.ExceptWith(this.staleDatabases);
				result = true;
			}
			catch (Exception ex)
			{
				ReplayCrimsonEvents.ClusdbPeriodicCleanupFailed.Log<string, string>("Database", ex.ToString());
			}
			this.staleDatabases = dbGuidsToDelete;
			return result;
		}

		private void RemoveStaleDatabaseGuids(IClusterDB clusdb, HashSet<Guid> dbGuids)
		{
			this.staleDatabases.IntersectWith(dbGuids);
			if (this.staleDatabases.Count == 0)
			{
				return;
			}
			this.RemoveItemsFromClusdb("Database", clusdb, "ExchangeActiveManager\\LastLog", false, this.staleDatabases, null, 200);
			this.RemoveItemsFromClusdb("Database", clusdb, "ExchangeActiveManager\\LastLog", false, this.staleDatabases, (string s) => s + "_Time", 200);
			this.RemoveItemsFromClusdb("Database", clusdb, "ExchangeActiveManager\\Dumpster", true, this.staleDatabases, null, 200);
			this.RemoveItemsFromClusdb("Database", clusdb, "ExchangeActiveManager\\SafetyNet2", true, this.staleDatabases, null, 200);
			this.RemoveItemsFromClusdb("Database", clusdb, "Exchange\\Databases", true, this.staleDatabases, null, 200);
			this.RemoveItemsFromClusdb("Database", clusdb, "MsExchangeIs", true, this.staleDatabases, (string s) => "Private_" + s, 200);
			this.RemoveItemsFromClusdb("Database", clusdb, "ExchangeActiveManager\\DbState", false, this.staleDatabases, null, 200);
		}

		private bool CleanupServers(IADToplogyConfigurationSession adSession, IClusterDB clusdb, HashSet<Guid> serverGuids, Dictionary<Guid, IADServer> validServers)
		{
			if (validServers.Count == 0)
			{
				ReplayCrimsonEvents.ClusdbPeriodicCleanupSkipped.Log<string, string>("Server", "Could not find a single valid server in the cached list");
				return false;
			}
			IADServer value = validServers.First<KeyValuePair<Guid, IADServer>>().Value;
			if (adSession.FindServerByName(value.Name) == null)
			{
				ReplayCrimsonEvents.ClusdbPeriodicCleanupSkipped.Log<string, string>("Server", "Could not find a single valid server in AD");
				return false;
			}
			bool result = false;
			try
			{
				this.RemoveStaleServerGuids(clusdb, serverGuids);
				serverGuids.ExceptWith(this.staleServers);
				result = true;
			}
			catch (Exception ex)
			{
				ReplayCrimsonEvents.ClusdbPeriodicCleanupFailed.Log<string, string>("Server", ex.ToString());
			}
			this.staleServers = serverGuids;
			return result;
		}

		private void RemoveStaleServerGuids(IClusterDB clusdb, HashSet<Guid> serverGuids)
		{
			this.staleServers.IntersectWith(serverGuids);
			if (this.staleServers.Count == 0)
			{
				return;
			}
			this.RemoveItemsFromClusdb("Database", clusdb, "Exchange\\Servers", true, this.staleServers, null, 200);
		}

		private int RemoveItemsFromClusdb(string objectName, IClusterDB clusdb, string keyName, bool isDeleteKey, IEnumerable<Guid> collection, Func<string, string> nameGenerator = null, int batchSize = 200)
		{
			if (!DistributedStore.Instance.IsKeyExist(keyName, null))
			{
				return 0;
			}
			int num = 0;
			int num2 = 0;
			HashSet<string> hashSet = new HashSet<string>(collection.Select(delegate(Guid id)
			{
				string text2 = id.ToString();
				if (nameGenerator != null)
				{
					return nameGenerator(text2);
				}
				return text2;
			}));
			num2 = hashSet.Count;
			try
			{
				if (isDeleteKey)
				{
					IEnumerable<string> subKeyNames = clusdb.GetSubKeyNames(keyName);
					hashSet.IntersectWith(subKeyNames);
				}
				else
				{
					IEnumerable<string> other = from vi in clusdb.GetValueInfos(keyName)
					select vi.Item1;
					hashSet.IntersectWith(other);
				}
			}
			catch (Exception ex)
			{
				ReplayCrimsonEvents.ClusdbPeriodicCleanupFailed.Log<string, string>(objectName, ex.ToString());
				return 0;
			}
			if (hashSet.Count == 0)
			{
				return 0;
			}
			int num3 = 0;
			int num4 = 0;
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			foreach (IEnumerable<string> enumerable in hashSet.Batch(batchSize))
			{
				num++;
				int num5 = 0;
				try
				{
					using (IClusterDBWriteBatch clusterDBWriteBatch = clusdb.CreateWriteBatch(keyName))
					{
						foreach (string text in enumerable)
						{
							num5++;
							if (isDeleteKey)
							{
								clusterDBWriteBatch.DeleteKey(text);
							}
							else
							{
								clusterDBWriteBatch.DeleteValue(text);
							}
						}
						clusterDBWriteBatch.Execute();
						num3 += num5;
					}
				}
				catch (Exception ex2)
				{
					num4++;
					ReplayCrimsonEvents.ClusdbCleanupBatchOperationFailed.Log<string, string, string, bool, int, int, int, int>(objectName, ex2.Message, keyName, isDeleteKey, num, num5, num2, hashSet.Count);
				}
			}
			long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
			ReplayCrimsonEvents.ClusdbPeriodicCleanupCompleted.Log<string, string, int, string, int, int, long>(objectName, keyName, num3, isDeleteKey ? "Key" : "Property", num, num4, elapsedMilliseconds);
			return num3;
		}

		public const string DbStateSubKey = "ExchangeActiveManager\\DbState";

		public const string ServersSubKey = "Exchange\\Servers";

		public const string DatabaseCategoryName = "Database";

		public const string ServerCategoryName = "Server";

		private HashSet<Guid> staleDatabases = new HashSet<Guid>();

		private HashSet<Guid> staleServers = new HashSet<Guid>();
	}
}
