using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Cluster.ActiveManagerServer;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Rpc.ActiveManager;
using Microsoft.Win32;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ClusterBatchWriter
	{
		private ClusterBatchWriter()
		{
			this.batchWriterStartTime = ExDateTime.Now;
			this.firstLocalUpdateTime = ExDateTime.MinValue;
			this.mostRecentValidServerUpdateTimeFromPamUtc = SharedHelper.DateTimeMinValueUtc;
			this.m_clusdbUpdates = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			this.m_writerTimer = new Timer(new TimerCallback(this.WriterProc), null, RegistryParameters.ClusterBatchWriterIntervalInMsec, RegistryParameters.ClusterBatchWriterIntervalInMsec);
			ExTraceGlobals.ClusterTracer.TraceDebug((long)this.GetHashCode(), "ClusterBatchWriter instantiated");
		}

		public static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.ClusterTracer;
			}
		}

		internal static void Start()
		{
			if (ClusterBatchWriter.s_clusterBatchWriter == null)
			{
				ClusterBatchWriter.s_clusterBatchWriter = new ClusterBatchWriter();
			}
		}

		internal static void Stop()
		{
			if (ClusterBatchWriter.s_clusterBatchWriter != null)
			{
				ClusterBatchWriter.s_clusterBatchWriter.Shutdown();
				ClusterBatchWriter.s_clusterBatchWriter = null;
			}
		}

		internal static void SetLastLog(Guid dbGuid, long value)
		{
			if (ClusterBatchWriter.s_clusterBatchWriter != null)
			{
				lock (ClusterBatchWriter.s_clusterBatchWriter.s_lock)
				{
					long num;
					if (!ClusterBatchWriter.s_clusterBatchWriter.TryGetLastLog(dbGuid, out num) || value > num)
					{
						ClusterBatchWriter.s_clusterBatchWriter.Set(dbGuid.ToString(), value.ToString());
					}
				}
			}
		}

		private void Shutdown()
		{
			this.m_writerTimer.Change(-1, -1);
			lock (this.s_lock)
			{
				this.m_shutdownEvent = new ManualResetEvent(false);
			}
			this.m_writerTimer.Dispose(this.m_shutdownEvent);
			ExTraceGlobals.ClusterTracer.TraceDebug((long)this.GetHashCode(), "ClusterBatchWriter shutdown waiting for m_shutdownEvent to be signaled.");
			this.m_shutdownEvent.WaitOne();
			this.m_shutdownEvent.Close();
			this.m_writerTimer = null;
			ExTraceGlobals.ClusterTracer.TraceDebug((long)this.GetHashCode(), "ClusterBatchWriter shutdown completed.");
		}

		private bool TryGetLastLog(Guid dbGuid, out long lastLogGen)
		{
			lastLogGen = 0L;
			string s;
			bool flag = this.m_clusdbUpdates.TryGetValue(dbGuid.ToString(), out s);
			if (flag && !long.TryParse(s, out lastLogGen))
			{
				flag = false;
			}
			return flag;
		}

		private void Set(string key, string value)
		{
			lock (this.s_lock)
			{
				this.m_clusdbUpdates[key] = value;
			}
		}

		private void WriterProc(object o)
		{
			ExTraceGlobals.ClusterTracer.TraceDebug((long)this.GetHashCode(), "ClusterBatchWriter.WriterProc entered.");
			try
			{
				if (!DagHelper.IsLocalNodeClustered())
				{
					ClusterBatchWriter.Tracer.Information(0L, "The local machine is not clustered. Skipping cluster batch writer proc.");
					return;
				}
			}
			catch (ExClusTransientException arg)
			{
				ClusterBatchWriter.Tracer.Information<ExClusTransientException>(0L, "ClusterBatchWriter.WriterProc failed to determine clustering: {0}", arg);
				return;
			}
			lock (this.s_lock)
			{
				if (this.m_fRunning)
				{
					ExTraceGlobals.ClusterTracer.TraceDebug((long)this.GetHashCode(), "ClusterBatchWriter WriterProc bailing as another thread is running.");
					return;
				}
				this.m_fRunning = true;
			}
			try
			{
				Dictionary<string, string> clusdbUpdates;
				lock (this.s_lock)
				{
					ExTraceGlobals.ClusterTracer.TraceDebug<int>((long)this.GetHashCode(), "ClusterBatchWriter m_clusdbUpdates.Count={0}.", this.m_clusdbUpdates.Count);
					clusdbUpdates = this.m_clusdbUpdates;
					this.m_clusdbUpdates = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
				}
				bool flag3 = this.SendUpdatesAndCheckIfLocalUpdateRequired(clusdbUpdates);
				if (flag3)
				{
					ActiveManagerServerPerfmon.LastLogLocalClusterBatchUpdatesAttempted.Increment();
					if (!this.UpdateClusdb(clusdbUpdates))
					{
						ActiveManagerServerPerfmon.LastLogLocalClusterBatchUpdatesFailed.Increment();
						lock (this.s_lock)
						{
							foreach (string key in clusdbUpdates.Keys)
							{
								if (!this.m_clusdbUpdates.ContainsKey(key))
								{
									this.m_clusdbUpdates[key] = clusdbUpdates[key];
								}
							}
							ExTraceGlobals.ClusterTracer.TraceDebug<int>((long)this.GetHashCode(), "ClusterBatchWriter UpdateClusdb failed. m_clusdbUpdates.Count={0}.", this.m_clusdbUpdates.Count);
						}
					}
				}
			}
			finally
			{
				lock (this.s_lock)
				{
					this.m_fRunning = false;
				}
			}
			ExTraceGlobals.ClusterTracer.TraceDebug((long)this.GetHashCode(), "ClusterBatchWriter.WriterProc completed.");
		}

		private bool SendUpdatesAndCheckIfLocalUpdateRequired(Dictionary<string, string> clusdbUpdates)
		{
			AmConfig cfg = AmSystemManager.Instance.Config;
			Exception ex = null;
			if (cfg.IsPamOrSam)
			{
				ex = AmHelper.HandleKnownExceptions(delegate(object param0, EventArgs param1)
				{
					ActiveManagerServerPerfmon.LastLogRemoteUpdateRpcAttempted.Increment();
					DateTime t = RpcUpdateLastLogImpl.Send(AmServerName.LocalComputerName, cfg.DagConfig.CurrentPAM, clusdbUpdates);
					if (t > this.mostRecentValidServerUpdateTimeFromPamUtc)
					{
						this.mostRecentValidServerUpdateTimeFromPamUtc = t;
					}
				});
			}
			if (ex != null)
			{
				ActiveManagerServerPerfmon.LastLogRemoteUpdateRpcFailed.Increment();
				ReplayCrimsonEvents.LastLogUpdateRpcFailed.LogPeriodic<AmRole, DateTime, string>(AmServerName.LocalComputerName.NetbiosName, TimeSpan.FromMinutes(5.0), cfg.Role, this.mostRecentValidServerUpdateTimeFromPamUtc, ex.Message);
			}
			ExDateTime lastServerUpdateTimeAsPerClusdbUtc;
			bool flag = this.IsLocalUpdateRequired(out lastServerUpdateTimeAsPerClusdbUtc);
			this.LogUpdateEvents(flag, lastServerUpdateTimeAsPerClusdbUtc, ex);
			return flag;
		}

		private void LogUpdateEvents(bool isLocalUpdateRequired, ExDateTime lastServerUpdateTimeAsPerClusdbUtc, Exception exception)
		{
			ExDateTime now = ExDateTime.Now;
			string text = (exception != null) ? exception.Message : "<none>";
			if (!isLocalUpdateRequired)
			{
				if (this.firstLocalUpdateTime != ExDateTime.MinValue)
				{
					ReplayCrimsonEvents.SwitchingBackToPamClusterDatabaseUpdates.Log<ExDateTime, ExDateTime, DateTime, ExDateTime, string>(this.firstLocalUpdateTime, lastServerUpdateTimeAsPerClusdbUtc, this.mostRecentValidServerUpdateTimeFromPamUtc, this.batchWriterStartTime, text);
					this.firstLocalUpdateTime = ExDateTime.MinValue;
				}
				return;
			}
			if (this.firstLocalUpdateTime == ExDateTime.MinValue)
			{
				this.firstLocalUpdateTime = now;
				ReplayCrimsonEvents.InitiatingClusdbUpdatesFromLocalServer.Log<ExDateTime, ExDateTime, DateTime, ExDateTime, string>(this.firstLocalUpdateTime, lastServerUpdateTimeAsPerClusdbUtc, this.mostRecentValidServerUpdateTimeFromPamUtc, this.batchWriterStartTime, text);
				return;
			}
			ReplayCrimsonEvents.ContinuingLocalClusdbUpdates.LogPeriodic<ExDateTime, ExDateTime, DateTime, ExDateTime, string>(AmServerName.LocalComputerName.NetbiosName, TimeSpan.FromMinutes(5.0), this.firstLocalUpdateTime, lastServerUpdateTimeAsPerClusdbUtc, this.mostRecentValidServerUpdateTimeFromPamUtc, this.batchWriterStartTime, text);
		}

		private bool IsLocalUpdateRequired(out ExDateTime lastUpdatedTimeInClusdbUtc)
		{
			lastUpdatedTimeInClusdbUtc = ClusterBatchWriter.GetLastServerUpdateTimeFromClusdb();
			if (lastUpdatedTimeInClusdbUtc == SharedHelper.ExDateTimeMinValueUtc)
			{
				return true;
			}
			TimeSpan t = ExDateTime.UtcNow - lastUpdatedTimeInClusdbUtc;
			TimeSpan t2 = TimeSpan.FromSeconds((double)RegistryParameters.LastLogUpdateThresholdInSec);
			return t > t2;
		}

		internal static ExDateTime GetLastServerUpdateTimeFromClusdb()
		{
			ExDateTime exDateTimeMinValueUtc = SharedHelper.ExDateTimeMinValueUtc;
			try
			{
				using (IClusterDB clusterDB = ClusterDB.Open())
				{
					string value = clusterDB.GetValue<string>("ExchangeActiveManager\\LastLog", AmServerName.LocalComputerName.NetbiosName, string.Empty);
					if (!string.IsNullOrEmpty(value) && !ExDateTime.TryParse(value, out exDateTimeMinValueUtc))
					{
						exDateTimeMinValueUtc = SharedHelper.ExDateTimeMinValueUtc;
					}
				}
			}
			catch (ClusterException ex)
			{
				ClusterBatchWriter.Tracer.TraceError<string>(0L, "GetLastServerUpdateTimeFromClusdb() failed with {0}", ex.Message);
			}
			return exDateTimeMinValueUtc;
		}

		private bool IsMountingServerTheLocalServer(string dbGuidStr)
		{
			Guid dbGuid = new Guid(dbGuidStr);
			AmConfig config = AmSystemManager.Instance.Config;
			if (config.IsUnknown)
			{
				ClusterBatchWriter.Tracer.TraceError(0L, "ClusterBatchWriter finds AmConfig is unknown.");
				return false;
			}
			AmDbStateInfo amDbStateInfo = config.DbState.Read(dbGuid);
			return amDbStateInfo.IsActiveServerValid && amDbStateInfo.ActiveServer.IsLocalComputerName;
		}

		private bool UpdateClusdb(Dictionary<string, string> tmpClusdbUpdates)
		{
			string text = "Start";
			try
			{
				this.UpdateClusdbInternal(tmpClusdbUpdates, out text);
				return true;
			}
			catch (ClusterException ex)
			{
				ReplayCrimsonEvents.LastLogBatchUpdateFailed.Log<string, string>(text, ex.Message);
			}
			return false;
		}

		private void UpdateClusdbInternal(Dictionary<string, string> tmpClusdbUpdates, out string lastAttemptedOperationName)
		{
			lastAttemptedOperationName = "Preparing";
			Dictionary<string, string> clusdbUpdates = this.PrepareUpdates(tmpClusdbUpdates);
			lastAttemptedOperationName = "OpenCluster";
			using (AmCluster amCluster = AmCluster.Open())
			{
				lastAttemptedOperationName = "GetClusterKey";
				using (IDistributedStoreKey clusterKey = DistributedStore.Instance.GetClusterKey(amCluster.Handle, null, null, DxStoreKeyAccessMode.Write, false))
				{
					lastAttemptedOperationName = "OpenAmRootKey";
					using (IDistributedStoreKey distributedStoreKey = clusterKey.OpenKey("ExchangeActiveManager", DxStoreKeyAccessMode.Write, false, null))
					{
						lastAttemptedOperationName = "OpenAmRootKey";
						using (IDistributedStoreKey distributedStoreKey2 = distributedStoreKey.OpenKey("LastLog", DxStoreKeyAccessMode.CreateIfNotExist, false, null))
						{
							lastAttemptedOperationName = "CreateBatch";
							using (IDistributedStoreBatchRequest distributedStoreBatchRequest = distributedStoreKey2.CreateBatchUpdateRequest())
							{
								lastAttemptedOperationName = "PopulateBatch";
								this.PopulateBatch(distributedStoreKey2, distributedStoreBatchRequest, clusdbUpdates);
								lastAttemptedOperationName = "ExecuteBatch";
								distributedStoreBatchRequest.Execute(null);
							}
						}
					}
				}
			}
		}

		private Dictionary<string, string> PrepareUpdates(Dictionary<string, string> tmpClusdbUpdates)
		{
			IEnumerable<IADDatabase> databasesOnLocalServer = Dependencies.ADConfig.GetDatabasesOnLocalServer();
			if (databasesOnLocalServer != null)
			{
				foreach (IADDatabase iaddatabase in databasesOnLocalServer)
				{
					string key = iaddatabase.Guid.ToString();
					string text = null;
					if (!tmpClusdbUpdates.TryGetValue(key, out text))
					{
						tmpClusdbUpdates[key] = null;
					}
				}
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (string text2 in tmpClusdbUpdates.Keys)
			{
				if (this.IsMountingServerTheLocalServer(text2))
				{
					dictionary[text2] = tmpClusdbUpdates[text2];
				}
				else
				{
					ClusterBatchWriter.Tracer.TraceError<string>(0L, "ClusterBatchWriter skipping update for db {0} because active status not confirmed", text2);
				}
			}
			return dictionary;
		}

		private void PopulateBatch(IDistributedStoreKey stateHandle, IDistributedStoreBatchRequest batchHandle, Dictionary<string, string> clusdbUpdates)
		{
			string propertyValue = ExDateTime.UtcNow.ToString("s");
			foreach (string text in clusdbUpdates.Keys)
			{
				bool flag = false;
				long num = 0L;
				long num2 = 0L;
				bool flag2 = false;
				string text2 = clusdbUpdates[text];
				string value = stateHandle.GetValue(text, null, out flag, null);
				if (flag)
				{
					flag2 = true;
				}
				if (text2 != null && (!flag || !long.TryParse(text2, out num) || !long.TryParse(value, out num2) || num > num2))
				{
					ClusterBatchWriter.Tracer.TraceError<string, long, long>(0L, "ClusterBatchWriter prepping update for db {0} from {1} to {2}", text, num2, num);
					batchHandle.SetValue(text, text2, RegistryValueKind.Unknown);
					flag2 = true;
				}
				if (flag2)
				{
					string propertyName = AmDbState.ConstructLastLogTimeStampProperty(text);
					batchHandle.SetValue(propertyName, propertyValue, RegistryValueKind.Unknown);
				}
			}
			batchHandle.SetValue(AmServerName.LocalComputerName.NetbiosName, propertyValue, RegistryValueKind.Unknown);
		}

		private const string AmRootKeyName = "ExchangeActiveManager";

		internal const string CopyStateKeyName = "LastLog";

		internal const string CopyStateFullKeyName = "ExchangeActiveManager\\LastLog";

		private static ClusterBatchWriter s_clusterBatchWriter;

		private object s_lock = new object();

		private Dictionary<string, string> m_clusdbUpdates;

		private Timer m_writerTimer;

		private ManualResetEvent m_shutdownEvent;

		private bool m_fRunning;

		private ExDateTime firstLocalUpdateTime;

		private ExDateTime batchWriterStartTime;

		private DateTime mostRecentValidServerUpdateTimeFromPamUtc;
	}
}
