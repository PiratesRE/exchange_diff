using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class CopyStatusClientLookupTable : ReaderWriterLockedBase
	{
		public CopyStatusClientCachedEntry GetCopyStatusCachedEntry(Guid dbGuid, AmServerName server)
		{
			CopyStatusClientCachedEntry status = null;
			base.ReaderLockedOperation(delegate
			{
				status = this.GetCopyStatusCachedEntryNoLock(dbGuid, server);
			});
			return status;
		}

		public List<CopyStatusClientCachedEntry> GetCopyStatusCachedEntriesByDatabase(Guid dbGuid)
		{
			List<CopyStatusClientCachedEntry> statuses = null;
			Dictionary<AmServerName, CopyStatusClientCachedEntry> statusTable = null;
			base.ReaderLockedOperation(delegate
			{
				this.m_dbServerStatuses.TryGetValue(dbGuid, out statusTable);
				if (statusTable != null)
				{
					foreach (KeyValuePair<AmServerName, CopyStatusClientCachedEntry> keyValuePair in statusTable)
					{
						if (statuses == null)
						{
							statuses = new List<CopyStatusClientCachedEntry>(statusTable.Count);
						}
						statuses.Add(keyValuePair.Value);
					}
				}
			});
			return statuses;
		}

		public List<CopyStatusClientCachedEntry> GetCopyStatusCachedEntriesByServer(AmServerName server)
		{
			List<CopyStatusClientCachedEntry> statuses = null;
			Dictionary<Guid, CopyStatusClientCachedEntry> statusTable = null;
			base.ReaderLockedOperation(delegate
			{
				this.m_serverDbStatuses.TryGetValue(server, out statusTable);
				if (statusTable != null)
				{
					foreach (KeyValuePair<Guid, CopyStatusClientCachedEntry> keyValuePair in statusTable)
					{
						if (statuses == null)
						{
							statuses = new List<CopyStatusClientCachedEntry>(statusTable.Count);
						}
						statuses.Add(keyValuePair.Value);
					}
				}
			});
			return statuses;
		}

		public CopyStatusClientCachedEntry AddCopyStatusCachedEntry(Guid dbGuid, AmServerName server, CopyStatusClientCachedEntry status)
		{
			CopyStatusClientCachedEntry returnEntry = null;
			base.WriterLockedOperation(delegate
			{
				returnEntry = this.AddCopyStatusCachedEntryNoLock(dbGuid, server, status);
			});
			return returnEntry;
		}

		public IEnumerable<CopyStatusClientCachedEntry> AddCopyStatusCachedEntriesForServer(AmServerName server, IEnumerable<CopyStatusClientCachedEntry> statusEntries)
		{
			int capacity = statusEntries.Count<CopyStatusClientCachedEntry>();
			List<CopyStatusClientCachedEntry> returnEntries = new List<CopyStatusClientCachedEntry>(capacity);
			base.WriterLockedOperation(delegate
			{
				foreach (CopyStatusClientCachedEntry copyStatusClientCachedEntry in statusEntries)
				{
					returnEntries.Add(this.AddCopyStatusCachedEntryNoLock(copyStatusClientCachedEntry.DbGuid, server, copyStatusClientCachedEntry));
				}
			});
			return returnEntries;
		}

		public void UpdateCopyStatusCachedEntries(Dictionary<Guid, Dictionary<AmServerName, CopyStatusClientCachedEntry>> dbServerStatuses)
		{
			base.WriterLockedOperation(delegate
			{
				foreach (Guid key in dbServerStatuses.Keys)
				{
					Dictionary<AmServerName, CopyStatusClientCachedEntry> dictionary = dbServerStatuses[key];
					if (dictionary != null)
					{
						foreach (KeyValuePair<AmServerName, CopyStatusClientCachedEntry> keyValuePair in dictionary)
						{
							this.AddCopyStatusCachedEntryNoLock(keyValuePair.Value.DbGuid, keyValuePair.Key, keyValuePair.Value);
						}
					}
				}
			});
		}

		private CopyStatusClientCachedEntry GetCopyStatusCachedEntryNoLock(Guid dbGuid, AmServerName server)
		{
			CopyStatusClientCachedEntry result = null;
			if (this.m_dbServerStatuses.ContainsKey(dbGuid))
			{
				Dictionary<AmServerName, CopyStatusClientCachedEntry> dictionary = this.m_dbServerStatuses[dbGuid];
				dictionary.TryGetValue(server, out result);
			}
			return result;
		}

		private CopyStatusClientCachedEntry AddCopyStatusCachedEntryNoLock(Guid dbGuid, AmServerName server, CopyStatusClientCachedEntry status)
		{
			CopyStatusClientCachedEntry copyStatusCachedEntryNoLock = this.GetCopyStatusCachedEntryNoLock(dbGuid, server);
			CopyStatusClientCachedEntry result = copyStatusCachedEntryNoLock;
			if (CopyStatusHelper.CheckCopyStatusNewer(status, copyStatusCachedEntryNoLock))
			{
				this.AddCopyStatusToDbTable(dbGuid, server, status);
				this.AddCopyStatusToServerTable(dbGuid, server, status);
				result = status;
			}
			return result;
		}

		private void AddCopyStatusToServerTable(Guid dbGuid, AmServerName server, CopyStatusClientCachedEntry status)
		{
			Dictionary<Guid, CopyStatusClientCachedEntry> dictionary = null;
			if (!this.m_serverDbStatuses.TryGetValue(server, out dictionary))
			{
				dictionary = new Dictionary<Guid, CopyStatusClientCachedEntry>(48);
				this.m_serverDbStatuses[server] = dictionary;
			}
			dictionary[dbGuid] = status;
		}

		private void AddCopyStatusToDbTable(Guid dbGuid, AmServerName server, CopyStatusClientCachedEntry status)
		{
			Dictionary<AmServerName, CopyStatusClientCachedEntry> dictionary = null;
			if (!this.m_dbServerStatuses.TryGetValue(dbGuid, out dictionary))
			{
				dictionary = new Dictionary<AmServerName, CopyStatusClientCachedEntry>(5);
				this.m_dbServerStatuses[dbGuid] = dictionary;
			}
			dictionary[server] = status;
		}

		internal const int InitialActiveDatabasesPerServerCapacity = 20;

		internal const int InitialDatabaseCopiesPerServerCapacity = 48;

		internal const int InitialDatabaseCopiesPerDbCapacity = 5;

		internal const int InitialServersCapacity = 16;

		internal const int InitialDatabasesPerDagCapacity = 160;

		private Dictionary<Guid, Dictionary<AmServerName, CopyStatusClientCachedEntry>> m_dbServerStatuses = new Dictionary<Guid, Dictionary<AmServerName, CopyStatusClientCachedEntry>>(160);

		private Dictionary<AmServerName, Dictionary<Guid, CopyStatusClientCachedEntry>> m_serverDbStatuses = new Dictionary<AmServerName, Dictionary<Guid, CopyStatusClientCachedEntry>>(16);
	}
}
