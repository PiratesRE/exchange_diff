using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class CopyStatusClientLookup : ICopyStatusClientLookup
	{
		private static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.MonitoringTracer;
			}
		}

		public CopyStatusClientLookup(CopyStatusClientLookupTable statusTable, ICopyStatusPoller statusPoller, ActiveManager activeManager)
		{
			this.m_statusTable = statusTable;
			this.m_statusPoller = statusPoller;
			this.m_activeManager = activeManager;
			this.m_cachingEnabled = (statusTable != null);
		}

		public IEnumerable<CopyStatusClientCachedEntry> GetCopyStatusesByDatabase(Guid dbGuid, IEnumerable<AmServerName> servers, CopyStatusClientLookupFlags flags)
		{
			if (servers == null)
			{
				return new List<CopyStatusClientCachedEntry>();
			}
			List<CopyStatusClientCachedEntry> list = null;
			if (!this.IsReadThroughNeeded(flags))
			{
				list = this.m_statusTable.GetCopyStatusCachedEntriesByDatabase(dbGuid);
			}
			if (list == null)
			{
				list = new List<CopyStatusClientCachedEntry>(servers.Count<AmServerName>());
			}
			using (IEnumerator<AmServerName> enumerator = servers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					AmServerName server = enumerator.Current;
					if (!list.Any((CopyStatusClientCachedEntry status) => server.Equals(status.ServerContacted)))
					{
						list.Add(this.GetCopyStatusCachedEntry(dbGuid, server, flags));
					}
				}
			}
			return (from status in list
			where servers.Contains(status.ServerContacted)
			select status).ToList<CopyStatusClientCachedEntry>();
		}

		public IEnumerable<CopyStatusClientCachedEntry> GetCopyStatusesByServer(AmServerName server, IEnumerable<IADDatabase> expectedDatabases, CopyStatusClientLookupFlags flags)
		{
			if (expectedDatabases == null)
			{
				return new List<CopyStatusClientCachedEntry>();
			}
			List<CopyStatusClientCachedEntry> list = null;
			if (!this.IsReadThroughNeeded(flags))
			{
				if (this.m_statusPoller != null)
				{
					this.m_statusPoller.TryWaitForInitialization();
				}
				list = this.m_statusTable.GetCopyStatusCachedEntriesByServer(server);
			}
			if (list == null)
			{
				list = new List<CopyStatusClientCachedEntry>(expectedDatabases.Count<IADDatabase>());
			}
			Dictionary<Guid, CopyStatusClientCachedEntry> tempStatusTable = list.ToDictionary((CopyStatusClientCachedEntry status) => status.DbGuid);
			Dictionary<Guid, IADDatabase> expectedDbsTable = expectedDatabases.ToDictionary((IADDatabase db) => db.Guid);
			if (expectedDatabases.All((IADDatabase db) => tempStatusTable.ContainsKey(db.Guid)))
			{
				return (from status in list
				where expectedDbsTable.ContainsKey(status.DbGuid)
				select status).ToList<CopyStatusClientCachedEntry>();
			}
			Exception ex = null;
			return this.FetchAllCopyStatusesFromRpc(server, expectedDatabases, ref ex);
		}

		public CopyStatusClientCachedEntry GetCopyStatus(Guid dbGuid, AmServerName server, CopyStatusClientLookupFlags flags)
		{
			return this.GetCopyStatusCachedEntry(dbGuid, server, flags);
		}

		public CopyStatusClientCachedEntry GetCopyStatusCachedEntry(Guid dbGuid, AmServerName server, CopyStatusClientLookupFlags flags)
		{
			CopyStatusClientCachedEntry copyStatusClientCachedEntry = null;
			Exception ex = null;
			bool flag = this.IsReadThroughNeeded(flags);
			if (!flag)
			{
				if (this.m_statusPoller != null)
				{
					this.m_statusPoller.TryWaitForInitialization();
				}
				copyStatusClientCachedEntry = this.m_statusTable.GetCopyStatusCachedEntry(dbGuid, server);
				if (copyStatusClientCachedEntry == null)
				{
					flag = true;
					CopyStatusClientLookup.Tracer.TraceDebug<Guid, AmServerName>((long)this.GetHashCode(), "CopyStatusClientLookup.GetCopyStatus for DB '{0}' to server '{1}' hit a cache miss! Performing a read through instead!", dbGuid, server);
				}
			}
			if (flag)
			{
				copyStatusClientCachedEntry = this.FetchCopyStatusFromRpc(dbGuid, server, ref ex);
			}
			return copyStatusClientCachedEntry;
		}

		protected virtual CopyStatusClientCachedEntry FetchCopyStatusFromRpc(Guid dbGuid, AmServerName server, ref Exception exception)
		{
			CopyStatusClientLookup.Tracer.TraceDebug<Guid, AmServerName>((long)this.GetHashCode(), "CopyStatusClientLookup.GetCopyStatus() for DB '{0}' to server '{1}': Performing a forced read through!", dbGuid, server);
			CopyStatusClientCachedEntry[] copyStatus = CopyStatusHelper.GetCopyStatus(server, RpcGetDatabaseCopyStatusFlags2.None, new Guid[]
			{
				dbGuid
			}, RegistryParameters.GetMailboxDatabaseCopyStatusRPCTimeoutInMSec, this.m_activeManager, out exception);
			CopyStatusClientCachedEntry copyStatusClientCachedEntry = copyStatus[0];
			if (this.m_cachingEnabled)
			{
				copyStatusClientCachedEntry = this.m_statusTable.AddCopyStatusCachedEntry(dbGuid, server, copyStatusClientCachedEntry);
			}
			return copyStatusClientCachedEntry;
		}

		protected virtual IEnumerable<CopyStatusClientCachedEntry> FetchAllCopyStatusesFromRpc(AmServerName server, IEnumerable<IADDatabase> expectedDatabases, ref Exception exception)
		{
			int arg = expectedDatabases.Count<IADDatabase>();
			CopyStatusClientLookup.Tracer.TraceDebug<AmServerName, int>((long)this.GetHashCode(), "CopyStatusClientLookup.GetCopyStatusesByServer() for server '{0}': Performing a forced read through, expecting {1} database copy results!", server, arg);
			RpcHealthStateInfo[] array;
			CopyStatusClientCachedEntry[] allCopyStatuses = CopyStatusHelper.GetAllCopyStatuses(server, RpcGetDatabaseCopyStatusFlags2.None, expectedDatabases, RegistryParameters.GetMailboxDatabaseCopyStatusRPCTimeoutInMSec, this.m_activeManager, false, out array, out exception);
			IEnumerable<CopyStatusClientCachedEntry> result = allCopyStatuses;
			if (this.m_cachingEnabled)
			{
				result = this.m_statusTable.AddCopyStatusCachedEntriesForServer(server, allCopyStatuses);
			}
			return result;
		}

		private bool IsReadThroughNeeded(CopyStatusClientLookupFlags flags)
		{
			return (flags & CopyStatusClientLookupFlags.ReadThrough) != CopyStatusClientLookupFlags.None || !this.m_cachingEnabled;
		}

		private readonly bool m_cachingEnabled;

		private CopyStatusClientLookupTable m_statusTable;

		private ICopyStatusPoller m_statusPoller;

		private ActiveManager m_activeManager;
	}
}
