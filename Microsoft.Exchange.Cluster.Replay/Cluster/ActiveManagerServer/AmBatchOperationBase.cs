using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage.ActiveManager;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal abstract class AmBatchOperationBase
	{
		internal AmBatchOperationBase()
		{
			this.CustomStatus = AmDbActionStatus.None;
			this.m_amConfig = AmSystemManager.Instance.Config;
		}

		internal BatchOperationCompletedDelegate CompletionCallback { get; set; }

		internal AmDbActionStatus CustomStatus { get; set; }

		internal bool IsAllDone
		{
			get
			{
				bool isAllDone;
				lock (this.m_locker)
				{
					isAllDone = this.m_isAllDone;
				}
				return isAllDone;
			}
		}

		internal void MarkAllDone()
		{
			lock (this.m_locker)
			{
				AmTrace.Debug("{0} finished processing all the databases", new object[]
				{
					base.GetType().Name
				});
				if (!this.m_isAllDone)
				{
					this.m_isAllDone = true;
					this.LogCompletionInternal();
					if (this.CompletionCallback != null)
					{
						this.CompletionCallback(new List<AmDbOperation>(this.m_opList));
					}
					ThreadPoolThreadCountHelper.Reset();
				}
				else
				{
					AmTrace.Debug("IsAllDone() is already marked and completionback was already called. Possibly custom status is reached already.", new object[0]);
				}
			}
		}

		internal void Run()
		{
			AmTrace.Entering("AmBatchOperationBase.Run()", new object[0]);
			if (RegistryParameters.AmDisableBatchOperations)
			{
				ReplayCrimsonEvents.BatchMounterOperationsDisabled.Log();
				AmTrace.Leaving("AmBatchOperationBase.Run(), BatchMounterOperationsDisabled", new object[0]);
				return;
			}
			lock (this.m_locker)
			{
				try
				{
					this.m_isDebugOptionEnabled = this.m_amConfig.IsDebugOptionsEnabled();
					this.LogStartupInternal();
					Exception ex = AmHelper.HandleKnownExceptions(delegate(object param0, EventArgs param1)
					{
						this.RunInternal();
					});
					if (ex != null)
					{
						AmTrace.Error("Batch mounter operation {0} got an exception {1}", new object[]
						{
							base.GetType().Name,
							ex
						});
						ReplayCrimsonEvents.BatchMounterOperationFailed.Log<string>(ex.Message);
					}
				}
				finally
				{
					if (!this.m_derivedManagesAllDone && (this.m_opList == null || this.m_opList.Count == 0))
					{
						this.MarkAllDone();
					}
				}
			}
			AmTrace.Leaving("AmBatchOperationBase.Run()", new object[0]);
		}

		internal AmDbCompletionReason Wait(TimeSpan timeout)
		{
			AmTrace.Debug("Waiting for {0} to complete (timeout={1})", new object[]
			{
				base.GetType().Name,
				timeout
			});
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			AmDbCompletionReason result;
			while (!AmSystemManager.Instance.IsShutdown)
			{
				if (stopwatch.Elapsed > timeout)
				{
					result = AmDbCompletionReason.Timedout;
				}
				else
				{
					if (!this.IsAllDone)
					{
						Thread.Sleep(50);
						continue;
					}
					result = AmDbCompletionReason.Finished;
				}
				IL_6E:
				AmTrace.Debug("{0}.Wait completed with the reason {1}", new object[]
				{
					base.GetType().Name,
					timeout
				});
				return result;
			}
			result = AmDbCompletionReason.Cancelled;
			goto IL_6E;
		}

		internal AmDbCompletionReason Wait()
		{
			return this.Wait(TimeSpan.MaxValue);
		}

		protected void EnqueueDatabaseOperationBatch(Guid dbGuid, List<AmDbOperation> operationList)
		{
			this.m_totalBatchOperationsQueued++;
			foreach (AmDbOperation operation in operationList)
			{
				this.FixDatabaseOperation(operation);
			}
			this.m_opList.AddRange(operationList);
		}

		protected void StartDatabaseOperationBatch(Guid dbGuid, List<AmDbOperation> operationList)
		{
			AmDatabaseQueueManager databaseQueueManager = AmSystemManager.Instance.DatabaseQueueManager;
			if (!databaseQueueManager.Enqueue(dbGuid, operationList, false))
			{
				foreach (AmDbOperation opr in operationList)
				{
					this.DecrementCounters(opr);
				}
			}
		}

		protected void EnqueueDatabaseOperation(AmDbOperation operation)
		{
			this.m_totalSingleOperationsQueued++;
			this.FixDatabaseOperation(operation);
			this.m_opList.Add(operation);
		}

		protected void StartDatabaseOperations()
		{
			AmDatabaseQueueManager databaseQueueManager = AmSystemManager.Instance.DatabaseQueueManager;
			foreach (AmDbOperation opr in this.m_opList)
			{
				if (!databaseQueueManager.Enqueue(opr))
				{
					this.DecrementCounters(opr);
				}
			}
		}

		protected void DecrementCounters(AmDbOperation opr)
		{
			if (opr is AmDbMountOperation)
			{
				this.m_mountRequests--;
				return;
			}
			if (opr is AmDbDismountMismountedOperation)
			{
				this.m_dismountRequests--;
				return;
			}
			if (opr is AmDbClusterDatabaseSyncOperation)
			{
				this.m_clusDbSyncRequests--;
				return;
			}
			if (opr is AmDbAdPropertySyncOperation)
			{
				this.m_adSyncRequests--;
				return;
			}
			if (opr is AmDbMoveOperation)
			{
				this.m_moveRequests--;
			}
		}

		protected AmMultiNodeMdbStatusFetcher StartMdbStatusFetcher()
		{
			AmMultiNodeMdbStatusFetcher amMultiNodeMdbStatusFetcher = new AmMultiNodeMdbStatusFetcher();
			ThreadPoolThreadCountHelper.IncreaseForServerOperations(this.m_amConfig);
			amMultiNodeMdbStatusFetcher.Start(this.m_amConfig, new Func<List<AmServerName>>(this.GetServers));
			return amMultiNodeMdbStatusFetcher;
		}

		protected void AddDelayedFailoverEntryAsync(AmServerName nodeName, AmDbActionReason reasonCode)
		{
			ThreadPool.QueueUserWorkItem(delegate(object unused)
			{
				AmSystemManager.Instance.TransientFailoverSuppressor.AddEntry(reasonCode, nodeName);
			});
		}

		protected abstract List<AmServerName> GetServers();

		protected abstract void RunInternal();

		protected abstract void LogStartupInternal();

		protected abstract void LogCompletionInternal();

		private void FixDatabaseOperation(AmDbOperation operation)
		{
			operation.CustomStatus = this.CustomStatus;
			operation.CompletionCallback = (AmReportCompletionDelegate)Delegate.Combine(operation.CompletionCallback, new AmReportCompletionDelegate(this.OnOperationComplete));
		}

		private void OnOperationComplete(IADDatabase db)
		{
			lock (this.m_locker)
			{
				if (++this.m_totalOperationsCompleted == this.m_opList.Count)
				{
					this.MarkAllDone();
				}
			}
		}

		protected List<AmDbOperation> m_opList = new List<AmDbOperation>();

		private int m_totalSingleOperationsQueued;

		private int m_totalBatchOperationsQueued;

		protected int m_totalOperationsCompleted;

		protected AmConfig m_amConfig;

		protected bool m_isDebugOptionEnabled;

		protected bool m_derivedManagesAllDone;

		protected int m_mountRequests;

		protected int m_dismountRequests;

		protected int m_clusDbSyncRequests;

		protected int m_adSyncRequests;

		protected int m_moveRequests;

		private object m_locker = new object();

		private bool m_isAllDone;
	}
}
