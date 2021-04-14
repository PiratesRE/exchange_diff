using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Office.CompliancePolicy.PolicySync;

namespace Microsoft.Exchange.Data.Storage.UnifiedPolicy
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SyncManager
	{
		private SyncManager()
		{
		}

		public static SyncManager Instance
		{
			get
			{
				if (!SyncManager.instance.initialized)
				{
					throw new InvalidOperationException("SyncManager.Initialize has not been called.");
				}
				return SyncManager.instance;
			}
		}

		public static void Initialize(SyncAgentContext syncAgentContext)
		{
			if (!SyncManager.instance.initialized)
			{
				lock (SyncManager.instance.initializeLockObject)
				{
					if (!SyncManager.instance.initialized && !SyncManager.instance.shuttingdown)
					{
						PersistentSyncWorkItemQueueProvider persistentQueueProvider = new PersistentSyncWorkItemQueueProvider();
						ParallelJobDispatcher jobDispatcher = new ParallelJobDispatcher(syncAgentContext, syncAgentContext.SyncAgentConfig.MaxSyncWorkItemsPerJob);
						MemoryWorkItemQueueProvider value = new MemoryWorkItemQueueProvider(persistentQueueProvider, jobDispatcher, syncAgentContext);
						SyncManager.instance.queues[NotificationType.Sync] = value;
						ParallelJobDispatcher jobDispatcher2 = new ParallelJobDispatcher(syncAgentContext, syncAgentContext.SyncAgentConfig.MaxPublishWorkItemsPerJob);
						MemoryWorkItemQueueProvider value2 = new MemoryWorkItemQueueProvider(persistentQueueProvider, jobDispatcher2, syncAgentContext);
						SyncManager.instance.queues[NotificationType.ApplicationStatus] = value2;
						SyncManager.instance.initialized = true;
					}
				}
			}
		}

		public static void Shutdown()
		{
			lock (SyncManager.instance.initializeLockObject)
			{
				SyncManager.instance.shuttingdown = true;
				if (!SyncManager.instance.initialized)
				{
					return;
				}
			}
			SyncManager.instance.InternalShutdown();
		}

		public static WorkItemBase EnqueueWorkItem(WorkItemBase workItem)
		{
			NotificationType index;
			if (workItem is SyncWorkItem)
			{
				index = NotificationType.Sync;
			}
			else
			{
				if (!(workItem is SyncStatusUpdateWorkitem))
				{
					throw new NotSupportedException("workitem type not supported");
				}
				index = NotificationType.ApplicationStatus;
			}
			SyncManager.Instance[index].Enqueue(workItem);
			return workItem;
		}

		internal IWorkItemQueueProvider this[NotificationType index]
		{
			get
			{
				if (!SyncManager.instance.initialized)
				{
					throw new InvalidOperationException("SyncManager.Initialize has not been called.");
				}
				return this.queues[index];
			}
		}

		private void InternalShutdown()
		{
		}

		private static readonly SyncManager instance = new SyncManager();

		private readonly object initializeLockObject = new object();

		private bool initialized;

		private bool shuttingdown;

		private Dictionary<NotificationType, IWorkItemQueueProvider> queues = new Dictionary<NotificationType, IWorkItemQueueProvider>();
	}
}
