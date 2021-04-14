using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.HA;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class CopyStatusPoller : TimerComponent, ICopyStatusPoller
	{
		private static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.MonitoringTracer;
			}
		}

		public CopyStatusPoller(IMonitoringADConfigProvider adConfigProvider, CopyStatusClientLookupTable statusTable, ActiveManager activeManager) : base(TimeSpan.Zero, CopyStatusPoller.CopyStatusPollerInterval, "CopyStatusPoller")
		{
			this.m_adConfigProvider = adConfigProvider;
			this.m_statusTable = statusTable;
			this.m_activeManager = activeManager;
			this.m_rpcThreadsInProgress = new ConcurrentDictionary<AmServerName, bool>();
		}

		public bool TryWaitForInitialization()
		{
			TimeSpan timeout = TimeSpan.FromMilliseconds((double)RegistryParameters.GetMailboxDatabaseCopyStatusRPCTimeoutInMSec);
			return this.m_firstPollCompleted.WaitOne(timeout) == ManualOneShotEvent.Result.Success;
		}

		protected override void TimerCallbackInternal()
		{
			try
			{
				this.Run();
			}
			catch (MonitoringADConfigException ex)
			{
				CopyStatusPoller.Tracer.TraceError<MonitoringADConfigException>((long)this.GetHashCode(), "CopyStatusPoller: Got exception when retrieving AD config: {0}", ex);
				ReplayCrimsonEvents.CopyStatusPollerError.LogPeriodic<string, MonitoringADConfigException>(this.GetHashCode(), DiagCore.DefaultEventSuppressionInterval, ex.Message, ex);
			}
			finally
			{
				this.m_firstPollCompleted.Set();
			}
		}

		protected override void StopInternal()
		{
			base.StopInternal();
			this.m_firstPollCompleted.Close();
		}

		private void Run()
		{
			IMonitoringADConfig config = this.m_adConfigProvider.GetConfig(true);
			AmMultiNodeCopyStatusFetcher_ForPoller amMultiNodeCopyStatusFetcher_ForPoller = new AmMultiNodeCopyStatusFetcher_ForPoller(config, this.m_activeManager, this.m_rpcThreadsInProgress);
			Dictionary<Guid, Dictionary<AmServerName, CopyStatusClientCachedEntry>> status = amMultiNodeCopyStatusFetcher_ForPoller.GetStatus(CopyStatusPoller.CopyStatusPollerInterval);
			if (status != null)
			{
				this.m_statusTable.UpdateCopyStatusCachedEntries(status);
			}
		}

		public const RpcGetDatabaseCopyStatusFlags2 GetCopyStatusRpcFlags = RpcGetDatabaseCopyStatusFlags2.None;

		private const string FirstCopyStatusPollCompletedEventName = "FirstCopyStatusPollCompletedEvent";

		private static readonly TimeSpan CopyStatusPollerInterval = TimeSpan.FromMilliseconds((double)RegistryParameters.CopyStatusPollerIntervalInMsec);

		private ManualOneShotEvent m_firstPollCompleted = new ManualOneShotEvent("FirstCopyStatusPollCompletedEvent");

		private CopyStatusClientLookupTable m_statusTable;

		private IMonitoringADConfigProvider m_adConfigProvider;

		private ActiveManager m_activeManager;

		private ConcurrentDictionary<AmServerName, bool> m_rpcThreadsInProgress;
	}
}
