using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage.RightsManagement
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PerTenantQueryController<T>
	{
		public PerTenantQueryController(IEqualityComparer<T> comparer)
		{
			this.dictionary = new Dictionary<T, PerTenantQueryController<T>.SynchronizedQueue>(comparer);
		}

		public bool EnqueueResult(T tenantId, RightsManagementAsyncResult result)
		{
			PerTenantQueryController<T>.SynchronizedQueue tenantQueue = this.GetTenantQueue(tenantId);
			return tenantQueue.Enqueue(result);
		}

		public void InvokeCallbacks(T tenantId, object result)
		{
			IEnumerable<RightsManagementAsyncResult> enumerable;
			lock (this.syncRoot)
			{
				PerTenantQueryController<T>.SynchronizedQueue tenantQueue = this.GetTenantQueue(tenantId);
				enumerable = tenantQueue.DequeueAll();
				this.RemoveTenantQueue(tenantId);
			}
			foreach (RightsManagementAsyncResult state in enumerable)
			{
				ThreadPool.QueueUserWorkItem(delegate(object o)
				{
					RightsManagementAsyncResult rightsManagementAsyncResult = (RightsManagementAsyncResult)o;
					rightsManagementAsyncResult.AddBreadCrumb(Constants.State.PerTenantQueryControllerInvokeCallback);
					rightsManagementAsyncResult.InvokeCallback(result);
				}, state);
			}
		}

		private PerTenantQueryController<T>.SynchronizedQueue GetTenantQueue(T tenantId)
		{
			PerTenantQueryController<T>.SynchronizedQueue synchronizedQueue;
			lock (this.syncRoot)
			{
				if (this.dictionary.TryGetValue(tenantId, out synchronizedQueue))
				{
					return synchronizedQueue;
				}
				synchronizedQueue = new PerTenantQueryController<T>.SynchronizedQueue();
				this.dictionary.Add(tenantId, synchronizedQueue);
			}
			ExTraceGlobals.RightsManagementTracer.TraceDebug<T>((long)this.GetHashCode(), "Created a new queue for tenant {0}", tenantId);
			return synchronizedQueue;
		}

		private void RemoveTenantQueue(T tenantId)
		{
			lock (this.syncRoot)
			{
				if (!this.dictionary.ContainsKey(tenantId))
				{
					return;
				}
				this.dictionary.Remove(tenantId);
			}
			ExTraceGlobals.RightsManagementTracer.TraceDebug<T>((long)this.GetHashCode(), "Removed queue for tenant {0}", tenantId);
		}

		private readonly object syncRoot = new object();

		private readonly Dictionary<T, PerTenantQueryController<T>.SynchronizedQueue> dictionary;

		private sealed class SynchronizedQueue
		{
			public SynchronizedQueue()
			{
				this.queue = new Queue<RightsManagementAsyncResult>();
			}

			public bool Enqueue(RightsManagementAsyncResult asyncResult)
			{
				int count;
				lock (this.syncRoot)
				{
					count = this.queue.Count;
					this.queue.Enqueue(asyncResult);
				}
				ExTraceGlobals.RightsManagementTracer.TraceDebug<int>((long)this.GetHashCode(), "Number of elements in the queue : {0}", count + 1);
				return count == 0;
			}

			public IEnumerable<RightsManagementAsyncResult> DequeueAll()
			{
				Queue<RightsManagementAsyncResult> queue = null;
				lock (this.syncRoot)
				{
					queue = this.queue;
					this.queue = new Queue<RightsManagementAsyncResult>();
				}
				ExTraceGlobals.RightsManagementTracer.TraceDebug<int>((long)this.GetHashCode(), "Dequeued elements from the queue. Number of entries: {0}", queue.Count);
				return queue;
			}

			private readonly object syncRoot = new object();

			private Queue<RightsManagementAsyncResult> queue;
		}
	}
}
