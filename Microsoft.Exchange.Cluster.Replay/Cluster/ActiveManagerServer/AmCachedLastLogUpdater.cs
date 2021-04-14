using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmCachedLastLogUpdater : TimerComponent
	{
		internal AmCachedLastLogUpdater() : base(TimeSpan.FromSeconds((double)RegistryParameters.PamLastLogUpdaterIntervalInSec), TimeSpan.FromSeconds((double)RegistryParameters.PamLastLogUpdaterIntervalInSec), "AmCachedLastLogUpdater")
		{
		}

		internal ExDateTime AddEntries(string serverNameFqdn, DateTime initiatedTimeUtc, KeyValuePair<Guid, long>[] lastLogEntries)
		{
			AmServerName amServerName = new AmServerName(serverNameFqdn);
			lock (this.locker)
			{
				AmCachedLastLogUpdater.ServerRequestInfo serverRequestInfo;
				if (!this.serverPropertyMap.TryGetValue(amServerName, out serverRequestInfo))
				{
					serverRequestInfo = new AmCachedLastLogUpdater.ServerRequestInfo(amServerName);
					this.serverPropertyMap[amServerName] = serverRequestInfo;
				}
				serverRequestInfo.Update(initiatedTimeUtc, lastLogEntries);
			}
			return this.GetLastUpdatedTime(amServerName);
		}

		private ExDateTime GetLastUpdatedTime(AmServerName serverName)
		{
			ExDateTime minValue;
			if (!this.serverUpdateTimeMap.TryGetValue(serverName, out minValue))
			{
				minValue = ExDateTime.MinValue;
			}
			return minValue;
		}

		internal Dictionary<AmServerName, AmCachedLastLogUpdater.ServerRequestInfo> Cleanup()
		{
			Dictionary<AmServerName, AmCachedLastLogUpdater.ServerRequestInfo> result;
			lock (this.locker)
			{
				Dictionary<AmServerName, AmCachedLastLogUpdater.ServerRequestInfo> dictionary = this.serverPropertyMap;
				this.serverPropertyMap = new Dictionary<AmServerName, AmCachedLastLogUpdater.ServerRequestInfo>(32);
				result = dictionary;
			}
			return result;
		}

		protected override void TimerCallbackInternal()
		{
			this.Flush();
		}

		internal void Flush()
		{
			if (AmSystemManager.Instance.Config.IsPAM)
			{
				Dictionary<AmServerName, AmCachedLastLogUpdater.ServerRequestInfo> dictionary = this.Cleanup();
				if (dictionary.Count > 0)
				{
					Exception ex = null;
					try
					{
						using (IClusterDB clusterDB = ClusterDB.Open())
						{
							if (clusterDB.IsInitialized)
							{
								using (IClusterDBWriteBatch clusterDBWriteBatch = clusterDB.CreateWriteBatch("ExchangeActiveManager\\LastLog"))
								{
									this.PopulateBatch(clusterDBWriteBatch, dictionary);
									clusterDBWriteBatch.Execute();
									ExDateTime now = ExDateTime.Now;
									foreach (AmServerName key in dictionary.Keys)
									{
										this.serverUpdateTimeMap[key] = now;
									}
									goto IL_BA;
								}
							}
							ExTraceGlobals.ClusterTracer.TraceError((long)this.GetHashCode(), "Flush(): clusdb is not initialized");
							IL_BA:;
						}
					}
					catch (ADExternalException ex2)
					{
						ex = ex2;
					}
					catch (ADOperationException ex3)
					{
						ex = ex3;
					}
					catch (ADTransientException ex4)
					{
						ex = ex4;
					}
					catch (ClusterException ex5)
					{
						ex = ex5;
					}
					catch (AmServerException ex6)
					{
						ex = ex6;
					}
					catch (AmServerTransientException ex7)
					{
						ex = ex7;
					}
					if (ex != null)
					{
						ReplayCrimsonEvents.CachedLastLogUpdateFailed.LogPeriodic<int, string>(AmServerName.LocalComputerName.NetbiosName, TimeSpan.FromMinutes(5.0), dictionary.Count, ex.Message);
					}
				}
			}
		}

		private void PopulateBatch(IClusterDBWriteBatch writeBatch, Dictionary<AmServerName, AmCachedLastLogUpdater.ServerRequestInfo> requestInfoMap)
		{
			Dictionary<Guid, AmDbStateInfo> dbStateInfoMap = new Dictionary<Guid, AmDbStateInfo>();
			AmConfig config = AmSystemManager.Instance.Config;
			if (!config.IsPAM)
			{
				throw new AmInvalidConfiguration(string.Format("Role = {0}", config.Role));
			}
			AmDbStateInfo[] array = config.DbState.ReadAll();
			if (array != null)
			{
				dbStateInfoMap = array.ToDictionary((AmDbStateInfo s) => s.DatabaseGuid);
			}
			foreach (AmCachedLastLogUpdater.ServerRequestInfo serverRequestInfo in requestInfoMap.Values)
			{
				AmServerName serverName = serverRequestInfo.ServerName;
				Dictionary<Guid, long> databaseLogGenMap = serverRequestInfo.DatabaseLogGenMap;
				HashSet<Guid> databasesByServer = this.GetDatabasesByServer(serverName);
				string value = serverRequestInfo.MostRecentRequestReceivedUtc.ToString("s");
				foreach (KeyValuePair<Guid, long> keyValuePair in databaseLogGenMap)
				{
					Guid key = keyValuePair.Key;
					long value2 = keyValuePair.Value;
					string text = key.ToString();
					string valueName = AmDbState.ConstructLastLogTimeStampProperty(text);
					if (this.IsDatabaseActiveOnServer(dbStateInfoMap, key, serverName))
					{
						writeBatch.SetValue(text, value2.ToString());
						writeBatch.SetValue(valueName, value);
					}
					databasesByServer.Remove(key);
				}
				foreach (Guid databaseGuid in databasesByServer)
				{
					string valueName2 = AmDbState.ConstructLastLogTimeStampProperty(databaseGuid.ToString());
					if (this.IsDatabaseActiveOnServer(dbStateInfoMap, databaseGuid, serverName))
					{
						writeBatch.SetValue(valueName2, value);
					}
				}
				writeBatch.SetValue(serverName.NetbiosName, value);
			}
		}

		private HashSet<Guid> GetDatabasesByServer(AmServerName serverName)
		{
			HashSet<Guid> hashSet = new HashSet<Guid>();
			IADConfig adconfig = Dependencies.ADConfig;
			IADServer server = adconfig.GetServer(serverName);
			if (server != null)
			{
				IEnumerable<IADDatabase> databasesOnServer = adconfig.GetDatabasesOnServer(serverName);
				if (databasesOnServer == null)
				{
					return hashSet;
				}
				using (IEnumerator<IADDatabase> enumerator = databasesOnServer.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						IADDatabase iaddatabase = enumerator.Current;
						hashSet.Add(iaddatabase.Guid);
					}
					return hashSet;
				}
			}
			ExTraceGlobals.ClusterTracer.TraceError<AmServerName>((long)this.GetHashCode(), "MiniSever object is null when querying for server '{0}'", serverName);
			return hashSet;
		}

		private bool IsDatabaseActiveOnServer(Dictionary<Guid, AmDbStateInfo> dbStateInfoMap, Guid databaseGuid, AmServerName serverName)
		{
			AmDbStateInfo amDbStateInfo;
			return dbStateInfoMap != null && dbStateInfoMap.TryGetValue(databaseGuid, out amDbStateInfo) && amDbStateInfo != null && amDbStateInfo.IsActiveServerValid && AmServerName.IsEqual(amDbStateInfo.ActiveServer, serverName);
		}

		private Dictionary<AmServerName, AmCachedLastLogUpdater.ServerRequestInfo> serverPropertyMap = new Dictionary<AmServerName, AmCachedLastLogUpdater.ServerRequestInfo>(32);

		private readonly ConcurrentDictionary<AmServerName, ExDateTime> serverUpdateTimeMap = new ConcurrentDictionary<AmServerName, ExDateTime>();

		private readonly object locker = new object();

		internal class ServerRequestInfo
		{
			internal ServerRequestInfo(AmServerName serverName)
			{
				this.ServerName = serverName;
				this.MostRecentRequestReceivedUtc = SharedHelper.DateTimeMinValueUtc;
				this.DatabaseLogGenMap = new Dictionary<Guid, long>();
			}

			internal void Update(DateTime initiatedTimeUtc, KeyValuePair<Guid, long>[] lastLogEntries)
			{
				this.MostRecentRequestReceivedUtc = initiatedTimeUtc;
				foreach (KeyValuePair<Guid, long> keyValuePair in lastLogEntries)
				{
					Guid key = keyValuePair.Key;
					long value = keyValuePair.Value;
					long num = 0L;
					if (!this.DatabaseLogGenMap.TryGetValue(key, out num) || value > num)
					{
						this.DatabaseLogGenMap[key] = value;
					}
				}
			}

			internal AmServerName ServerName { get; private set; }

			internal DateTime MostRecentRequestReceivedUtc { get; set; }

			internal Dictionary<Guid, long> DatabaseLogGenMap { get; set; }
		}
	}
}
