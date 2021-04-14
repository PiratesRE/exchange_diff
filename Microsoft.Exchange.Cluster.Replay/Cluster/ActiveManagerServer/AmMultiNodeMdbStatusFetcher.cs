using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.HA;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmMultiNodeMdbStatusFetcher : AmMultiNodeRpcMap
	{
		internal Dictionary<AmServerName, MdbStatus[]> MdbStatusMap { get; private set; }

		internal Dictionary<AmServerName, AmMdbStatusServerInfo> ServerInfoMap { get; private set; }

		private static Dictionary<AmServerName, AmMdbStatusServerInfo> GetMultiNodeServerInfo(AmConfig cfg, List<AmServerName> serversList)
		{
			Dictionary<AmServerName, AmMdbStatusServerInfo> dictionary = new Dictionary<AmServerName, AmMdbStatusServerInfo>();
			foreach (AmServerName amServerName in serversList)
			{
				if (!cfg.IsUnknown)
				{
					if (cfg.IsDebugOptionsEnabled() && cfg.IsIgnoreServerDebugOptionEnabled(amServerName))
					{
						AmTrace.Warning("Server {0} is ignored from batch mounter operation since debug option {1} is enabled", new object[]
						{
							amServerName.NetbiosName,
							AmDebugOptions.IgnoreServerFromAutomaticActions.ToString()
						});
					}
					else if (cfg.IsStandalone || cfg.DagConfig.IsNodePubliclyUp(amServerName))
					{
						dictionary[amServerName] = new AmMdbStatusServerInfo(amServerName, true, TimeSpan.FromSeconds((double)RegistryParameters.MdbStatusFetcherServerUpTimeoutInSec));
					}
					else if (RegistryParameters.TransientFailoverSuppressionDelayInSec > 0)
					{
						dictionary[amServerName] = new AmMdbStatusServerInfo(amServerName, false, TimeSpan.FromSeconds((double)RegistryParameters.MdbStatusFetcherServerDownTimeoutInSec));
					}
				}
			}
			return dictionary;
		}

		internal AmMultiNodeMdbStatusFetcher() : base("AmMultiNodeMdbStatusFetcher")
		{
		}

		private void Initialize(AmConfig cfg, List<AmServerName> serverList, bool isBasicInformation)
		{
			Dictionary<AmServerName, AmMdbStatusServerInfo> multiNodeServerInfo = AmMultiNodeMdbStatusFetcher.GetMultiNodeServerInfo(cfg, serverList);
			List<AmServerName> nodeList = multiNodeServerInfo.Keys.ToList<AmServerName>();
			base.Initialize(nodeList);
			this.ServerInfoMap = multiNodeServerInfo;
			this.m_isBasicInformation = isBasicInformation;
			this.MdbStatusMap = new Dictionary<AmServerName, MdbStatus[]>(this.m_expectedCount);
		}

		internal void Start(AmConfig cfg, Func<List<AmServerName>> getServersFunc)
		{
			ThreadPool.QueueUserWorkItem(delegate(object param0)
			{
				this.Initialize(cfg, getServersFunc(), true);
				this.Run();
			});
		}

		internal void Run()
		{
			lock (this.m_locker)
			{
				if (!this.m_isRunning)
				{
					this.m_isRunning = true;
					base.RunAllRpcs();
				}
			}
			this.WaitUntilStatusIsReady();
		}

		internal void WaitUntilStatusIsReady()
		{
			TimeSpan timeout = TimeSpan.FromSeconds((double)(RegistryParameters.MdbStatusFetcherServerUpTimeoutInSec + 15));
			base.WaitForCompletion(timeout);
		}

		internal override void TestInitialState()
		{
			base.TestInitialState();
			DiagCore.RetailAssert(this.MdbStatusMap != null, "this.MdbStatusMap should not be null.", new object[0]);
			DiagCore.RetailAssert(this.MdbStatusMap.Count == 0, "this.MdbStatusMap should be 0 at the start.", new object[0]);
		}

		internal override void TestFinalState()
		{
			base.TestFinalState();
			DiagCore.RetailAssert(base.IsTimedout || this.MdbStatusMap.Count == this.m_expectedCount, "this.MdbStatusMap should have m_expectedCount entries.", new object[0]);
		}

		protected override Exception RunServerRpc(AmServerName node, out object result)
		{
			Exception result2 = null;
			Exception storeException = null;
			MdbStatus[] mdbStatuses = null;
			result = null;
			AmMdbStatusServerInfo amMdbStatusServerInfo = this.ServerInfoMap[node];
			bool flag = false;
			try
			{
				InvokeWithTimeout.Invoke(delegate()
				{
					storeException = this.RunServerRpcInternal(node, out mdbStatuses);
				}, amMdbStatusServerInfo.TimeOut);
			}
			catch (TimeoutException ex)
			{
				result2 = ex;
				flag = true;
			}
			if (!flag)
			{
				result2 = storeException;
				amMdbStatusServerInfo.IsStoreRunning = true;
				amMdbStatusServerInfo.IsReplayRunning = AmHelper.IsReplayRunning(node);
				result = mdbStatuses;
			}
			return result2;
		}

		private Exception RunServerRpcInternal(AmServerName node, out MdbStatus[] results)
		{
			results = null;
			Exception ex = null;
			MdbStatus[] array = null;
			if (!AmStoreHelper.GetAllDatabaseStatuses(node, this.m_isBasicInformation, out array))
			{
				Thread.Sleep(2000);
				if (!AmStoreHelper.GetAllDatabaseStatuses(node, this.m_isBasicInformation, out array, out ex))
				{
					AmTrace.Error("Failed to get mounted database information from store on server {0}. Exception: {1}", new object[]
					{
						node,
						ex
					});
					ReplayCrimsonEvents.ListMdbStatusFailed.LogPeriodic<string, string>(node.NetbiosName, TimeSpan.FromMinutes(15.0), node.NetbiosName, ex.ToString());
					return ex;
				}
			}
			results = array;
			return ex;
		}

		protected override void UpdateStatus(AmServerName node, object result)
		{
			this.MdbStatusMap[node] = (MdbStatus[])result;
		}

		private object m_locker = new object();

		private bool m_isRunning;

		private bool m_isBasicInformation;
	}
}
