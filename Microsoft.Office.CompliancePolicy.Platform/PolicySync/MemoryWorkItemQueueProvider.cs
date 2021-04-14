using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	public sealed class MemoryWorkItemQueueProvider : IWorkItemQueueProvider
	{
		public MemoryWorkItemQueueProvider(IWorkItemQueueProvider persistentQueueProvider, JobDispatcherBase jobDispatcher, SyncAgentContext syncAgentContext)
		{
			ArgumentValidator.ThrowIfNull("persistentQueueProvider", persistentQueueProvider);
			ArgumentValidator.ThrowIfNull("jobDispatcher", jobDispatcher);
			ArgumentValidator.ThrowIfNull("syncAgentContext", syncAgentContext);
			this.hostStateProvider = syncAgentContext.HostStateProvider;
			this.persistentQueueProvider = persistentQueueProvider;
			jobDispatcher.WorkItemQueue = this;
			this.logProvider = syncAgentContext.LogProvider;
			this.config = syncAgentContext.SyncAgentConfig;
			if (this.config.DispatcherTriggerInterval != null)
			{
				this.dispatchTimer = new Timer(new TimerCallback(jobDispatcher.Dispatch), null, TimeSpan.FromMilliseconds(-1.0), TimeSpan.FromMilliseconds(-1.0));
			}
		}

		public void Enqueue(WorkItemBase item)
		{
			ArgumentValidator.ThrowIfNull("item", item);
			if (this.hostStateProvider.IsShuttingDown())
			{
				SyncAgentTransientException ex = new SyncAgentTransientException("Queue is shutting down", false, SyncAgentErrorCode.EnqueueErrorShutDown);
				this.logProvider.LogOneEntry("UnifiedPolicySyncAgent", item.TenantContext.TenantId.ToString(), item.ExternalIdentity, ExecutionLog.EventType.Error, string.Empty, string.Format("In-memory {0} Queue is shutting down", item.GetType().Name), ex, new KeyValuePair<string, object>[0]);
				throw ex;
			}
			bool flag = false;
			try
			{
				object obj;
				Monitor.Enter(obj = this.syncObject, ref flag);
				this.logProvider.LogOneEntry("UnifiedPolicySyncAgent", item.TenantContext.TenantId.ToString(), item.ExternalIdentity, ExecutionLog.EventType.Information, string.Format("In-memory {0} Queue Length", item.GetType().Name), string.Format("In-memory {0} Queue Length: {1}", item.GetType().Name, this.internalQueue.Count()), null, new KeyValuePair<string, object>[0]);
				item.ResetStatus();
				if (item.ExecuteTimeUTC == default(DateTime))
				{
					item.ExecuteTimeUTC = DateTime.UtcNow + this.config.WorkItemExecuteDelayTime;
				}
				WorkItemBase similarItem = null;
				if (this.internalQueue.TryGetValue(item.GetPrimaryKey(), out similarItem))
				{
					if (similarItem.IsEqual(item))
					{
						if (item.HasPersistentBackUp)
						{
							Utility.DoWorkAndLogIfFail(delegate
							{
								this.persistentQueueProvider.Delete(item);
							}, this.logProvider, item.TenantContext.TenantId.ToString(), item.ExternalIdentity, ExecutionLog.EventType.Warning, string.Empty, string.Format("Duped {0} WI {1} of Tenant {2} can't be removed from the WI store", item.GetType().Name, item.WorkItemId, item.TenantContext.TenantId), false, false);
						}
						this.logProvider.LogOneEntry("UnifiedPolicySyncAgent", item.TenantContext.TenantId.ToString(), item.ExternalIdentity, ExecutionLog.EventType.Information, string.Empty, string.Format("{0} Notification {1} is de-duped due to the existence of Notification {2}", item.GetType().Name, item.ExternalIdentity, similarItem.ExternalIdentity), null, new KeyValuePair<string, object>[0]);
					}
					else
					{
						if (!similarItem.Merge(item))
						{
							SyncAgentTransientException ex2 = new SyncAgentTransientException("The item insertion has failed due to conflict", false, SyncAgentErrorCode.EnqueueErrorMergeConflict);
							this.logProvider.LogOneEntry("UnifiedPolicySyncAgent", item.TenantContext.TenantId.ToString(), item.ExternalIdentity, ExecutionLog.EventType.Error, string.Empty, string.Format("{0} Notification {1} fails to be merged into Notification {2} due to merge conflict", item.GetType().Name, item.ExternalIdentity, similarItem.ExternalIdentity), ex2, new KeyValuePair<string, object>[0]);
							throw ex2;
						}
						Utility.DoWorkAndLogIfFail(delegate
						{
							this.persistentQueueProvider.Update(similarItem);
						}, this.logProvider, item.TenantContext.TenantId.ToString(), item.ExternalIdentity, ExecutionLog.EventType.Error, string.Empty, string.Format("{0} WI {1} of Tenant {2} can't be saved to the WI store", item.GetType().Name, item.WorkItemId, item.TenantContext.TenantId), true, false);
						if (item.HasPersistentBackUp)
						{
							Utility.DoWorkAndLogIfFail(delegate
							{
								this.persistentQueueProvider.Delete(item);
							}, this.logProvider, item.TenantContext.TenantId.ToString(), item.ExternalIdentity, ExecutionLog.EventType.Warning, string.Empty, string.Format("Merged {0} WI {1} of Tenant {2} can't be removed from the WI store", item.GetType().Name, item.WorkItemId, item.TenantContext.TenantId), false, false);
						}
						this.internalQueue.Enqueue(similarItem);
						this.logProvider.LogOneEntry("UnifiedPolicySyncAgent", item.TenantContext.TenantId.ToString(), item.ExternalIdentity, ExecutionLog.EventType.Information, string.Empty, string.Format("{0} Notification {1} is merged into Notification {2}. The execution is scheduled at {3}.", new object[]
						{
							item.GetType().Name,
							item.ExternalIdentity,
							similarItem.ExternalIdentity,
							similarItem.ExecuteTimeUTC
						}), null, new KeyValuePair<string, object>[0]);
					}
				}
				else
				{
					if (this.IsFull())
					{
						SyncAgentTransientException ex3 = new SyncAgentTransientException("Queue is full", false, SyncAgentErrorCode.EnqueueErrorQueueFull);
						this.logProvider.LogOneEntry("UnifiedPolicySyncAgent", item.TenantContext.TenantId.ToString(), item.ExternalIdentity, ExecutionLog.EventType.Error, string.Empty, string.Format("In-memory {0} Queue is full", item.GetType().Name), ex3, new KeyValuePair<string, object>[0]);
						throw ex3;
					}
					if (!item.HasPersistentBackUp)
					{
						Utility.DoWorkAndLogIfFail(delegate
						{
							this.persistentQueueProvider.Enqueue(item);
						}, this.logProvider, item.TenantContext.TenantId.ToString(), item.ExternalIdentity, ExecutionLog.EventType.Error, string.Empty, string.Format("New {0} WI {1} of Tenant {2} can't be saved to the WI store", item.GetType().Name, item.WorkItemId, item.TenantContext.TenantId), true, false);
					}
					this.internalQueue.Enqueue(item);
					this.logProvider.LogOneEntry("UnifiedPolicySyncAgent", item.TenantContext.TenantId.ToString(), item.ExternalIdentity, ExecutionLog.EventType.Information, string.Empty, string.Format("New {0} WI {1} of Tenant {2} is enqueued. The execution is scheduled at {3}.", new object[]
					{
						item.GetType().Name,
						item.WorkItemId,
						item.TenantContext.TenantId,
						item.ExecuteTimeUTC
					}), null, new KeyValuePair<string, object>[0]);
					if (this.dispatchTimer != null && !this.isDispatchScheduled)
					{
						this.logProvider.LogOneEntry("UnifiedPolicySyncAgent", item.TenantContext.TenantId.ToString(), item.ExternalIdentity, ExecutionLog.EventType.Information, string.Empty, string.Format("The dispatcher is scheduled at {0}.", DateTime.UtcNow + this.config.DispatcherTriggerInterval.Value), null, new KeyValuePair<string, object>[0]);
						this.SetDispatcherSchedule(true);
					}
				}
			}
			finally
			{
				if (flag)
				{
					object obj;
					Monitor.Exit(obj);
				}
			}
		}

		public IList<WorkItemBase> Dequeue(int maxCount)
		{
			ArgumentValidator.ThrowIfZeroOrNegative("maxCount", maxCount);
			if (this.hostStateProvider.IsShuttingDown())
			{
				return null;
			}
			IList<WorkItemBase> result;
			lock (this.syncObject)
			{
				result = this.internalQueue.Dequeue(maxCount);
			}
			return result;
		}

		public IList<WorkItemBase> GetAll()
		{
			throw new NotSupportedException("GetAll() isn't supported by in-memory queue");
		}

		public bool IsEmpty()
		{
			if (this.hostStateProvider.IsShuttingDown())
			{
				return true;
			}
			bool result;
			lock (this.syncObject)
			{
				result = this.internalQueue.IsEmpty();
			}
			return result;
		}

		public void Update(WorkItemBase item)
		{
			throw new NotSupportedException("Update() isn't supported by in-memory queue");
		}

		public void Delete(WorkItemBase item)
		{
			throw new NotSupportedException("Delete() isn't supported by in-memory queue");
		}

		public void OnWorkItemCompleted(WorkItemBase item)
		{
			if (this.hostStateProvider.IsShuttingDown())
			{
				return;
			}
			WorkItemStatus status = item.Status;
			SyncAgentExceptionBase exception = (item.Errors != null && item.Errors.Any<SyncAgentExceptionBase>()) ? item.Errors.Last<SyncAgentExceptionBase>() : null;
			lock (this.syncObject)
			{
				this.logProvider.LogOneEntry("UnifiedPolicySyncAgent", item.TenantContext.TenantId.ToString(), item.ExternalIdentity, (status == WorkItemStatus.Fail) ? ExecutionLog.EventType.Error : ExecutionLog.EventType.Information, string.Empty, "Entered OnWorkItemCompleted", exception, new KeyValuePair<string, object>[0]);
				if (status == WorkItemStatus.Success || (WorkItemStatus.Fail == status && !this.config.RetryStrategy.CanRetry(item.TryCount)))
				{
					Utility.DoWorkAndLogIfFail(delegate
					{
						this.persistentQueueProvider.Delete(item);
					}, this.logProvider, item.TenantContext.TenantId.ToString(), item.ExternalIdentity, ExecutionLog.EventType.Warning, string.Empty, string.Format("{0} WI {1} of Tenant {2} can't be removed from the WI store", status, item.WorkItemId, item.TenantContext.TenantId), false, false);
				}
				else
				{
					item.ExecuteTimeUTC = DateTime.UtcNow + this.config.RetryStrategy.GetRetryInterval(item.TryCount);
					if (WorkItemStatus.NotStarted != status)
					{
						Utility.DoWorkAndLogIfFail(delegate
						{
							this.persistentQueueProvider.Update(item);
						}, this.logProvider, item.TenantContext.TenantId.ToString(), item.ExternalIdentity, ExecutionLog.EventType.Warning, string.Empty, string.Format("{0} WI {1} of Tenant {2} can't be saved to the WI store", status, item.WorkItemId, item.TenantContext.TenantId), false, false);
					}
					if (this.config.ReEnqueueNonSuccessWorkItem)
					{
						Utility.DoWorkAndLogIfFail(delegate
						{
							this.Enqueue(item);
						}, this.logProvider, item.TenantContext.TenantId.ToString(), item.ExternalIdentity, ExecutionLog.EventType.Error, string.Empty, string.Format("{0} WI {1} of Tenant {2}  can't be re-inserted back to the queue", status, item.WorkItemId, item.TenantContext.TenantId), false, false);
					}
				}
			}
		}

		public void OnAllWorkItemDispatched()
		{
			if (this.hostStateProvider.IsShuttingDown())
			{
				return;
			}
			lock (this.syncObject)
			{
				if (this.dispatchTimer != null)
				{
					this.SetDispatcherSchedule(!this.internalQueue.IsEmpty());
				}
			}
		}

		private void SetDispatcherSchedule(bool schedule)
		{
			if (schedule)
			{
				this.dispatchTimer.Change(this.config.DispatcherTriggerInterval.Value, TimeSpan.FromMilliseconds(-1.0));
			}
			else
			{
				this.dispatchTimer.Change(-1, -1);
			}
			this.isDispatchScheduled = schedule;
		}

		private bool IsFull()
		{
			return this.hostStateProvider.IsShuttingDown() || this.config.MaxQueueLength == this.internalQueue.Count();
		}

		private readonly object syncObject = new object();

		private readonly IndexedQueue internalQueue = new IndexedQueue();

		private readonly Timer dispatchTimer;

		private IWorkItemQueueProvider persistentQueueProvider;

		private HostStateProvider hostStateProvider;

		private bool isDispatchScheduled;

		private ExecutionLog logProvider;

		private SyncAgentConfiguration config;
	}
}
