using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery
{
	public abstract class CoordinatedRecoveryAction
	{
		public CoordinatedRecoveryAction(RecoveryActionId actionId, string requester, int minimumRequiredTobeInReadyState, int maximumConcurrentActionsAllowed, string[] resources)
		{
			this.CancellationTokenSource = new CancellationTokenSource();
			this.TraceContext = TracingContext.Default;
			this.Requester = requester;
			this.Resources = resources;
			this.statusMap = new Dictionary<string, ResourceAvailabilityStatus>(resources.Length);
			foreach (string key in resources)
			{
				this.statusMap.Add(key, ResourceAvailabilityStatus.Unknown);
			}
			this.ActionId = actionId;
			this.MinimumRequiredTobeInReadyState = minimumRequiredTobeInReadyState;
			this.MaximumConcurrentActionsAllowed = maximumConcurrentActionsAllowed;
			WTFDiagnostics.TraceDebug<string, string, int, int, int>(ExTraceGlobals.RecoveryActionTracer, this.TraceContext, "Created an instance of {0}. (requester: {1}, resourceCount: {2}, readyMinimum: {3}, concurrentMax: {4})", base.GetType().Name, this.Requester, this.statusMap.Count, this.MinimumRequiredTobeInReadyState, this.MaximumConcurrentActionsAllowed, null, ".ctor", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Recovery\\CoordinatedRecoveryAction.cs", 126);
		}

		public RecoveryActionId ActionId { get; set; }

		internal int MinimumRequiredTobeInReadyState { get; set; }

		internal int MaximumConcurrentActionsAllowed { get; set; }

		internal string Requester { get; private set; }

		internal string[] Resources { get; private set; }

		private protected TracingContext TraceContext { protected get; private set; }

		private protected CancellationTokenSource CancellationTokenSource { protected get; private set; }

		public void Execute(TimeSpan arbitrationTimeout, Action<CoordinatedRecoveryAction.ResourceAvailabilityStatistics> action)
		{
			CoordinatedRecoveryAction.ResourceAvailabilityStatistics stats = null;
			RecoveryActionThrottlingMode throttlingMode = RecoveryActionHelper.GetRecoveryActionDistributedThrottlingMode(this.ActionId, RecoveryActionThrottlingMode.None);
			if (throttlingMode == RecoveryActionThrottlingMode.None)
			{
				this.totalRequests = this.statusMap.Count;
				Task[] tasks = new Task[this.totalRequests];
				int num = 0;
				bool isParallelMode = true;
				object serialExecutionLock = new object();
				WTFDiagnostics.TraceDebug<string>(ExTraceGlobals.RecoveryActionTracer, this.TraceContext, "{0}: Initiating arbitration", base.GetType().Name, null, "Execute", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Recovery\\CoordinatedRecoveryAction.cs", 192);
				string[] array = this.statusMap.Keys.ToArray<string>();
				string[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					string resourceName2 = array2[i];
					string resourceName = resourceName2;
					Task task = Task.Factory.StartNew(delegate()
					{
						if (isParallelMode)
						{
							this.ExecutePerResource(resourceName);
							return;
						}
						object serialExecutionLock;
						lock (serialExecutionLock)
						{
							this.ExecutePerResource(resourceName);
						}
					});
					tasks[num++] = task;
				}
				bool isTimedout = false;
				RecoveryActionHelper.RunAndMeasure(string.Format("WaitAll (resourceCount: {0} timeout: {1})", tasks.Length, arbitrationTimeout), false, ManagedAvailabilityCrimsonEvents.MeasureOperation, delegate
				{
					isTimedout = Task.WaitAll(tasks, (int)arbitrationTimeout.TotalMilliseconds, this.CancellationTokenSource.Token);
					if (isTimedout)
					{
						WTFDiagnostics.TraceDebug(ExTraceGlobals.RecoveryActionTracer, this.TraceContext, "WaitAll() timed out", null, "Execute", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Recovery\\CoordinatedRecoveryAction.cs", 240);
					}
					return string.Format("isTimeout = {0}", isTimedout);
				});
				lock (this.locker)
				{
					this.isMajorityCheckCompleted = true;
				}
				stats = this.GetConsolidatedStatistics();
				this.EnsureArbitrationSucceeeded(stats);
			}
			RecoveryActionHelper.RunAndMeasure(string.Format("Running the coordinated action of {0} - Throttling mode: {1}", base.GetType().Name, throttlingMode), true, ManagedAvailabilityCrimsonEvents.MeasureOperation, delegate
			{
				if (throttlingMode != RecoveryActionThrottlingMode.ForceFail)
				{
					action(stats);
					return string.Empty;
				}
				throw new DistributedThrottlingRejectedOperationException(this.ActionId.ToString(), this.Requester);
			});
		}

		public virtual void EnsureArbitrationSucceeeded(CoordinatedRecoveryAction.ResourceAvailabilityStatistics stats)
		{
			Exception ex = null;
			if (stats.TotalReady < this.MinimumRequiredTobeInReadyState)
			{
				WTFDiagnostics.TraceError<int, int, int, int>(ExTraceGlobals.RecoveryActionTracer, this.TraceContext, "Arbitration failed since number of ready resoures not meeting the minimum requirement. (ready={0}, minimumReady={1}, concurrent={2}, maximumConcurrent={3})", stats.TotalReady, this.MinimumRequiredTobeInReadyState, stats.TotalArbitrating, this.MaximumConcurrentActionsAllowed, null, "EnsureArbitrationSucceeeded", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Recovery\\CoordinatedRecoveryAction.cs", 291);
				ex = new ArbitrationMinimumRequiredReadyNotSatisfiedException(stats.TotalReady, this.MinimumRequiredTobeInReadyState);
			}
			else if (stats.TotalArbitrating > this.MaximumConcurrentActionsAllowed)
			{
				WTFDiagnostics.TraceError<int, int, int, int>(ExTraceGlobals.RecoveryActionTracer, this.TraceContext, "Arbitration failed since number of concurrent operations exceeded the maximum requirement. (ready={0}, minimumReady={1}, concurrent={2}, maximumConcurrent={3})", stats.TotalReady, this.MinimumRequiredTobeInReadyState, stats.TotalArbitrating, this.MaximumConcurrentActionsAllowed, null, "EnsureArbitrationSucceeeded", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Recovery\\CoordinatedRecoveryAction.cs", 306);
				ex = new ArbitrationMaximumAllowedConcurrentNotSatisfiedException(stats.TotalArbitrating, this.MaximumConcurrentActionsAllowed);
			}
			else
			{
				WTFDiagnostics.TraceDebug<int, int, int, int>(ExTraceGlobals.RecoveryActionTracer, this.TraceContext, "Arbitration succeeded. (ready={0}, minimumReady={1}, concurrent={2}, maximumConcurrent={3})", stats.TotalReady, this.MinimumRequiredTobeInReadyState, stats.TotalArbitrating, this.MaximumConcurrentActionsAllowed, null, "EnsureArbitrationSucceeeded", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Recovery\\CoordinatedRecoveryAction.cs", 321);
			}
			string text = string.Join(",", stats.ReadyResources);
			string text2 = string.Join(",", stats.ArbitratingResources);
			string text3 = string.Join(",", stats.InMaintenanceResources);
			string text4 = string.Join(",", stats.UnknownResources);
			string text5 = string.Join(",", stats.OfflineResources);
			ManagedAvailabilityCrimsonEvent managedAvailabilityCrimsonEvent = ManagedAvailabilityCrimsonEvents.ArbitrationSucceeded;
			if (ex != null)
			{
				managedAvailabilityCrimsonEvent = ManagedAvailabilityCrimsonEvents.ArbitrationFailed;
			}
			managedAvailabilityCrimsonEvent.LogGeneric(new object[]
			{
				base.GetType().Name,
				this.Requester,
				stats.TotalRequested,
				stats.TotalReady,
				stats.TotalArbitrating,
				stats.TotalMaintenance,
				stats.TotalUnknown,
				this.MinimumRequiredTobeInReadyState,
				this.MaximumConcurrentActionsAllowed,
				text,
				text2,
				text3,
				text4,
				(ex != null) ? ex.Message : string.Empty,
				stats.TotalOffline,
				text5
			});
			if (ex != null)
			{
				throw ex;
			}
		}

		internal string GetShortName(string fqdn)
		{
			string[] array = fqdn.Split(new char[]
			{
				'.'
			});
			return array[0];
		}

		internal CoordinatedRecoveryAction.ResourceAvailabilityStatistics GetConsolidatedStatistics()
		{
			CoordinatedRecoveryAction.ResourceAvailabilityStatistics resourceAvailabilityStatistics = new CoordinatedRecoveryAction.ResourceAvailabilityStatistics();
			resourceAvailabilityStatistics.TotalRequested = this.statusMap.Count;
			resourceAvailabilityStatistics.ReadyResources = new List<string>();
			resourceAvailabilityStatistics.ArbitratingResources = new List<string>();
			resourceAvailabilityStatistics.UnknownResources = new List<string>();
			resourceAvailabilityStatistics.InMaintenanceResources = new List<string>();
			resourceAvailabilityStatistics.OfflineResources = new List<string>();
			foreach (KeyValuePair<string, ResourceAvailabilityStatus> keyValuePair in this.statusMap)
			{
				switch (keyValuePair.Value)
				{
				case ResourceAvailabilityStatus.Ready:
					resourceAvailabilityStatistics.TotalReady++;
					resourceAvailabilityStatistics.ReadyResources.Add(this.GetShortName(keyValuePair.Key));
					continue;
				case ResourceAvailabilityStatus.Arbitrating:
					resourceAvailabilityStatistics.TotalArbitrating++;
					resourceAvailabilityStatistics.ArbitratingResources.Add(this.GetShortName(keyValuePair.Key));
					continue;
				case ResourceAvailabilityStatus.Maintenance:
					resourceAvailabilityStatistics.TotalMaintenance++;
					resourceAvailabilityStatistics.InMaintenanceResources.Add(this.GetShortName(keyValuePair.Key));
					continue;
				case ResourceAvailabilityStatus.Offline:
					resourceAvailabilityStatistics.TotalOffline++;
					resourceAvailabilityStatistics.OfflineResources.Add(this.GetShortName(keyValuePair.Key));
					continue;
				}
				resourceAvailabilityStatistics.TotalUnknown++;
				resourceAvailabilityStatistics.UnknownResources.Add(this.GetShortName(keyValuePair.Key));
			}
			WTFDiagnostics.TraceDebug(ExTraceGlobals.RecoveryActionTracer, this.TraceContext, string.Format("GetConsolidatedStatistics(). (Requested: {0} Ready: {1}, Busy: {2}, Maintenance: {3}, Unknown: {4}, Offline:{5})", new object[]
			{
				resourceAvailabilityStatistics.TotalRequested,
				resourceAvailabilityStatistics.TotalReady,
				resourceAvailabilityStatistics.TotalArbitrating,
				resourceAvailabilityStatistics.TotalMaintenance,
				resourceAvailabilityStatistics.TotalUnknown,
				resourceAvailabilityStatistics.TotalOffline
			}), null, "GetConsolidatedStatistics", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Recovery\\CoordinatedRecoveryAction.cs", 425);
			return resourceAvailabilityStatistics;
		}

		internal void ExecutePerResource(string resourceName)
		{
			WTFDiagnostics.TraceDebug<string, int>(ExTraceGlobals.RecoveryActionTracer, this.TraceContext, "RunCheck: Task started for {0} (ManagedThreadId: {1})", resourceName, Thread.CurrentThread.ManagedThreadId, null, "ExecutePerResource", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Recovery\\CoordinatedRecoveryAction.cs", 446);
			lock (this.locker)
			{
				if (this.IsArbitrationCompleted())
				{
					WTFDiagnostics.TraceDebug<string>(ExTraceGlobals.RecoveryActionTracer, this.TraceContext, "Skipping the work since completion is marked. (resourceName: {0})", resourceName, null, "ExecutePerResource", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Recovery\\CoordinatedRecoveryAction.cs", 457);
					return;
				}
			}
			ResourceAvailabilityStatus status = ResourceAvailabilityStatus.Unknown;
			RecoveryActionHelper.RunAndMeasure(string.Format("RunCheck({0})", resourceName), false, ManagedAvailabilityCrimsonEvents.MeasureOperation, delegate
			{
				string text = string.Empty;
				status = this.RunCheck(resourceName, out text);
				text = string.Format("{0} => {1} :", resourceName, status) + text;
				return text;
			});
			this.UpdateStatus(resourceName, status);
			if (this.IsArbitrationCompleted() && !this.CancellationTokenSource.IsCancellationRequested)
			{
				this.CancellationTokenSource.Cancel();
			}
		}

		protected virtual bool IsArbitrationCompleted()
		{
			return this.isMajorityCheckCompleted;
		}

		protected abstract ResourceAvailabilityStatus RunCheck(string resourceName, out string debugInfo);

		private void UpdateStatus(string resourceName, ResourceAvailabilityStatus status)
		{
			lock (this.locker)
			{
				if (!this.IsArbitrationCompleted())
				{
					this.statusMap[resourceName] = status;
					if (status == ResourceAvailabilityStatus.Arbitrating)
					{
						this.inArbitrationCount++;
						if (this.inArbitrationCount > this.MaximumConcurrentActionsAllowed)
						{
							this.isMajorityCheckCompleted = true;
						}
					}
					else if (status == ResourceAvailabilityStatus.Ready)
					{
						this.readyCount++;
						if (this.readyCount >= this.MinimumRequiredTobeInReadyState)
						{
							this.isMajorityCheckCompleted = true;
						}
					}
				}
			}
		}

		private object locker = new object();

		private Dictionary<string, ResourceAvailabilityStatus> statusMap;

		private int totalRequests;

		private int readyCount;

		private int inArbitrationCount;

		private bool isMajorityCheckCompleted;

		public class ResourceAvailabilityStatistics
		{
			internal int TotalRequested { get; set; }

			internal int TotalReady { get; set; }

			internal int TotalArbitrating { get; set; }

			internal int TotalUnknown { get; set; }

			internal int TotalMaintenance { get; set; }

			internal int TotalOffline { get; set; }

			internal List<string> ReadyResources { get; set; }

			internal List<string> ArbitratingResources { get; set; }

			internal List<string> UnknownResources { get; set; }

			internal List<string> InMaintenanceResources { get; set; }

			internal List<string> OfflineResources { get; set; }

			public string GetStatisticsAsString()
			{
				return string.Format("ResourceAvailabilityStatistics: (Requested: {0} Ready: {1}, Busy: {2}, Maintenance: {3}, Unknown: {4}, Offline:{5})", new object[]
				{
					this.TotalRequested,
					this.TotalReady,
					this.TotalArbitrating,
					this.TotalMaintenance,
					this.TotalUnknown,
					this.TotalOffline
				});
			}
		}
	}
}
