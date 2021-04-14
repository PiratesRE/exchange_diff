using System;
using System.Collections.Generic;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Rpc.ActiveManager;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmServerDbStatusInfoCache
	{
		internal static AmDbStatusInfo2 GetServerForDatabase(Guid mdbGuid)
		{
			AmConfig config = AmSystemManager.Instance.Config;
			if (config.IsUnknown)
			{
				AmTrace.Error("GetSFD: Invalid configuration (db={0})", new object[]
				{
					mdbGuid
				});
				throw new AmInvalidConfiguration(config.LastError);
			}
			AmDbStateInfo stateInfo = config.DbState.Read(mdbGuid);
			return AmServerDbStatusInfoCache.ConvertToDbStatusInfo(stateInfo);
		}

		internal AmDbStatusInfo2 GetEntry(Guid databaseGuid)
		{
			AmConfig config = AmSystemManager.Instance.Config;
			if (config.IsUnknown)
			{
				AmTrace.Error("GetSFD: Invalid configuration (db={0})", new object[]
				{
					databaseGuid
				});
				throw new AmInvalidConfiguration(config.LastError);
			}
			string text = config.DbState.ReadStateString(databaseGuid);
			AmServerDbStatusInfoCache.StringStatusInfoPair stringStatusInfoPair = null;
			AmDbStatusInfo2 amDbStatusInfo = null;
			lock (this.m_locker)
			{
				this.m_cacheMap.TryGetValue(databaseGuid, out stringStatusInfoPair);
			}
			if (stringStatusInfoPair != null && text != null && string.Equals(text, stringStatusInfoPair.RawStateString))
			{
				amDbStatusInfo = stringStatusInfoPair.StatusInfo;
			}
			else
			{
				AmDbStateInfo stateInfo = AmDbStateInfo.Parse(databaseGuid, text);
				amDbStatusInfo = AmServerDbStatusInfoCache.ConvertToDbStatusInfo(stateInfo);
				stringStatusInfoPair = new AmServerDbStatusInfoCache.StringStatusInfoPair(text, amDbStatusInfo);
				lock (this.m_locker)
				{
					AmTrace.Debug("Updating cache for database {0}.", new object[]
					{
						databaseGuid
					});
					this.m_cacheMap[databaseGuid] = stringStatusInfoPair;
				}
			}
			return amDbStatusInfo;
		}

		internal void Clear()
		{
			lock (this.m_locker)
			{
				this.m_cacheMap.Clear();
			}
		}

		private static AmDbStatusInfo2 ConvertToDbStatusInfo(AmDbStateInfo stateInfo)
		{
			if (!stateInfo.IsMountSucceededAtleastOnce)
			{
				AmTrace.Error("Database does not appear to be ever attempted for mount {0}", new object[]
				{
					stateInfo.DatabaseGuid
				});
				throw new AmDatabaseNeverMountedException();
			}
			return new AmDbStatusInfo2(stateInfo.ActiveServer.Fqdn, 0, stateInfo.LastMountedServer.Fqdn, stateInfo.LastMountedTime);
		}

		private Dictionary<Guid, AmServerDbStatusInfoCache.StringStatusInfoPair> m_cacheMap = new Dictionary<Guid, AmServerDbStatusInfoCache.StringStatusInfoPair>(100);

		private object m_locker = new object();

		internal class StringStatusInfoPair
		{
			internal StringStatusInfoPair(string rawStateStr, AmDbStatusInfo2 statusInfo)
			{
				this.RawStateString = rawStateStr;
				this.StatusInfo = statusInfo;
			}

			internal string RawStateString { get; private set; }

			internal AmDbStatusInfo2 StatusInfo { get; private set; }
		}
	}
}
