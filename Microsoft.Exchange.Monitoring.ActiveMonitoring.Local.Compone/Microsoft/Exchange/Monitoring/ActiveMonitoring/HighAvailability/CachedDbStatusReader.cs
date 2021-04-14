using System;
using System.Collections.Generic;
using Microsoft.Exchange.Cluster.ActiveManagerServer;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Rpc.Cluster;
using Microsoft.Win32;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.HighAvailability
{
	internal class CachedDbStatusReader
	{
		private CachedDbStatusReader()
		{
			int value = HighAvailabilityUtility.NonCachedRegReader.GetValue<int>(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ActiveMonitoring\\HighAvailability\\Parameters", "RpcCacheExpirationInSeconds", 120);
			this.DefaultTimeout = TimeSpan.FromSeconds((double)value);
			this.localCopyStatusCachedList = new CachedList<CopyStatusClientCachedEntry, Guid>(delegate(Guid[] guids)
			{
				List<KeyValuePair<Guid, CopyStatusClientCachedEntry>> list = new List<KeyValuePair<Guid, CopyStatusClientCachedEntry>>();
				Exception ex = null;
				CopyStatusClientCachedEntry[] copyStatus = CopyStatusHelper.GetCopyStatus(AmServerName.LocalComputerName, RpcGetDatabaseCopyStatusFlags2.None, guids, 5000, null, out ex);
				if (ex != null)
				{
					throw new HighAvailabilityMAProbeException(string.Format("exception caught GetCopyStatus - {0}", ex.ToString()));
				}
				if (copyStatus != null && copyStatus.Length > 0)
				{
					foreach (CopyStatusClientCachedEntry copyStatusClientCachedEntry in copyStatus)
					{
						list.Add(new KeyValuePair<Guid, CopyStatusClientCachedEntry>(copyStatusClientCachedEntry.DbGuid, copyStatusClientCachedEntry));
					}
				}
				return list.ToArray();
			}, this.DefaultTimeout);
			this.allCopiesStatusForDatabaseCachedList = new CachedList<List<CopyStatusClientCachedEntry>, Guid>(delegate(Guid guid)
			{
				IADDatabaseAvailabilityGroup localDAG = CachedAdReader.Instance.LocalDAG;
				List<AmServerName> list = new List<AmServerName>();
				if (localDAG == null || localDAG.Servers == null || localDAG.Servers.Count < 1)
				{
					list.Add(AmServerName.LocalComputerName);
				}
				else
				{
					MailboxDatabase mailboxDatabaseFromGuid = DirectoryAccessor.Instance.GetMailboxDatabaseFromGuid(guid);
					if (mailboxDatabaseFromGuid == null)
					{
						throw new InvalidOperationException(string.Format("Database with GUID '{0}' is not found.", guid));
					}
					DatabaseCopy[] databaseCopies = mailboxDatabaseFromGuid.GetDatabaseCopies();
					foreach (DatabaseCopy databaseCopy in databaseCopies)
					{
						AmServerName item = new AmServerName(databaseCopy.HostServer);
						list.Add(item);
					}
				}
				Dictionary<Guid, Dictionary<AmServerName, CopyStatusClientCachedEntry>> copyStatusForDatabaseInternal = CachedDbStatusReader.GetCopyStatusForDatabaseInternal(new Guid[]
				{
					guid
				}, list);
				List<CopyStatusClientCachedEntry> list2 = new List<CopyStatusClientCachedEntry>();
				if (copyStatusForDatabaseInternal == null || copyStatusForDatabaseInternal.Count < 1)
				{
					return list2;
				}
				Dictionary<AmServerName, CopyStatusClientCachedEntry> dictionary = null;
				if (!copyStatusForDatabaseInternal.TryGetValue(guid, out dictionary))
				{
					return list2;
				}
				foreach (KeyValuePair<AmServerName, CopyStatusClientCachedEntry> keyValuePair in dictionary)
				{
					list2.Add(keyValuePair.Value);
				}
				return list2;
			}, this.DefaultTimeout);
		}

		public static CachedDbStatusReader Instance
		{
			get
			{
				if (CachedDbStatusReader.cachedDbStatusReaderInstance == null)
				{
					lock (CachedDbStatusReader.instanceCreationLock)
					{
						CachedDbStatusReader.cachedDbStatusReaderInstance = new CachedDbStatusReader();
					}
				}
				return CachedDbStatusReader.cachedDbStatusReaderInstance;
			}
		}

		public CopyStatusClientCachedEntry GetDbCopyStatusOnLocalServer(Guid mdbGuid)
		{
			return this.localCopyStatusCachedList.GetValue(mdbGuid);
		}

		public KeyValuePair<Guid, CopyStatusClientCachedEntry>[] GetDbsCopyStatusOnLocalServer(params Guid[] mdbGuids)
		{
			return this.localCopyStatusCachedList.GetValues(mdbGuids);
		}

		public List<CopyStatusClientCachedEntry> GetAllCopyStatusesForDatabase(Guid mdbGuid)
		{
			return this.allCopiesStatusForDatabaseCachedList.GetValue(mdbGuid);
		}

		private static Dictionary<Guid, Dictionary<AmServerName, CopyStatusClientCachedEntry>> GetCopyStatusForDatabaseInternal(Guid[] listOfMdbGuids, List<AmServerName> listOfTargetServers)
		{
			AmMultiNodeCopyStatusFetcher amMultiNodeCopyStatusFetcher = new AmMultiNodeCopyStatusFetcher(listOfTargetServers, listOfMdbGuids, null, RpcGetDatabaseCopyStatusFlags2.None, null, true);
			return amMultiNodeCopyStatusFetcher.GetStatus();
		}

		public readonly TimeSpan DefaultTimeout;

		private static CachedDbStatusReader cachedDbStatusReaderInstance = null;

		private static object instanceCreationLock = new object();

		private CachedList<CopyStatusClientCachedEntry, Guid> localCopyStatusCachedList;

		private CachedList<List<CopyStatusClientCachedEntry>, Guid> allCopiesStatusForDatabaseCachedList;
	}
}
