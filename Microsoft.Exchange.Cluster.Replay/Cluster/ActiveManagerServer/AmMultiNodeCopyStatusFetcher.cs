using System;
using System.Collections.Generic;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.HA;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmMultiNodeCopyStatusFetcher : AmMultiNodeRpcMap
	{
		public AmMultiNodeCopyStatusFetcher(List<AmServerName> nodeList, Dictionary<AmServerName, IEnumerable<IADDatabase>> databasesMap, RpcGetDatabaseCopyStatusFlags2 rpcFlags, ActiveManager activeManager, bool isGetHealthStates) : this(nodeList, databasesMap, rpcFlags, activeManager, isGetHealthStates, RegistryParameters.BCSGetCopyStatusRPCTimeoutInMSec)
		{
		}

		public AmMultiNodeCopyStatusFetcher(List<AmServerName> nodeList, Guid[] mdbGuids, Dictionary<AmServerName, IEnumerable<IADDatabase>> databasesMap, RpcGetDatabaseCopyStatusFlags2 rpcFlags, ActiveManager activeManager, bool isGetHealthStates = false) : this(nodeList, mdbGuids, databasesMap, rpcFlags, activeManager, RegistryParameters.BCSGetCopyStatusRPCTimeoutInMSec, isGetHealthStates)
		{
		}

		public AmMultiNodeCopyStatusFetcher(List<AmServerName> nodeList, Dictionary<AmServerName, IEnumerable<IADDatabase>> databasesMap, RpcGetDatabaseCopyStatusFlags2 rpcFlags, ActiveManager activeManager, bool isGetHealthStates, int rpcTimeoutInMs) : this(nodeList, null, databasesMap, rpcFlags, activeManager, rpcTimeoutInMs, isGetHealthStates)
		{
		}

		public AmMultiNodeCopyStatusFetcher(List<AmServerName> nodeList, Guid[] mdbGuids, Dictionary<AmServerName, IEnumerable<IADDatabase>> databasesMap, RpcGetDatabaseCopyStatusFlags2 rpcFlags, ActiveManager activeManager, int rpcTimeoutInMs, bool isGetHealthStates = false) : base(nodeList, "AmMultiNodeCopyStatusFetcher")
		{
			this.m_rpcTimeoutInMs = rpcTimeoutInMs;
			this.m_mdbGuids = mdbGuids;
			this.m_databaseMap = databasesMap;
			this.m_rpcFlags = rpcFlags;
			this.m_activeManager = activeManager;
			this.m_isGetHealthStates = isGetHealthStates;
			this.m_healthStateTable = new Dictionary<AmServerName, RpcHealthStateInfo[]>(16);
			if (mdbGuids == null || mdbGuids.Length == 0)
			{
				this.m_copyStatusMap = new Dictionary<Guid, Dictionary<AmServerName, CopyStatusClientCachedEntry>>(160);
				return;
			}
			this.m_copyStatusMap = new Dictionary<Guid, Dictionary<AmServerName, CopyStatusClientCachedEntry>>(mdbGuids.Length);
		}

		internal Dictionary<Guid, Dictionary<AmServerName, CopyStatusClientCachedEntry>> GetStatus()
		{
			return this.GetStatus(InvokeWithTimeout.InfiniteTimeSpan);
		}

		internal Dictionary<Guid, Dictionary<AmServerName, CopyStatusClientCachedEntry>> GetStatus(TimeSpan timeout)
		{
			Dictionary<AmServerName, RpcHealthStateInfo[]> dictionary = null;
			return this.GetStatus(timeout, out dictionary);
		}

		internal Dictionary<Guid, Dictionary<AmServerName, CopyStatusClientCachedEntry>> GetStatus(out Dictionary<AmServerName, RpcHealthStateInfo[]> healthStateTable)
		{
			return this.GetStatus(InvokeWithTimeout.InfiniteTimeSpan, out healthStateTable);
		}

		internal Dictionary<Guid, Dictionary<AmServerName, CopyStatusClientCachedEntry>> GetStatus(TimeSpan timeout, out Dictionary<AmServerName, RpcHealthStateInfo[]> healthStateTable)
		{
			healthStateTable = null;
			if (this.m_copyStatusMap.Count > 0)
			{
				healthStateTable = this.m_healthStateTable;
				return this.m_copyStatusMap;
			}
			base.RunAllRpcs(timeout);
			healthStateTable = this.m_healthStateTable;
			this.Cleanup();
			return this.m_copyStatusMap;
		}

		internal virtual bool TryGetCopyStatus(AmServerName node, Guid[] mdbGuids, out CopyStatusClientCachedEntry[] results, out RpcHealthStateInfo[] healthStates, out Exception exception)
		{
			int timeoutMs = (this.m_rpcTimeoutInMs > 0) ? this.m_rpcTimeoutInMs : RegistryParameters.BCSGetCopyStatusRPCTimeoutInMSec;
			results = null;
			healthStates = null;
			exception = null;
			if (mdbGuids != null)
			{
				results = CopyStatusHelper.GetCopyStatus(node, this.m_rpcFlags, mdbGuids, timeoutMs, this.m_activeManager, this.m_isGetHealthStates, out healthStates, out exception);
			}
			else
			{
				results = CopyStatusHelper.GetAllCopyStatuses(node, this.m_rpcFlags, this.m_databaseMap[node], timeoutMs, this.m_activeManager, this.m_isGetHealthStates, out healthStates, out exception);
			}
			bool flag = exception == null;
			if (!flag)
			{
				ExTraceGlobals.ActiveManagerTracer.TraceError<AmServerName, Exception>(0L, "AmMultiNodeCopyStatusFetcher: GetCopyStatus RPC to server '{0}' failed with ex: {1}", node, exception);
			}
			return flag;
		}

		internal override void TestInitialState()
		{
			base.TestInitialState();
			DiagCore.RetailAssert(this.m_copyStatusMap != null, "m_copyStatusMap should not be null at the start.", new object[0]);
			DiagCore.RetailAssert(this.m_copyStatusMap.Count == 0, "m_copyStatusMap.Count should be 0 at the start.", new object[0]);
		}

		protected override Exception RunServerRpc(AmServerName node, out object result)
		{
			result = null;
			Exception ex = null;
			CopyStatusClientCachedEntry[] key = null;
			RpcHealthStateInfo[] value = null;
			bool flag = this.TryGetCopyStatus(node, this.m_mdbGuids, out key, out value, out ex);
			DiagCore.AssertOrWatson(flag || ex != null, "ex cannot be null when TryGetCopyStatus() returns false!", new object[0]);
			DiagCore.AssertOrWatson(!flag || ex == null, "ex has to be null when TryGetCopyStatus() returns true! Actual: {0}", new object[]
			{
				ex
			});
			result = new KeyValuePair<CopyStatusClientCachedEntry[], RpcHealthStateInfo[]>(key, value);
			return ex;
		}

		protected override void UpdateStatus(AmServerName node, object result)
		{
			KeyValuePair<CopyStatusClientCachedEntry[], RpcHealthStateInfo[]> keyValuePair = (KeyValuePair<CopyStatusClientCachedEntry[], RpcHealthStateInfo[]>)result;
			CopyStatusClientCachedEntry[] key = keyValuePair.Key;
			RpcHealthStateInfo[] value = keyValuePair.Value;
			if (key != null && key.Length > 0)
			{
				foreach (CopyStatusClientCachedEntry copyStatusClientCachedEntry in key)
				{
					Guid dbGuid = copyStatusClientCachedEntry.DbGuid;
					if (this.m_copyStatusMap.ContainsKey(dbGuid))
					{
						Dictionary<AmServerName, CopyStatusClientCachedEntry> dictionary = this.m_copyStatusMap[dbGuid];
						dictionary[node] = copyStatusClientCachedEntry;
					}
					else
					{
						Dictionary<AmServerName, CopyStatusClientCachedEntry> dictionary2 = new Dictionary<AmServerName, CopyStatusClientCachedEntry>(5);
						dictionary2[node] = copyStatusClientCachedEntry;
						this.m_copyStatusMap[dbGuid] = dictionary2;
					}
				}
			}
			this.m_healthStateTable[node] = value;
		}

		private const int NumberOfDbCopiesHint = 5;

		private readonly bool m_isGetHealthStates;

		protected Dictionary<Guid, Dictionary<AmServerName, CopyStatusClientCachedEntry>> m_copyStatusMap;

		protected Dictionary<AmServerName, RpcHealthStateInfo[]> m_healthStateTable;

		private Guid[] m_mdbGuids;

		private ActiveManager m_activeManager;

		private readonly int m_rpcTimeoutInMs;

		private Dictionary<AmServerName, IEnumerable<IADDatabase>> m_databaseMap;

		private RpcGetDatabaseCopyStatusFlags2 m_rpcFlags;
	}
}
